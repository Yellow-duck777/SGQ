using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGQ.Domain.Entities;
using SGQ.Domain.Enums;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.ViewModels;
using SGQ.Web.Security;
using SGQ.Web.Services;

namespace SGQ.Web.Controllers;

[Authorize]
public class NaoConformidadesController(ApplicationDbContext context, IPrazoService prazoService, IFluxoNotificacaoService? notificacoes = null) : Controller
{
    public async Task<IActionResult> Index(string? busca, StatusNaoConformidade? status, ClassificacaoOcorrencia? classificacao, OrigemNaoConformidade? origem, int? produtoId, string? responsavel, DateOnly? inicio, DateOnly? fim)
    {
        var query = context.NaoConformidades.Include(item => item.ReclamacaoCliente).Include(item => item.Produto).AsQueryable();
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim();
            query = query.Where(nc => nc.Codigo.Contains(termo) || nc.Area.Contains(termo) ||
                (nc.Produto != null && nc.Produto.Nome.Contains(termo)) ||
                (nc.ReclamacaoCliente != null && nc.ReclamacaoCliente.Codigo.Contains(termo)));
        }
        if (status.HasValue) query = query.Where(nc => nc.Status == status);
        if (classificacao.HasValue) query = query.Where(nc => nc.Classificacao == classificacao);
        if (origem.HasValue) query = query.Where(nc => nc.Origem == origem);
        if (produtoId.HasValue) query = query.Where(nc => nc.ProdutoId == produtoId);
        if (!string.IsNullOrWhiteSpace(responsavel))
        {
            var termo = responsavel.Trim();
            query = query.Where(nc => nc.UsuarioAbertura.Contains(termo) ||
                nc.Acoes.Any(acao => acao.Responsavel.Contains(termo)));
        }
        if (inicio.HasValue) query = query.Where(nc => nc.DataAbertura >= inicio);
        if (fim.HasValue) query = query.Where(nc => nc.DataAbertura <= fim);

        ViewBag.Produtos = new SelectList(await context.Produtos.OrderBy(produto => produto.Nome).ToListAsync(), "Id", "Nome", produtoId);
        return View(await query.OrderByDescending(nc => nc.CriadaEm).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await PopulateProdutos();
        return View(new NaoConformidadeCreateViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Create(NaoConformidadeCreateViewModel model)
    {
        if (model.Classificacao == ClassificacaoOcorrencia.Critica && !model.DataAlvo.HasValue)
            ModelState.AddModelError(nameof(model.DataAlvo), "A GQ deve definir a data-alvo para NC crítica.");
        if (model.ProdutoId.HasValue && !await context.Produtos.AnyAsync(item => item.Id == model.ProdutoId)) ModelState.AddModelError(nameof(model.ProdutoId), "Produto inválido.");
        if (!ModelState.IsValid) { await PopulateProdutos(model.ProdutoId); return View(model); }
        if (model.Classificacao is ClassificacaoOcorrencia.Maior or ClassificacaoOcorrencia.Menor)
            model.DataAlvo = await prazoService.CalcularPrazoNaoConformidadeAsync(model.DataAbertura, model.Classificacao);

        var year = model.DataAbertura.Year;
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var sequence = (await context.NaoConformidades.Where(item => item.Ano == year).MaxAsync(item => (int?)item.SequenciaAnual) ?? 0) + 1;
        var nc = new NaoConformidade { Ano = year, SequenciaAnual = sequence, Codigo = $"NC-{year}-{sequence:D6}", DataAbertura = model.DataAbertura, DataAlvo = model.DataAlvo, Origem = model.Origem, Area = model.Area, ProdutoId = model.ProdutoId, Descricao = model.Descricao, Classificacao = model.Classificacao, UsuarioAbertura = User.Identity?.Name ?? "Usuário autenticado", CriadaEm = DateTimeOffset.UtcNow };
        context.NaoConformidades.Add(nc); await context.SaveChangesAsync(); await transaction.CommitAsync();
        if (nc.Classificacao == ClassificacaoOcorrencia.Critica)
            await NotificarAsync("NcCritica", nc.Codigo, [Roles.GarantiaQualidade, Roles.ResponsavelTecnico, Roles.ControleQualidade],
                $"SGQ: NC crítica — {nc.Codigo}", $"A Não Conformidade {nc.Codigo} foi classificada como Crítica.");
        TempData["Success"] = $"Não Conformidade {nc.Codigo} criada.";
        return RedirectToAction(nameof(Details), new { id = nc.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var nc = await context.NaoConformidades.Include(item => item.ReclamacaoCliente).Include(item => item.Produto).Include(item => item.Acoes).SingleOrDefaultAsync(item => item.Id == id);
        if (nc is null) return NotFound();
        ViewBag.Anexos = await AnexosDoProcesso(context.Anexos, id).OrderByDescending(item => item.EnviadoEm).ToListAsync();
        ViewBag.HistoricoAuditoria = await context.HistoricosAuditoria
            .Where(item => item.Entidade == nameof(NaoConformidade) && item.ChaveRegistro == id.ToString())
            .OrderByDescending(item => item.OcorridaEm)
            .ToListAsync();
        return View(nc);
    }

    private static IQueryable<Anexo> AnexosDoProcesso(IQueryable<Anexo> anexos, int id) => anexos
        .Where(item => item.NaoConformidadeId == id || item.Vinculos.Any(vinculo => vinculo.NaoConformidadeId == id))
        .Include(item => item.ReclamacaoCliente).Include(item => item.NaoConformidade).Include(item => item.Recall)
        .Include(item => item.Vinculos).ThenInclude(item => item.ReclamacaoCliente)
        .Include(item => item.Vinculos).ThenInclude(item => item.NaoConformidade)
        .Include(item => item.Vinculos).ThenInclude(item => item.Recall);

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
    public async Task<IActionResult> SolicitarLaboratorio(SolicitarLaboratorioExternoViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Details), new { id = model.Id });
        var item = await context.NaoConformidades.FindAsync(model.Id); if (item is null) return NotFound();
        if (item.Status != StatusNaoConformidade.EmInvestigacao) return RedirectToAction(nameof(Details), new { id = model.Id });
        item.LaboratorioExterno = model.Laboratorio.Trim(); item.DataEnvioAmostraLaboratorio = model.DataEnvioAmostra; item.LaudoLaboratorioAnexoId = null; item.Status = StatusNaoConformidade.AguardandoLaboratorioExterno;
        await context.SaveChangesAsync();
        await NotificarAsync("LaboratorioExternoSolicitado", item.Codigo, [Roles.GarantiaQualidade, Roles.ResponsavelTecnico],
            $"SGQ: laboratório externo solicitado — {item.Codigo}", $"A Não Conformidade {item.Codigo} aguarda análise do laboratório {item.LaboratorioExterno}.");
        TempData["Success"] = "Laboratório externo solicitado."; return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarResultadoLaboratorio(ResultadoLaboratorioExternoViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Details), new { id = model.Id });
        var item = await context.NaoConformidades.FindAsync(model.Id); if (item is null) return NotFound();
        var laudoValido = item.LaudoLaboratorioAnexoId.HasValue && await context.Anexos.AnyAsync(a => a.Id == item.LaudoLaboratorioAnexoId && a.NaoConformidadeId == item.Id && a.Critico && a.Ativo);
        if (item.Status != StatusNaoConformidade.AguardandoLaboratorioExterno || !laudoValido) { TempData["Error"] = "Envie o laudo como anexo crítico antes de registrar o resultado."; return RedirectToAction(nameof(Details), new { id = model.Id }); }
        item.DataRecebimentoResultadoLaboratorio = model.DataRecebimento; item.IdentificacaoLaudoLaboratorio = model.IdentificacaoLaudo.Trim(); item.ResultadoLaboratorio = model.Resultado.Trim(); item.Status = StatusNaoConformidade.EmInvestigacao;
        await context.SaveChangesAsync();
        await NotificarAsync("ResultadoLaboratorioRecebido", item.Codigo, [Roles.GarantiaQualidade, Roles.ResponsavelTecnico],
            $"SGQ: resultado de laboratório recebido — {item.Codigo}", $"O resultado do laboratório externo da Não Conformidade {item.Codigo} foi registrado e o processo voltou para investigação.");
        TempData["Success"] = "Resultado laboratorial registrado. A NC voltou para investigação."; return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Roles.GarantiaQualidade + "," + Roles.ResponsavelTecnico)]
    public async Task<IActionResult> Prorrogar(ProrrogacaoPrazoViewModel model)
    {
        if (!ModelState.IsValid || !model.NovaData.HasValue) return RedirectToAction(nameof(Details), new { id = model.Id });
        var item = await context.NaoConformidades.FindAsync(model.Id); if (item is null) return NotFound();
        if (!item.DataAlvo.HasValue || model.NovaData <= item.DataAlvo) { TempData["Error"] = "Informe uma nova data posterior ao prazo atual."; return RedirectToAction(nameof(Details), new { id = model.Id }); }
        context.ProrrogacoesPrazo.Add(new ProrrogacaoPrazo { NaoConformidadeId = item.Id, DataAnterior = item.DataAlvo.Value, NovaData = model.NovaData.Value, Motivo = model.Motivo.Trim(), ClienteComunicado = model.ClienteComunicado, RegistroComunicacaoCliente = model.RegistroComunicacaoCliente?.Trim(), Usuario = User.Identity?.Name ?? "Usuário autenticado", RegistradaEm = DateTimeOffset.UtcNow });
        item.DataAlvo = model.NovaData; await context.SaveChangesAsync();
        await NotificarAsync("ProrrogacaoRegistrada", item.Codigo, [Roles.GarantiaQualidade, Roles.ResponsavelTecnico],
            $"SGQ: prazo prorrogado — {item.Codigo}", $"O prazo da Não Conformidade {item.Codigo} foi prorrogado para {item.DataAlvo:dd/MM/yyyy}.");
        TempData["Success"] = "Prazo prorrogado e registrado no histórico."; return RedirectToAction(nameof(Details), new { id = model.Id });
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
        ZerarRodadaDeAprovacao(nc);
        await context.SaveChangesAsync();
        if (eficaz)
            await NotificarAsync("NcAguardandoAprovacao", nc.Codigo, [Roles.ResponsavelTecnico, Roles.GarantiaQualidade],
                $"SGQ: NC aguardando aprovação — {nc.Codigo}", $"A eficácia da Não Conformidade {nc.Codigo} foi registrada e ela aguarda aprovação de RT e GQ.");
        TempData["Success"] = eficaz ? "Eficácia registrada. A NC aguarda aprovações." : "Eficácia considerada ineficaz; a NC retornou para investigação."; return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.ResponsavelTecnico + "," + Roles.GarantiaQualidade)]
    public async Task<IActionResult> Aprovar(int id, string perfil)
    {
        var nc = await context.NaoConformidades.FindAsync(id); if (nc is null) return NotFound(); if (nc.Status != StatusNaoConformidade.AguardandoAprovacao) return RedirectToAction(nameof(Details), new { id });
        var usuario = User.Identity?.Name ?? "Usuário autenticado";
        if (nc.DecisaoCq == StatusDecisaoCq.Favoravel) { TempData["Error"] = "O CQ já decidiu a divergência; a NC aguarda o encerramento pela GQ."; return RedirectToAction(nameof(Details), new { id }); }
        if (perfil == "RT" && User.IsInRole(Roles.ResponsavelTecnico))
        {
            if (MesmoUsuario(nc.UsuarioParecerGq, usuario)) return PareceresDistintosExigidos(id);
            nc.AprovadaRt = true; nc.ReprovadaRt = false; nc.UsuarioParecerRt = usuario;
        }
        else if (perfil == "GQ" && User.IsInRole(Roles.GarantiaQualidade))
        {
            if (MesmoUsuario(nc.UsuarioParecerRt, usuario)) return PareceresDistintosExigidos(id);
            nc.AprovadaGq = true; nc.ReprovadaGq = false; nc.UsuarioParecerGq = usuario;
        }
        else return Forbid();
        if ((nc.AprovadaRt && nc.ReprovadaGq) || (nc.AprovadaGq && nc.ReprovadaRt)) nc.Status = StatusNaoConformidade.AguardandoDecisaoCq;
        await context.SaveChangesAsync();
        if (nc.Status == StatusNaoConformidade.AguardandoDecisaoCq)
            await NotificarAsync("DivergenciaRtGq", nc.Codigo, [Roles.ControleQualidade],
                $"SGQ: divergência requer decisão do CQ — {nc.Codigo}", $"Há divergência entre RT e GQ na aprovação da Não Conformidade {nc.Codigo}. O CQ deve registrar a decisão.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.ResponsavelTecnico + "," + Roles.GarantiaQualidade)]
    public async Task<IActionResult> Reprovar(int id, string perfil)
    {
        var nc = await context.NaoConformidades.FindAsync(id); if (nc is null) return NotFound(); if (nc.Status != StatusNaoConformidade.AguardandoAprovacao) return RedirectToAction(nameof(Details), new { id });
        var usuario = User.Identity?.Name ?? "Usuário autenticado";
        if (nc.DecisaoCq == StatusDecisaoCq.Favoravel) { TempData["Error"] = "O CQ já decidiu a divergência; a NC aguarda o encerramento pela GQ."; return RedirectToAction(nameof(Details), new { id }); }
        if (perfil == "RT" && User.IsInRole(Roles.ResponsavelTecnico))
        {
            if (MesmoUsuario(nc.UsuarioParecerGq, usuario)) return PareceresDistintosExigidos(id);
            nc.ReprovadaRt = true; nc.AprovadaRt = false; nc.UsuarioParecerRt = usuario;
        }
        else if (perfil == "GQ" && User.IsInRole(Roles.GarantiaQualidade))
        {
            if (MesmoUsuario(nc.UsuarioParecerRt, usuario)) return PareceresDistintosExigidos(id);
            nc.ReprovadaGq = true; nc.AprovadaGq = false; nc.UsuarioParecerGq = usuario;
        }
        else return Forbid();
        nc.Status = (nc.ReprovadaRt && nc.AprovadaGq) || (nc.ReprovadaGq && nc.AprovadaRt) ? StatusNaoConformidade.AguardandoDecisaoCq : StatusNaoConformidade.EmTratamento;
        await context.SaveChangesAsync();
        if (nc.Status == StatusNaoConformidade.AguardandoDecisaoCq)
            await NotificarAsync("DivergenciaRtGq", nc.Codigo, [Roles.ControleQualidade],
                $"SGQ: divergência requer decisão do CQ — {nc.Codigo}", $"Há divergência entre RT e GQ na aprovação da Não Conformidade {nc.Codigo}. O CQ deve registrar a decisão.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Roles.ControleQualidade + "," + Roles.Administrador)]
    public async Task<IActionResult> DecidirDivergencia(int id, bool favoravel, string justificativa)
    {
        var nc = await context.NaoConformidades.FindAsync(id); if (nc is null) return NotFound();
        if (nc.Status != StatusNaoConformidade.AguardandoDecisaoCq || string.IsNullOrWhiteSpace(justificativa) || justificativa.Trim().Length < 10) { TempData["Error"] = "A decisão do CQ requer justificativa de ao menos 10 caracteres."; return RedirectToAction(nameof(Details), new { id }); }
        nc.DecisaoCq = favoravel ? StatusDecisaoCq.Favoravel : StatusDecisaoCq.Desfavoravel; nc.JustificativaDecisaoCq = justificativa.Trim(); nc.UsuarioDecisaoCq = User.Identity?.Name ?? "Usuário autenticado"; nc.DecididaPeloCqEm = DateTimeOffset.UtcNow;
        nc.Status = favoravel ? StatusNaoConformidade.AguardandoAprovacao : StatusNaoConformidade.EmTratamento;
        await context.SaveChangesAsync();
        await NotificarAsync("DecisaoCqRegistrada", nc.Codigo, [Roles.ResponsavelTecnico, Roles.GarantiaQualidade],
            $"SGQ: decisão do CQ registrada — {nc.Codigo}", $"O CQ registrou decisão {(favoravel ? "favorável" : "desfavorável")} para a Não Conformidade {nc.Codigo}.");
        TempData["Success"] = favoravel ? "Decisão favorável do CQ registrada; a NC pode ser encerrada pela GQ." : "Decisão desfavorável do CQ registrada; a NC retornou para tratamento."; return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Encerrar(int id)
    {
        var nc = await context.NaoConformidades.FindAsync(id); if (nc is null) return NotFound();
        var laudoValido = string.IsNullOrWhiteSpace(nc.LaboratorioExterno) || (nc.LaudoLaboratorioAnexoId.HasValue && await context.Anexos.AnyAsync(a => a.Id == nc.LaudoLaboratorioAnexoId && a.NaoConformidadeId == id && a.Critico && a.Ativo));
        if (nc.Status != StatusNaoConformidade.AguardandoAprovacao || (!(nc.AprovadaRt && nc.AprovadaGq) && nc.DecisaoCq != StatusDecisaoCq.Favoravel) || !laudoValido) { TempData["Error"] = "São necessárias as aprovações ou decisão favorável do CQ e, quando aplicável, um laudo crítico ativo antes do encerramento."; return RedirectToAction(nameof(Details), new { id }); }
        nc.Status = StatusNaoConformidade.Encerrada; nc.UsuarioEncerramento = User.Identity?.Name ?? "Usuário autenticado"; nc.EncerradaEm = DateTimeOffset.UtcNow; await context.SaveChangesAsync();
        await NotificarAsync("ProcessoEncerrado", nc.Codigo, [Roles.GarantiaQualidade, Roles.ResponsavelTecnico],
            $"SGQ: processo encerrado — {nc.Codigo}", $"A Não Conformidade {nc.Codigo} foi encerrada.", [nc.UsuarioAbertura, nc.UsuarioDecisaoCq ?? string.Empty, nc.UsuarioEncerramento]);
        TempData["Success"] = "Não Conformidade encerrada."; return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GarantiaQualidade + "," + Roles.ResponsavelTecnico + "," + Roles.Auditor)]
    public async Task<IActionResult> Reabrir(NaoConformidadeReaberturaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Informe uma justificativa de reabertura com pelo menos 10 caracteres.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        var nc = await context.NaoConformidades.FindAsync(model.Id);
        if (nc is null) return NotFound();
        if (nc.Status != StatusNaoConformidade.Encerrada)
        {
            TempData["Error"] = "Somente Não Conformidade encerrada pode ser reaberta.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        nc.StatusAnteriorReabertura = nc.Status;
        nc.JustificativaReabertura = model.Justificativa.Trim();
        nc.UsuarioReabertura = User.Identity?.Name ?? "Usuário autenticado";
        nc.ReabertaEm = DateTimeOffset.UtcNow;
        nc.UsuarioEncerramento = null;
        nc.EncerradaEm = null;
        ZerarRodadaDeAprovacao(nc);
        nc.Eficaz = null;
        nc.Status = StatusNaoConformidade.EmInvestigacao;
        await context.SaveChangesAsync();
        TempData["Success"] = "Não Conformidade reaberta. Revise a investigação, as ações e a eficácia antes das novas aprovações.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    private async Task PopulateProdutos(int? produtoId = null) => ViewBag.Produtos = new SelectList(await context.Produtos.OrderBy(item => item.Nome).ToListAsync(), "Id", "Nome", produtoId);

    private static bool MesmoUsuario(string? parecerAnterior, string usuario) =>
        parecerAnterior is not null && string.Equals(parecerAnterior, usuario, StringComparison.OrdinalIgnoreCase);

    private IActionResult PareceresDistintosExigidos(int id)
    {
        TempData["Error"] = "Os pareceres de RT e GQ devem ser dados por contas distintas.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // Cada rodada de aprovação começa limpa: pareceres, decisão do CQ e autores anteriores não se acumulam.
    private static void ZerarRodadaDeAprovacao(NaoConformidade nc)
    {
        nc.AprovadaRt = false; nc.AprovadaGq = false;
        nc.ReprovadaRt = false; nc.ReprovadaGq = false;
        nc.UsuarioParecerRt = null; nc.UsuarioParecerGq = null;
        nc.DecisaoCq = null; nc.JustificativaDecisaoCq = null; nc.UsuarioDecisaoCq = null; nc.DecididaPeloCqEm = null;
    }

    private Task NotificarAsync(string tipo, string referencia, IEnumerable<string> perfis, string assunto, string corpo, IEnumerable<string>? usuariosEnvolvidos = null) =>
        notificacoes?.NotificarAsync(tipo, referencia, perfis, assunto, corpo, usuariosEnvolvidos) ?? Task.CompletedTask;
}
