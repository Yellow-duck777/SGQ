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
    public async Task<IActionResult> Index(string? busca, StatusRecall? status, DecisaoRecall? decisao, int? produtoId, int? loteId, DateOnly? inicio, DateOnly? fim)
    {
        var query = context.Recalls.Include(item => item.Produto).Include(item => item.Lote).AsQueryable();
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim();
            query = query.Where(recall => recall.Codigo.Contains(termo) || recall.Produto.Nome.Contains(termo) || recall.Lote.Numero.Contains(termo));
        }
        if (status.HasValue) query = query.Where(recall => recall.Status == status);
        if (decisao.HasValue) query = query.Where(recall => recall.Decisao == decisao);
        if (produtoId.HasValue) query = query.Where(recall => recall.ProdutoId == produtoId);
        if (loteId.HasValue) query = query.Where(recall => recall.LoteId == loteId);
        if (inicio.HasValue) query = query.Where(recall => recall.DataAbertura >= inicio);
        if (fim.HasValue) query = query.Where(recall => recall.DataAbertura <= fim);

        ViewBag.Produtos = new SelectList(await context.Produtos.OrderBy(produto => produto.Nome).ToListAsync(), "Id", "Nome", produtoId);
        ViewBag.Lotes = new SelectList(await context.Lotes.OrderBy(lote => lote.Numero).ToListAsync(), "Id", "Numero", loteId);
        return View(await query.OrderByDescending(recall => recall.CriadaEm).ToListAsync());
    }

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
        ViewBag.HistoricoAuditoria = await context.HistoricosAuditoria
            .Where(item => item.Entidade == nameof(Recall) && item.ChaveRegistro == id.ToString())
            .OrderByDescending(item => item.OcorridaEm)
            .ToListAsync();
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
        if (perfil == "RT" && (User.IsInRole(Roles.ResponsavelTecnico) || User.IsInRole(Roles.Administrador))) { recall.AprovadaRt = true; recall.ReprovadaRt = false; }
        else if (perfil == "GQ" && (User.IsInRole(Roles.GarantiaQualidade) || User.IsInRole(Roles.Administrador))) { recall.AprovadaGq = true; recall.ReprovadaGq = false; }
        else return Forbid();
        if ((recall.AprovadaRt && recall.ReprovadaGq) || (recall.AprovadaGq && recall.ReprovadaRt)) recall.Status = StatusRecall.AguardandoDecisaoCq;
        else if (recall.AprovadaRt && recall.AprovadaGq) recall.Status = StatusRecall.EmRecolhimento;
        await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Roles.ResponsavelTecnico + "," + Roles.GarantiaQualidade + "," + Roles.Administrador)]
    public async Task<IActionResult> Reprovar(int id, string perfil)
    {
        var recall = await context.Recalls.FindAsync(id); if (recall is null) return NotFound(); if (recall.Status != StatusRecall.AguardandoAprovacao) return RedirectToAction(nameof(Details), new { id });
        if (perfil == "RT" && (User.IsInRole(Roles.ResponsavelTecnico) || User.IsInRole(Roles.Administrador))) { recall.ReprovadaRt = true; recall.AprovadaRt = false; }
        else if (perfil == "GQ" && (User.IsInRole(Roles.GarantiaQualidade) || User.IsInRole(Roles.Administrador))) { recall.ReprovadaGq = true; recall.AprovadaGq = false; }
        else return Forbid();
        recall.Status = (recall.ReprovadaRt && recall.AprovadaGq) || (recall.ReprovadaGq && recall.AprovadaRt) ? StatusRecall.AguardandoDecisaoCq : StatusRecall.EmAvaliacao;
        await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Roles.ControleQualidade + "," + Roles.Administrador)]
    public async Task<IActionResult> DecidirDivergencia(int id, bool favoravel, string justificativa)
    {
        var recall = await context.Recalls.FindAsync(id); if (recall is null) return NotFound();
        if (recall.Status != StatusRecall.AguardandoDecisaoCq || string.IsNullOrWhiteSpace(justificativa) || justificativa.Trim().Length < 10) { TempData["Error"] = "A decisão do CQ requer justificativa de ao menos 10 caracteres."; return RedirectToAction(nameof(Details), new { id }); }
        recall.DecisaoCq = favoravel ? StatusDecisaoCq.Favoravel : StatusDecisaoCq.Desfavoravel; recall.JustificativaDecisaoCq = justificativa.Trim(); recall.UsuarioDecisaoCq = User.Identity?.Name ?? "Usuário autenticado"; recall.DecididaPeloCqEm = DateTimeOffset.UtcNow;
        recall.Status = favoravel ? StatusRecall.EmRecolhimento : StatusRecall.EmAvaliacao;
        await context.SaveChangesAsync(); TempData["Success"] = favoravel ? "Decisão favorável do CQ registrada; o Recall segue para recolhimento." : "Decisão desfavorável do CQ registrada; o Recall voltou para avaliação."; return RedirectToAction(nameof(Details), new { id });
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

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GarantiaQualidade + "," + Roles.ResponsavelTecnico)]
    public async Task<IActionResult> Reabrir(RecallReaberturaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Informe o motivo e a justificativa da reabertura (mínimo de 10 caracteres cada).";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        var recall = await context.Recalls.FindAsync(model.Id);
        if (recall is null) return NotFound();
        if (recall.Status != StatusRecall.Encerrado)
        {
            TempData["Error"] = "Somente Recall encerrado pode ser reaberto.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        recall.StatusAnteriorReabertura = recall.Status;
        recall.MotivoReabertura = model.Motivo.Trim();
        recall.JustificativaReabertura = model.Justificativa.Trim();
        recall.UsuarioReabertura = User.Identity?.Name ?? "Usuário autenticado";
        recall.ReabertaEm = DateTimeOffset.UtcNow;
        recall.AprovadaRt = false;
        recall.AprovadaGq = false;
        recall.Status = StatusRecall.EmAvaliacao;
        await context.SaveChangesAsync();
        TempData["Success"] = "Recall reaberto. A avaliação e as aprovações técnicas devem ser realizadas novamente.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Roles.GarantiaQualidade + "," + Roles.ResponsavelTecnico)]
    public async Task<IActionResult> Prorrogar(ProrrogacaoPrazoViewModel model)
    {
        if (!ModelState.IsValid || !model.NovaData.HasValue) return RedirectToAction(nameof(Details), new { id = model.Id });
        var item = await context.Recalls.FindAsync(model.Id); if (item is null) return NotFound();
        if (!item.DataAlvo.HasValue || model.NovaData <= item.DataAlvo) { TempData["Error"] = "Informe uma nova data posterior ao prazo atual."; return RedirectToAction(nameof(Details), new { id = model.Id }); }
        context.ProrrogacoesPrazo.Add(new ProrrogacaoPrazo { RecallId = item.Id, DataAnterior = item.DataAlvo.Value, NovaData = model.NovaData.Value, Motivo = model.Motivo.Trim(), ClienteComunicado = model.ClienteComunicado, RegistroComunicacaoCliente = model.RegistroComunicacaoCliente?.Trim(), Usuario = User.Identity?.Name ?? "Usuário autenticado", RegistradaEm = DateTimeOffset.UtcNow });
        item.DataAlvo = model.NovaData; await context.SaveChangesAsync(); TempData["Success"] = "Prazo prorrogado e registrado no histórico."; return RedirectToAction(nameof(Details), new { id = model.Id });
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
