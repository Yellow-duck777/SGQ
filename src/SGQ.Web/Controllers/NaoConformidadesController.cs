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
public class NaoConformidadesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index() => View(await context.NaoConformidades.Include(item => item.ReclamacaoCliente).Include(item => item.Produto).OrderByDescending(item => item.CriadaEm).ToListAsync());

    public async Task<IActionResult> Create()
    {
        await PopulateProdutos();
        return View(new NaoConformidadeCreateViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Create(NaoConformidadeCreateViewModel model)
    {
        if (model.ProdutoId.HasValue && !await context.Produtos.AnyAsync(item => item.Id == model.ProdutoId)) ModelState.AddModelError(nameof(model.ProdutoId), "Produto inválido.");
        if (!ModelState.IsValid) { await PopulateProdutos(model.ProdutoId); return View(model); }
        var year = model.DataAbertura.Year;
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var sequence = (await context.NaoConformidades.Where(item => item.Ano == year).MaxAsync(item => (int?)item.SequenciaAnual) ?? 0) + 1;
        var nc = new NaoConformidade { Ano = year, SequenciaAnual = sequence, Codigo = $"NC-{year}-{sequence:D6}", DataAbertura = model.DataAbertura, DataAlvo = model.DataAlvo, Origem = model.Origem, Area = model.Area, ProdutoId = model.ProdutoId, Descricao = model.Descricao, Classificacao = model.Classificacao, UsuarioAbertura = User.Identity?.Name ?? "Usuário autenticado", CriadaEm = DateTimeOffset.UtcNow };
        context.NaoConformidades.Add(nc); await context.SaveChangesAsync(); await transaction.CommitAsync();
        TempData["Success"] = $"Não Conformidade {nc.Codigo} criada.";
        return RedirectToAction(nameof(Details), new { id = nc.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var nc = await context.NaoConformidades.Include(item => item.ReclamacaoCliente).Include(item => item.Produto).Include(item => item.Acoes).SingleOrDefaultAsync(item => item.Id == id);
        if (nc is null) return NotFound();
        ViewBag.Anexos = await context.Anexos.Where(item => item.NaoConformidadeId == id).OrderByDescending(item => item.EnviadoEm).ToListAsync();
        return View(nc);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarInvestigacao(int id, string contencao, string investigacao, string causaProvavel, string causaRaiz, string metodoAnalise)
    {
        var nc = await context.NaoConformidades.FindAsync(id); if (nc is null) return NotFound();
        if (nc.Status != StatusNaoConformidade.EmInvestigacao) return RedirectToAction(nameof(Details), new { id });
        if (new[] { contencao, investigacao, causaProvavel, causaRaiz, metodoAnalise }.Any(string.IsNullOrWhiteSpace)) { TempData["Error"] = "Preencha todos os campos da investigação."; return RedirectToAction(nameof(Details), new { id }); }
        nc.Contencao = contencao; nc.Investigacao = investigacao; nc.CausaProvavel = causaProvavel; nc.CausaRaiz = causaRaiz; nc.MetodoAnalise = metodoAnalise; nc.Status = StatusNaoConformidade.EmTratamento;
        await context.SaveChangesAsync(); TempData["Success"] = "Investigação registrada. Inclua e conclua as ações necessárias."; return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarAcao(AcaoNaoConformidadeViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Details), new { id = model.NaoConformidadeId });
        var nc = await context.NaoConformidades.FindAsync(model.NaoConformidadeId); if (nc is null) return NotFound();
        if (nc.Status != StatusNaoConformidade.EmTratamento) return RedirectToAction(nameof(Details), new { id = model.NaoConformidadeId });
        context.AcoesNaoConformidade.Add(new AcaoNaoConformidade { NaoConformidadeId = nc.Id, Descricao = model.Descricao, Responsavel = model.Responsavel, Prazo = model.Prazo, Obrigatoria = model.Obrigatoria });
        await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id = nc.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ConcluirAcao(int id, int acaoId, string? evidencia)
    {
        var acao = await context.AcoesNaoConformidade.SingleOrDefaultAsync(item => item.Id == acaoId && item.NaoConformidadeId == id); if (acao is null) return NotFound();
        acao.DataConclusao = DateOnly.FromDateTime(DateTime.Today); acao.Evidencia = evidencia; await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AvaliarEficacia(int id, bool eficaz)
    {
        var nc = await context.NaoConformidades.Include(item => item.Acoes).SingleOrDefaultAsync(item => item.Id == id); if (nc is null) return NotFound();
        if (nc.Status != StatusNaoConformidade.EmTratamento || !nc.Acoes.Any() || nc.Acoes.Any(item => item.Obrigatoria && item.DataConclusao is null)) { TempData["Error"] = "Conclua todas as ações obrigatórias antes de avaliar a eficácia."; return RedirectToAction(nameof(Details), new { id }); }
        nc.Eficaz = eficaz; nc.Status = eficaz ? StatusNaoConformidade.AguardandoAprovacao : StatusNaoConformidade.EmInvestigacao;
        await context.SaveChangesAsync(); TempData["Success"] = eficaz ? "Eficácia registrada. A NC aguarda aprovações." : "Eficácia considerada ineficaz; a NC retornou para investigação."; return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.ResponsavelTecnico + "," + Roles.GarantiaQualidade + "," + Roles.Administrador)]
    public async Task<IActionResult> Aprovar(int id, string perfil)
    {
        var nc = await context.NaoConformidades.FindAsync(id); if (nc is null) return NotFound(); if (nc.Status != StatusNaoConformidade.AguardandoAprovacao) return RedirectToAction(nameof(Details), new { id });
        if (perfil == "RT" && (User.IsInRole(Roles.ResponsavelTecnico) || User.IsInRole(Roles.Administrador))) nc.AprovadaRt = true;
        else if (perfil == "GQ" && (User.IsInRole(Roles.GarantiaQualidade) || User.IsInRole(Roles.Administrador))) nc.AprovadaGq = true;
        else return Forbid();
        await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reprovar(int id)
    {
        var nc = await context.NaoConformidades.FindAsync(id); if (nc is null) return NotFound(); nc.AprovadaRt = false; nc.AprovadaGq = false; nc.Status = StatusNaoConformidade.EmTratamento; await context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Encerrar(int id)
    {
        var nc = await context.NaoConformidades.FindAsync(id); if (nc is null) return NotFound();
        if (nc.Status != StatusNaoConformidade.AguardandoAprovacao || !nc.AprovadaRt || !nc.AprovadaGq) { TempData["Error"] = "São necessárias as aprovações de RT e GQ antes do encerramento."; return RedirectToAction(nameof(Details), new { id }); }
        nc.Status = StatusNaoConformidade.Encerrada; nc.UsuarioEncerramento = User.Identity?.Name ?? "Usuário autenticado"; nc.EncerradaEm = DateTimeOffset.UtcNow; await context.SaveChangesAsync(); TempData["Success"] = "Não Conformidade encerrada."; return RedirectToAction(nameof(Details), new { id });
    }

    private async Task PopulateProdutos(int? produtoId = null) => ViewBag.Produtos = new SelectList(await context.Produtos.OrderBy(item => item.Nome).ToListAsync(), "Id", "Nome", produtoId);
}
