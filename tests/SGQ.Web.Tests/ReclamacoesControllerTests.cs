using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Controllers;
using SGQ.Domain.Entities;
using SGQ.Domain.Enums;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Services;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Tests;

public class ReclamacoesControllerTests
{
    [Fact]
    public async Task RegistrarResultadoLaboratorio_SemLaudoCritico_MantemAguardandoLaboratorio()
    {
        await using var context = CriarContexto();
        var reclamacao = await AdicionarReclamacaoAsync(context, StatusReclamacao.AguardandoLaboratorioExterno);
        var controller = CriarController(context);

        var resultado = await controller.RegistrarResultadoLaboratorio(new ResultadoLaboratorioExternoViewModel
        {
            Id = reclamacao.Id,
            DataRecebimento = new DateOnly(2026, 9, 24),
            IdentificacaoLaudo = "LAUDO-001",
            Resultado = "Resultado laboratorial"
        });

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizada = await context.ReclamacoesClientes.FindAsync(reclamacao.Id);
        Assert.Equal(StatusReclamacao.AguardandoLaboratorioExterno, atualizada!.Status);
        Assert.Null(atualizada.ResultadoLaboratorio);
    }

    [Fact]
    public async Task RegistrarResultadoLaboratorio_ComLaudoCritico_ArmazenaResultadoERetornaParaInvestigacao()
    {
        await using var context = CriarContexto();
        var reclamacao = await AdicionarReclamacaoAsync(context, StatusReclamacao.AguardandoLaboratorioExterno);
        var anexo = new Anexo
        {
            ReclamacaoClienteId = reclamacao.Id,
            NomeOriginal = "laudo.pdf",
            NomeArmazenado = "laudo.pdf",
            TipoConteudo = "application/pdf",
            TamanhoBytes = 10,
            Critico = true,
            Usuario = "gq",
            EnviadoEm = DateTimeOffset.UtcNow
        };
        context.Anexos.Add(anexo);
        await context.SaveChangesAsync();
        reclamacao.LaudoLaboratorioAnexoId = anexo.Id;
        await context.SaveChangesAsync();
        var controller = CriarController(context);

        await controller.RegistrarResultadoLaboratorio(new ResultadoLaboratorioExternoViewModel
        {
            Id = reclamacao.Id,
            DataRecebimento = new DateOnly(2026, 9, 24),
            IdentificacaoLaudo = "LAUDO-001",
            Resultado = "Dentro da especificação"
        });

        var atualizada = await context.ReclamacoesClientes.FindAsync(reclamacao.Id);
        Assert.Equal(StatusReclamacao.EmInvestigacao, atualizada!.Status);
        Assert.Equal("LAUDO-001", atualizada.IdentificacaoLaudoLaboratorio);
        Assert.Equal("Dentro da especificação", atualizada.ResultadoLaboratorio);
    }

    [Fact]
    public async Task Reabrir_ReclamacaoEncerrada_RetornaParaInvestigacaoEPreservaHistorico()
    {
        await using var context = CriarContexto();
        var reclamacao = await AdicionarReclamacaoAsync(context, StatusReclamacao.Encerrada);
        var controller = CriarController(context);

        var resultado = await controller.Reabrir(new ReclamacaoReaberturaViewModel
        {
            Id = reclamacao.Id,
            Justificativa = "Nova evidência exige reavaliação da reclamação."
        });

        Assert.IsType<RedirectToActionResult>(resultado);
        var atualizada = await context.ReclamacoesClientes.FindAsync(reclamacao.Id);
        Assert.Equal(StatusReclamacao.Encerrada, atualizada!.StatusAnteriorReabertura);
        Assert.Equal(StatusReclamacao.EmInvestigacao, atualizada.Status);
        Assert.Equal("gq", atualizada.UsuarioReabertura);
        Assert.NotNull(atualizada.ReabertaEm);
    }

    [Fact]
    public async Task Index_ExpoeContagemPorSituacaoSemAplicarFiltros()
    {
        await using var context = CriarContexto();
        context.Clientes.Add(new Cliente { Id = 1, Nome = "Cliente teste", Contato = "contato" });
        context.Produtos.Add(new Produto { Id = 1, Nome = "Produto teste" });
        await context.SaveChangesAsync();
        await AdicionarReclamacaoAsync(context, StatusReclamacao.EmInvestigacao);
        await AdicionarReclamacaoAsync(context, StatusReclamacao.EmInvestigacao);
        await AdicionarReclamacaoAsync(context, StatusReclamacao.Encerrada);
        var controller = CriarController(context);

        var resultado = await controller.Index(null, StatusReclamacao.Encerrada, null, null, null, null, null);

        var view = Assert.IsType<ViewResult>(resultado);
        Assert.Single((IEnumerable<ReclamacaoCliente>)view.Model!);
        var contagens = Assert.IsType<Dictionary<StatusReclamacao, int>>(controller.ViewBag.ContagensStatus);
        Assert.Equal(2, contagens[StatusReclamacao.EmInvestigacao]);
        Assert.Equal(1, contagens[StatusReclamacao.Encerrada]);
    }

    private static async Task<ReclamacaoCliente> AdicionarReclamacaoAsync(ApplicationDbContext context, StatusReclamacao status)
    {
        var reclamacao = new ReclamacaoCliente
        {
            Codigo = "RC-2026-000001",
            DataRecebimento = new DateOnly(2026, 9, 24),
            CanalRecebimento = "E-mail",
            ClienteId = 1,
            ContatoCliente = "Contato",
            ProdutoId = 1,
            Descricao = "Descrição de teste",
            UsuarioAbertura = "gq",
            Status = status
        };
        context.ReclamacoesClientes.Add(reclamacao);
        await context.SaveChangesAsync();
        return reclamacao;
    }

    private static ReclamacoesController CriarController(ApplicationDbContext context)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, "gq"), new Claim(ClaimTypes.Role, "GQ")], "Teste"))
        };
        return new ReclamacoesController(context, new PrazoServiceFalso())
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
