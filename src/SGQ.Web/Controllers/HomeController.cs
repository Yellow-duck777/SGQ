using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGQ.Domain.Entities;
using SGQ.Domain.Enums;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Presentation;
using SGQ.Web.Security;
using SGQ.Web.Services;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Controllers;

public class HomeController(ApplicationDbContext context, IPrazoService prazoService) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        if (!Roles.Todos.Any(User.IsInRole)) return Forbid();

        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var limiteProximoVencimento = await prazoService.AdicionarDiasUteisAsync(hoje, 2);

        var resumoRc = ResumirPrazos(await context.ReclamacoesClientes.AsNoTracking()
            .Where(item => item.Status != StatusReclamacao.Encerrada).Select(item => item.DataAlvo).ToListAsync(), hoje, limiteProximoVencimento);
        var resumoNc = ResumirPrazos(await context.NaoConformidades.AsNoTracking()
            .Where(item => item.Status != StatusNaoConformidade.Encerrada).Select(item => item.DataAlvo).ToListAsync(), hoje, limiteProximoVencimento);
        var resumoRecall = ResumirPrazos(await context.Recalls.AsNoTracking()
            .Where(item => item.Status != StatusRecall.Encerrado).Select(item => item.DataAlvo).ToListAsync(), hoje, limiteProximoVencimento);

        var model = new DashboardViewModel
        {
            ReclamacoesAbertas = resumoRc.Abertos,
            NaoConformidadesAbertas = resumoNc.Abertos,
            RecallsAtivos = resumoRecall.Abertos,
            AguardandoAprovacao = await context.NaoConformidades.CountAsync(item => item.Status == StatusNaoConformidade.AguardandoAprovacao)
                + await context.Recalls.CountAsync(item => item.Status == StatusRecall.AguardandoAprovacao),
            ProcessosVencidos = resumoRc.Vencidos + resumoNc.Vencidos + resumoRecall.Vencidos,
            ProcessosProximosDoVencimento = resumoRc.VencendoEmBreve + resumoNc.VencendoEmBreve + resumoRecall.VencendoEmBreve,
            ClientesAtivos = await context.Clientes.CountAsync(),
            ProdutosCadastrados = await context.Produtos.CountAsync(),
            LotesCadastrados = await context.Lotes.CountAsync(),
            NomeUsuario = UsuarioApresentacao.PrimeiroNome(User.Identity?.Name),
            Perfis = UsuarioApresentacao.PerfisDoUsuario(User),
            ResumoReclamacoes = resumoRc,
            ResumoNaoConformidades = resumoNc,
            ResumoRecalls = resumoRecall,
            Atencao = await MontarAtencaoAsync(hoje),
            Movimentos = await ListarMovimentosAsync(),
            ReclamacoesRecentes = await context.ReclamacoesClientes
                .AsNoTracking()
                .OrderByDescending(item => item.CriadaEm)
                .Take(5)
                .Select(item => new DashboardReclamacaoViewModel
                {
                    Id = item.Id,
                    Codigo = item.Codigo,
                    Cliente = item.Cliente.Nome,
                    Produto = item.Produto.Nome,
                    Status = item.Status,
                    CriadaEm = item.CriadaEm
                })
                .ToListAsync()
        };

        return View(model);
    }

    private static DashboardResumoProcesso ResumirPrazos(IReadOnlyCollection<DateOnly?> prazos, DateOnly hoje, DateOnly limite) => new()
    {
        Abertos = prazos.Count,
        Vencidos = prazos.Count(prazo => prazo.HasValue && prazo < hoje),
        VencendoEmBreve = prazos.Count(prazo => prazo.HasValue && prazo >= hoje && prazo <= limite)
    };

    /// <summary>Pendências que dependem do perfil do usuário atual; Administrador enxerga todas, Auditor apenas os vencidos.</summary>
    private async Task<IReadOnlyList<DashboardAtencaoViewModel>> MontarAtencaoAsync(DateOnly hoje)
    {
        var admin = User.IsInRole(Roles.Administrador);
        var gq = User.IsInRole(Roles.GarantiaQualidade);
        var rt = User.IsInRole(Roles.ResponsavelTecnico);
        var cq = User.IsInRole(Roles.ControleQualidade);
        var itens = new List<DashboardAtencaoViewModel>();

        if (gq || admin)
        {
            var rc = context.ReclamacoesClientes.AsNoTracking();
            await AdicionarAsync(itens, "RC", "Reclamacoes", nameof(StatusReclamacao.AguardandoValidacaoGq),
                "Reclamações aguardando validação", "A GQ precisa validar e classificar antes de seguir.",
                rc.Where(item => item.Status == StatusReclamacao.AguardandoValidacaoGq).Select(item => item.DataAlvo));
            await AdicionarAsync(itens, "RC", "Reclamacoes", nameof(StatusReclamacao.AguardandoConclusao),
                "Reclamações aguardando conclusão", "A investigação terminou e falta registrar a conclusão.",
                rc.Where(item => item.Status == StatusReclamacao.AguardandoConclusao).Select(item => item.DataAlvo));
        }

        if (gq || rt || admin)
        {
            var nc = context.NaoConformidades.AsNoTracking().Where(item => item.Status == StatusNaoConformidade.AguardandoAprovacao);
            var recall = context.Recalls.AsNoTracking().Where(item => item.Status == StatusRecall.AguardandoAprovacao);
            if (!admin)
            {
                nc = nc.Where(item => (gq && !item.AprovadaGq) || (rt && !item.AprovadaRt));
                recall = recall.Where(item => (gq && !item.AprovadaGq) || (rt && !item.AprovadaRt));
            }
            var descricao = admin ? "Aguardam o parecer do RT e da GQ." : "Aguardam o seu parecer.";
            await AdicionarAsync(itens, "NC", "NaoConformidades", nameof(StatusNaoConformidade.AguardandoAprovacao),
                "Não conformidades aguardando aprovação", descricao, nc.Select(item => item.DataAlvo));
            await AdicionarAsync(itens, "RE", "Recalls", nameof(StatusRecall.AguardandoAprovacao),
                "Recalls aguardando aprovação", descricao, recall.Select(item => item.DataAlvo));
        }

        if (cq || admin)
        {
            await AdicionarAsync(itens, "NC", "NaoConformidades", nameof(StatusNaoConformidade.AguardandoDecisaoCq),
                "Não conformidades aguardando decisão do CQ", "O Controle de Qualidade precisa registrar a decisão.",
                context.NaoConformidades.AsNoTracking().Where(item => item.Status == StatusNaoConformidade.AguardandoDecisaoCq).Select(item => item.DataAlvo));
            await AdicionarAsync(itens, "RE", "Recalls", nameof(StatusRecall.AguardandoDecisaoCq),
                "Recalls aguardando decisão do CQ", "O Controle de Qualidade precisa registrar a decisão.",
                context.Recalls.AsNoTracking().Where(item => item.Status == StatusRecall.AguardandoDecisaoCq).Select(item => item.DataAlvo));
        }

        const string vencidoDescricao = "O prazo já passou e o processo continua aberto.";
        await AdicionarAsync(itens, "RC", "Reclamacoes", null, "Reclamações com prazo vencido", vencidoDescricao,
            context.ReclamacoesClientes.AsNoTracking().Where(item => item.Status != StatusReclamacao.Encerrada && item.DataAlvo < hoje).Select(item => item.DataAlvo), critico: true);
        await AdicionarAsync(itens, "NC", "NaoConformidades", null, "Não conformidades com prazo vencido", vencidoDescricao,
            context.NaoConformidades.AsNoTracking().Where(item => item.Status != StatusNaoConformidade.Encerrada && item.DataAlvo < hoje).Select(item => item.DataAlvo), critico: true);
        await AdicionarAsync(itens, "RE", "Recalls", null, "Recalls com prazo vencido", vencidoDescricao,
            context.Recalls.AsNoTracking().Where(item => item.Status != StatusRecall.Encerrado && item.DataAlvo < hoje).Select(item => item.DataAlvo), critico: true);

        return itens;
    }

    private static async Task AdicionarAsync(List<DashboardAtencaoViewModel> itens, string sigla, string controller, string? status,
        string titulo, string descricao, IQueryable<DateOnly?> prazos, bool critico = false)
    {
        var quantidade = await prazos.CountAsync();
        if (quantidade == 0) return;
        var datas = await prazos.Where(prazo => prazo != null).ToListAsync();
        itens.Add(new DashboardAtencaoViewModel
        {
            Sigla = sigla, Controller = controller, Status = status, Titulo = titulo, Descricao = descricao, Contagem = quantidade,
            MenorPrazo = datas.Count == 0 ? null : datas.Min(), Critico = critico
        });
    }

    private async Task<IReadOnlyList<DashboardMovimentoViewModel>> ListarMovimentosAsync()
    {
        var rcs = await context.ReclamacoesClientes.AsNoTracking().OrderByDescending(item => item.CriadaEm).Take(6)
            .Select(item => new DashboardMovimentoViewModel
            {
                Sigla = "RC", Controller = "Reclamacoes", Id = item.Id, Codigo = item.Codigo, Titulo = item.Cliente.Nome, Subtitulo = item.Produto.Nome,
                Status = item.Status, Encerrado = item.Status == StatusReclamacao.Encerrada, DataAlvo = item.DataAlvo, CriadaEm = item.CriadaEm
            }).ToListAsync();
        var ncs = await context.NaoConformidades.AsNoTracking().OrderByDescending(item => item.CriadaEm).Take(6)
            .Select(item => new DashboardMovimentoViewModel
            {
                Sigla = "NC", Controller = "NaoConformidades", Id = item.Id, Codigo = item.Codigo, Titulo = item.Area,
                Subtitulo = item.Produto != null ? item.Produto.Nome : "Sem produto",
                Status = item.Status, Encerrado = item.Status == StatusNaoConformidade.Encerrada, DataAlvo = item.DataAlvo, CriadaEm = item.CriadaEm
            }).ToListAsync();
        var recalls = await context.Recalls.AsNoTracking().OrderByDescending(item => item.CriadaEm).Take(6)
            .Select(item => new DashboardMovimentoViewModel
            {
                Sigla = "RE", Controller = "Recalls", Id = item.Id, Codigo = item.Codigo, Titulo = item.Produto.Nome, Subtitulo = "Lote " + item.Lote.Numero,
                Status = item.Status, Encerrado = item.Status == StatusRecall.Encerrado, DataAlvo = item.DataAlvo, CriadaEm = item.CriadaEm
            }).ToListAsync();
        return rcs.Concat(ncs).Concat(recalls).OrderByDescending(item => item.CriadaEm).Take(8).ToList();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
