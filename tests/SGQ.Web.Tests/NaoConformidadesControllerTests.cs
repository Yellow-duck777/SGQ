using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Controllers;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Services;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Tests;

public class NaoConformidadesControllerTests
{
    [Fact]
    public async Task Reprovar_QuandoGqAprovouErtReprova_EncaminhaParaDecisaoDoCq()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.AguardandoAprovacao, aprovadaGq: true);
        var controller = CriarController(context, "rt", "RT");

        var resultado = await controller.Reprovar(nc.Id, "RT");

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizada = await context.NaoConformidades.FindAsync(nc.Id);
        Assert.True(atualizada!.ReprovadaRt);
        Assert.False(atualizada.AprovadaRt);
        Assert.Equal(StatusNaoConformidade.AguardandoDecisaoCq, atualizada.Status);
    }

    [Fact]
    public async Task DecidirDivergencia_Favoravel_RegistraDecisaoERetornaParaAprovacao()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.AguardandoDecisaoCq);
        var controller = CriarController(context, "cq", "CQ");

        var resultado = await controller.DecidirDivergencia(nc.Id, favoravel: true, "Parecer técnico devidamente fundamentado.");

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizada = await context.NaoConformidades.FindAsync(nc.Id);
        Assert.Equal(StatusDecisaoCq.Favoravel, atualizada!.DecisaoCq);
        Assert.Equal("cq", atualizada.UsuarioDecisaoCq);
        Assert.NotNull(atualizada.DecididaPeloCqEm);
        Assert.Equal(StatusNaoConformidade.AguardandoAprovacao, atualizada.Status);
    }

    [Fact]
    public async Task DecidirDivergencia_SemJustificativaSuficiente_MantemAguardandoDecisaoDoCq()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.AguardandoDecisaoCq);
        var controller = CriarController(context, "cq", "CQ");

        var resultado = await controller.DecidirDivergencia(nc.Id, favoravel: true, "Curta");

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizada = await context.NaoConformidades.FindAsync(nc.Id);
        Assert.Equal(StatusNaoConformidade.AguardandoDecisaoCq, atualizada!.Status);
        Assert.Null(atualizada.DecisaoCq);
        Assert.Null(atualizada.DecididaPeloCqEm);
    }

    [Fact]
    public async Task Reabrir_NcEncerrada_ResetaAprovacoesERetornaParaInvestigacao()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.Encerrada, aprovadaRt: true, aprovadaGq: true, eficaz: true);
        var controller = CriarController(context, "gq", "GQ");

        var resultado = await controller.Reabrir(new NaoConformidadeReaberturaViewModel
        {
            Id = nc.Id,
            Justificativa = "Nova evidência exige revisão da investigação."
        });

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizada = await context.NaoConformidades.FindAsync(nc.Id);
        Assert.Equal(StatusNaoConformidade.Encerrada, atualizada!.StatusAnteriorReabertura);
        Assert.Equal(StatusNaoConformidade.EmInvestigacao, atualizada.Status);
        Assert.False(atualizada.AprovadaRt);
        Assert.False(atualizada.AprovadaGq);
        Assert.Null(atualizada.Eficaz);
        Assert.Equal("gq", atualizada.UsuarioReabertura);
    }

    private static async Task<NaoConformidade> AdicionarNcAsync(
        ApplicationDbContext context,
        StatusNaoConformidade status,
        bool aprovadaRt = false,
        bool aprovadaGq = false,
        bool? eficaz = null)
    {
        var nc = new NaoConformidade
        {
            Codigo = "NC-2026-000001",
            DataAbertura = new DateOnly(2026, 9, 24),
            Area = "Qualidade",
            UsuarioAbertura = "gq",
            Status = status,
            AprovadaRt = aprovadaRt,
            AprovadaGq = aprovadaGq,
            Eficaz = eficaz
        };
        context.NaoConformidades.Add(nc);
        await context.SaveChangesAsync();
        return nc;
    }

    private static NaoConformidadesController CriarController(ApplicationDbContext context, string usuario, string perfil)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, usuario), new Claim(ClaimTypes.Role, perfil)], "Teste"))
        };
        return new NaoConformidadesController(context, new PrazoServiceFalso())
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new MemoriaTempDataProvider())
        };
    }

    private static ApplicationDbContext CriarContexto() => new(
        new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
        new HttpContextAccessor());

    private sealed class PrazoServiceFalso : IPrazoService
    {
        public Task<DateOnly> AdicionarDiasUteisAsync(DateOnly dataInicial, int diasUteis, CancellationToken cancellationToken = default) => Task.FromResult(dataInicial);
        public Task<DateOnly> CalcularPrazoReclamacaoAsync(DateOnly dataInformacoesCompletas, CancellationToken cancellationToken = default) => Task.FromResult(dataInformacoesCompletas);
        public Task<DateOnly?> CalcularPrazoNaoConformidadeAsync(DateOnly dataAbertura, ClassificacaoOcorrencia classificacao, CancellationToken cancellationToken = default) => Task.FromResult<DateOnly?>(dataAbertura);
    }

    private sealed class MemoriaTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}
