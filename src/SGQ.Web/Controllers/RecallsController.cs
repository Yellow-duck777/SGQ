using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.ViewModels;
using SGQ.Web.Security;

namespace SGQ.Web.Controllers;

[Authorize]
public class RecallsController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index() => View(await context.Recalls
        .Include(item => item.Produto).Include(item => item.Lote)
        .OrderByDescending(item => item.CriadaEm).ToListAsync());

    public async Task<IActionResult> Create()
    {
        await PopulateOptions();
        return View(new RecallCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecallCreateViewModel model)
    {
        var lote = await context.Lotes.SingleOrDefaultAsync(item => item.Id == model.LoteId);
        if (lote is null || lote.ProdutoId != model.ProdutoId)
            ModelState.AddModelError(nameof(model.LoteId), "O lote selecionado deve pertencer ao produto informado.");
        if (!await context.Produtos.AnyAsync(item => item.Id == model.ProdutoId))
            ModelState.AddModelError(nameof(model.ProdutoId), "Produto inválido.");
        if (model.NaoConformidadeId.HasValue && !await context.NaoConformidades.AnyAsync(item => item.Id == model.NaoConformidadeId))
            ModelState.AddModelError(nameof(model.NaoConformidadeId), "Não Conformidade inválida.");
        if (model.ReclamacaoClienteId.HasValue && !await context.ReclamacoesClientes.AnyAsync(item => item.Id == model.ReclamacaoClienteId))
            ModelState.AddModelError(nameof(model.ReclamacaoClienteId), "Reclamação inválida.");

        if (!ModelState.IsValid)
        {
            await PopulateOptions(model.ProdutoId, model.LoteId, model.NaoConformidadeId, model.ReclamacaoClienteId);
            return View(model);
        }

        var year = model.DataAbertura.Year;
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var lastSequence = await context.Recalls.Where(item => item.Ano == year)
            .MaxAsync(item => (int?)item.SequenciaAnual) ?? 0;
        var sequence = lastSequence + 1;
        var recall = new Recall
        {
            Ano = year, SequenciaAnual = sequence, Codigo = $"REC-{year}-{sequence:D6}",
            DataAbertura = model.DataAbertura, DataAlvo = model.DataAlvo, Origem = model.Origem,
            NaoConformidadeId = model.NaoConformidadeId, ReclamacaoClienteId = model.ReclamacaoClienteId,
            ProdutoId = model.ProdutoId, LoteId = model.LoteId, DataFabricacao = model.DataFabricacao,
            DataValidade = model.DataValidade, QuantidadeProduzida = model.QuantidadeProduzida,
            QuantidadeEstoque = model.QuantidadeEstoque, QuantidadeDistribuida = model.QuantidadeDistribuida,
            ClientesEnvolvidos = model.ClientesEnvolvidos, NaturezaOcorrencia = model.NaturezaOcorrencia,
            RiscoPotencial = model.RiscoPotencial, Decisao = model.Decisao,
            JustificativaDecisao = model.JustificativaDecisao,
            Status = model.Decisao == DecisaoRecall.NaoAplicavel ? StatusRecall.Encerrado : StatusRecall.EmAvaliacao,
            UsuarioAbertura = User.Identity?.Name ?? "Usuário autenticado", CriadaEm = DateTimeOffset.UtcNow
        };
        context.Recalls.Add(recall);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Success"] = $"Recall {recall.Codigo} registrado.";
        return RedirectToAction(nameof(Details), new { id = recall.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var recall = await context.Recalls.Include(item => item.Produto).Include(item => item.Lote)
            .Include(item => item.NaoConformidade).Include(item => item.ReclamacaoCliente)
            .Include(item => item.Retornos)
            .SingleOrDefaultAsync(item => item.Id == id);
        if (recall is null) return NotFound();
        ViewBag.Anexos = await context.Anexos.Where(item => item.RecallId == id).OrderByDescending(item => item.EnviadoEm).ToListAsync();
        return View(recall);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> SolicitarAprovacao(int id)
    {
        var recall = await context.Recalls.FindAsync(id); if (recall is null) return NotFound();
        if (recall.Status == StatusRecall.EmAvaliacao) { recall.Status = StatusRecall.AguardandoAprovacao; await context.SaveChangesAsync(); }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.ResponsavelTecnico + "," + Roles.GarantiaQualidade + "," + Roles.Administrador)]
    public async Task<IActionResult> Aprovar(int id, string perfil)
    {
        var recall = await context.Recalls.FindAsync(id); if (recall is null) return NotFound();
        if (recall.Status != StatusRecall.AguardandoAprovacao) return RedirectToAction(nameof(Details), new { id });
        if (perfil == "RT" && (User.IsInRole(Roles.ResponsavelTecnico) || User.IsInRole(Roles.Administrador))) recall.AprovadaRt = true;
        else if (perfil == "GQ" && (User.IsInRole(Roles.GarantiaQualidade) || User.IsInRole(Roles.Administrador))) recall.AprovadaGq = true;
        else return Forbid();
        if (recall.AprovadaRt && recall.AprovadaGq) recall.Status = StatusRecall.EmRecolhimento;
        await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarOperacao(int id, bool bloqueioRegistrado, string comunicacaoClientes, string autoridadeSanitaria, string protocoloAutoridade)
    {
        var recall = await context.Recalls.FindAsync(id); if (recall is null) return NotFound();
        if (recall.Status != StatusRecall.EmRecolhimento) return RedirectToAction(nameof(Details), new { id });
        if (!bloqueioRegistrado || string.IsNullOrWhiteSpace(comunicacaoClientes) || string.IsNullOrWhiteSpace(autoridadeSanitaria)) { TempData["Error"] = "Registre bloqueio e as comunicações obrigatórias antes de iniciar os retornos."; return RedirectToAction(nameof(Details), new { id }); }
        recall.BloqueioRegistrado = true; recall.ComunicacaoClientes = comunicacaoClientes; recall.AutoridadeSanitaria = autoridadeSanitaria; recall.ProtocoloAutoridade = protocoloAutoridade; recall.ComunicadaAutoridadeEm = DateTimeOffset.UtcNow; recall.Status = StatusRecall.AguardandoRetorno;
        await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarRetorno(int recallId, string cliente, decimal quantidade, DateOnly dataRetorno, string condicaoEmbalagem, string? condicaoLacre, string? avarias, string? documentoTransporte)
    {
        var recall = await context.Recalls.FindAsync(recallId); if (recall is null) return NotFound();
        if (recall.Status != StatusRecall.AguardandoRetorno) return RedirectToAction(nameof(Details), new { id = recallId });
        if (string.IsNullOrWhiteSpace(cliente) || string.IsNullOrWhiteSpace(condicaoEmbalagem) || quantidade <= 0) { TempData["Error"] = "Informe cliente, quantidade e condição da embalagem."; return RedirectToAction(nameof(Details), new { id = recallId }); }
        context.RetornosRecall.Add(new RetornoRecall { RecallId = recallId, Cliente = cliente, Quantidade = quantidade, DataRetorno = dataRetorno, CondicaoEmbalagem = condicaoEmbalagem, CondicaoLacre = condicaoLacre, Avarias = avarias, DocumentoTransporte = documentoTransporte });
        await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id = recallId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AvancarDestinacao(int id)
    {
        var recall = await context.Recalls.Include(item => item.Retornos).SingleOrDefaultAsync(item => item.Id == id); if (recall is null) return NotFound();
        if (recall.Status == StatusRecall.AguardandoRetorno && recall.Retornos.Any()) { recall.Status = StatusRecall.EmAvaliacaoDeDestinacao; await context.SaveChangesAsync(); }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarDestinacao(int id, string destinacao, string evidenciaDestinacao)
    {
        var recall = await context.Recalls.FindAsync(id); if (recall is null) return NotFound();
        if (recall.Status != StatusRecall.EmAvaliacaoDeDestinacao || string.IsNullOrWhiteSpace(destinacao) || string.IsNullOrWhiteSpace(evidenciaDestinacao)) return RedirectToAction(nameof(Details), new { id });
        recall.Destinacao = destinacao; recall.EvidenciaDestinacao = evidenciaDestinacao; recall.Status = StatusRecall.AguardandoEncerramento; await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Encerrar(int id)
    {
        var recall = await context.Recalls.FindAsync(id); if (recall is null) return NotFound();
        if (recall.Status != StatusRecall.AguardandoEncerramento || !recall.AprovadaRt || !recall.AprovadaGq || recall.ComunicadaAutoridadeEm is null) { TempData["Error"] = "O Recall precisa de aprovações, comunicação regulatória e destinação antes do encerramento."; return RedirectToAction(nameof(Details), new { id }); }
        recall.Status = StatusRecall.Encerrado; recall.UsuarioEncerramento = User.Identity?.Name ?? "Usuário autenticado"; recall.EncerradaEm = DateTimeOffset.UtcNow; await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    private async Task PopulateOptions(int? produtoId = null, int? loteId = null, int? ncId = null, int? rcId = null)
    {
        ViewBag.Produtos = new SelectList(await context.Produtos.OrderBy(item => item.Nome).ToListAsync(), "Id", "Nome", produtoId);
        ViewBag.Lotes = new SelectList(await context.Lotes.Include(item => item.Produto).OrderBy(item => item.Numero)
            .Select(item => new { item.Id, Nome = $"{item.Numero} — {item.Produto.Nome}" }).ToListAsync(), "Id", "Nome", loteId);
        ViewBag.NaoConformidades = new SelectList(await context.NaoConformidades.OrderByDescending(item => item.CriadaEm).ToListAsync(), "Id", "Codigo", ncId);
        ViewBag.Reclamacoes = new SelectList(await context.ReclamacoesClientes.OrderByDescending(item => item.CriadaEm).ToListAsync(), "Id", "Codigo", rcId);
    }
}
