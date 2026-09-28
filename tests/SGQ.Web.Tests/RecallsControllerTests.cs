using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Controllers;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Tests;

public class RecallsControllerTests
{
    [Fact]
    public async Task Aprovar_QuandoRtEGqAprovam_EncaminhaParaRecolhimento()
    {
        await using var context = CriarContexto();
        var recall = await AdicionarRecallAsync(context, StatusRecall.AguardandoAprovacao);

        await CriarController(context, "rt", "RT").Aprovar(recall.Id, "RT");
        var resultado = await CriarController(context, "gq", "GQ").Aprovar(recall.Id, "GQ");

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizado = await context.Recalls.FindAsync(recall.Id);
        Assert.True(atualizado!.AprovadaRt);
        Assert.True(atualizado.AprovadaGq);
        Assert.Equal(StatusRecall.EmRecolhimento, atualizado.Status);
    }

    [Fact]
    public async Task RegistrarOperacao_ComRegistrosObrigatorios_AguardaRetornos()
    {
        await using var context = CriarContexto();
        var recall = await AdicionarRecallAsync(context, StatusRecall.EmRecolhimento);
        var controller = CriarController(context, "gq", "GQ");

        var resultado = await controller.RegistrarOperacao(recall.Id, bloqueioRegistrado: true, "Comunicado enviado", "Anvisa", "PROTO-001");

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizado = await context.Recalls.FindAsync(recall.Id);
        Assert.True(atualizado!.BloqueioRegistrado);
        Assert.Equal("Anvisa", atualizado.AutoridadeSanitaria);
        Assert.NotNull(atualizado.ComunicadaAutoridadeEm);
        Assert.Equal(StatusRecall.AguardandoRetorno, atualizado.Status);
    }

    [Fact]
    public async Task DecidirDivergencia_SemJustificativaSuficiente_MantemAguardandoDecisaoDoCq()
    {
        await using var context = CriarContexto();
        var recall = await AdicionarRecallAsync(context, StatusRecall.AguardandoDecisaoCq);
        var controller = CriarController(context, "cq", "CQ");

        var resultado = await controller.DecidirDivergencia(recall.Id, favoravel: true, "Curta");

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizado = await context.Recalls.FindAsync(recall.Id);
        Assert.Equal(StatusRecall.AguardandoDecisaoCq, atualizado!.Status);
        Assert.Null(atualizado.DecisaoCq);
        Assert.Null(atualizado.DecididaPeloCqEm);
    }

    [Fact]
    public async Task Reabrir_RecallEncerrado_ResetaAprovacoesERetornaParaAvaliacao()
    {
        await using var context = CriarContexto();
        var recall = await AdicionarRecallAsync(context, StatusRecall.Encerrado, aprovadaRt: true, aprovadaGq: true);
        var controller = CriarController(context, "rt", "RT");

        var resultado = await controller.Reabrir(new RecallReaberturaViewModel
        {
            Id = recall.Id,
            Motivo = "Nova ocorrência identificada",
            Justificativa = "A rastreabilidade exige uma nova avaliação técnica."
        });

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizado = await context.Recalls.FindAsync(recall.Id);
        Assert.Equal(StatusRecall.Encerrado, atualizado!.StatusAnteriorReabertura);
        Assert.Equal(StatusRecall.EmAvaliacao, atualizado.Status);
        Assert.False(atualizado.AprovadaRt);
        Assert.False(atualizado.AprovadaGq);
        Assert.Equal("rt", atualizado.UsuarioReabertura);
    }

    private static async Task<Recall> AdicionarRecallAsync(ApplicationDbContext context, StatusRecall status, bool aprovadaRt = false, bool aprovadaGq = false)
    {
        var recall = new Recall
        {
            Codigo = "REC-2026-000001",
            DataAbertura = new DateOnly(2026, 9, 24),
            ProdutoId = 1,
            LoteId = 1,
            ClientesEnvolvidos = "Cliente de teste",
            NaturezaOcorrencia = "Ocorrência de teste",
            RiscoPotencial = "Risco de teste",
            JustificativaDecisao = "Justificativa de teste",
            UsuarioAbertura = "gq",
            Status = status,
            AprovadaRt = aprovadaRt,
            AprovadaGq = aprovadaGq
        };
        context.Recalls.Add(recall);
        await context.SaveChangesAsync();
        return recall;
    }

    private static RecallsController CriarController(ApplicationDbContext context, string usuario, string perfil)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, usuario), new Claim(ClaimTypes.Role, perfil)], "Teste"))
        };
        return new RecallsController(context)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new MemoriaTempDataProvider())
        };
    }

    private static ApplicationDbContext CriarContexto() => new(
        new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
        new HttpContextAccessor());

    private sealed class MemoriaTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}
