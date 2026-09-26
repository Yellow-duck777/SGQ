using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SGQ.Web.Controllers;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Services;

namespace SGQ.Web.Tests;

public class FluxosProcessosControllerTests
{
    [Fact]
    public async Task Validar_ReclamacaoElegivel_CriaNaoConformidadeVinculadaEIniciaInvestigacao()
    {
        await using var context = CriarContexto();
        var reclamacao = new ReclamacaoCliente
        {
            Codigo = "RC-2026-000001", Ano = 2026, SequenciaAnual = 1,
            DataRecebimento = new DateOnly(2026, 9, 22), ClienteId = 1, ProdutoId = 1,
            CanalRecebimento = "E-mail", ContatoCliente = "Contato", Descricao = "Descrição de teste",
            UsuarioAbertura = "gq", Status = StatusReclamacao.AguardandoValidacaoGq
        };
        context.ReclamacoesClientes.Add(reclamacao);
        await context.SaveChangesAsync();
        var controller = CriarController(new ReclamacoesController(context, new PrazoService(context)));

        var resultado = await controller.Validar(reclamacao.Id, ClassificacaoOcorrencia.Maior);

        Assert.IsType<RedirectToActionResult>(resultado);
        var rcAtualizada = await context.ReclamacoesClientes.SingleAsync();
        var nc = await context.NaoConformidades.SingleAsync();
        Assert.Equal(StatusReclamacao.EmInvestigacao, rcAtualizada.Status);
        Assert.Equal(rcAtualizada.Id, nc.ReclamacaoClienteId);
        Assert.Equal(OrigemNaoConformidade.ReclamacaoCliente, nc.Origem);
        Assert.Equal(ClassificacaoOcorrencia.Maior, nc.Classificacao);
    }

    [Fact]
    public async Task Encerrar_NaoConformidadeSemAprovacoes_ConservaStatus()
    {
        await using var context = CriarContexto();
        var nc = new NaoConformidade
        {
            Codigo = "NC-2026-000001", DataAbertura = new DateOnly(2026, 9, 22),
            Area = "Qualidade", Descricao = "Descrição de teste", UsuarioAbertura = "gq",
            Status = StatusNaoConformidade.AguardandoAprovacao, AprovadaRt = false, AprovadaGq = false
        };
        context.NaoConformidades.Add(nc);
        await context.SaveChangesAsync();
        var controller = CriarController(new NaoConformidadesController(context, new PrazoService(context)));

        var resultado = await controller.Encerrar(nc.Id);

        Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal(StatusNaoConformidade.AguardandoAprovacao, (await context.NaoConformidades.SingleAsync()).Status);
    }

    [Fact]
    public async Task Encerrar_RecallSemComunicacaoRegulatoria_ConservaStatus()
    {
        await using var context = CriarContexto();
        var recall = new Recall
        {
            Codigo = "REC-2026-000001", DataAbertura = new DateOnly(2026, 9, 22),
            ProdutoId = 1, LoteId = 1, UsuarioAbertura = "gq", Status = StatusRecall.AguardandoEncerramento,
            AprovadaRt = true, AprovadaGq = true, Destinacao = "Descarte", EvidenciaDestinacao = "Registro"
        };
        context.Recalls.Add(recall);
        await context.SaveChangesAsync();
        var controller = CriarController(new RecallsController(context));

        var resultado = await controller.Encerrar(recall.Id);

        Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal(StatusRecall.AguardandoEncerramento, (await context.Recalls.SingleAsync()).Status);
    }

    private static T CriarController<T>(T controller) where T : Controller
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, "gq"), new Claim(ClaimTypes.Role, "GQ")], "Teste"))
        };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        controller.TempData = new TempDataDictionary(httpContext, new MemoriaTempDataProvider());
        return controller;
    }

    private static ApplicationDbContext CriarContexto() => new(
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options,
        new HttpContextAccessor());

    private sealed class MemoriaTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}
