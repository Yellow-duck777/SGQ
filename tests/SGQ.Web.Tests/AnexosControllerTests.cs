using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SGQ.Web.Controllers;
using SGQ.Web.Data;
using SGQ.Web.Models;

namespace SGQ.Web.Tests;

public class AnexosControllerTests
{
    [Fact]
    public async Task Enviar_EvidenciaCriticaComum_NaoExigeFluxoDeLaboratorio()
    {
        await using var context = CriarContexto();
        var reclamacao = new ReclamacaoCliente
        {
            Codigo = "RC-2026-000001", DataRecebimento = new DateOnly(2026, 9, 22),
            CanalRecebimento = "E-mail", ContatoCliente = "Contato", ClienteId = 1,
            ProdutoId = 1, Descricao = "Teste", UsuarioAbertura = "gq"
        };
        context.ReclamacoesClientes.Add(reclamacao);
        await context.SaveChangesAsync();

        var contentRoot = Path.Combine(Path.GetTempPath(), $"sgq-tests-{Guid.NewGuid():N}");
        var environment = new AmbienteWebTeste { ContentRootPath = contentRoot };
        try
        {
            var controller = CriarController(context, environment);
            await using var conteudo = new MemoryStream([1, 2, 3]);
            var arquivo = new FormFile(conteudo, 0, conteudo.Length, "arquivo", "evidencia.pdf") { Headers = new HeaderDictionary(), ContentType = "application/pdf" };

            var resultado = await controller.Enviar(arquivo, "Reclamacoes", reclamacao.Id, null, critico: true);

            Assert.IsType<RedirectToActionResult>(resultado);
            var anexo = await context.Anexos.SingleAsync();
            Assert.True(anexo.Critico);
            Assert.Null(reclamacao.LaudoLaboratorioAnexoId);
        }
        finally
        {
            if (Directory.Exists(contentRoot)) Directory.Delete(contentRoot, recursive: true);
        }
    }

    [Fact]
    public async Task Vincular_CriaVinculoParaProcessoDeDestino_EImpedeDuplicidade()
    {
        await using var context = CriarContexto();
        var reclamacao = new ReclamacaoCliente { Codigo = "RC-2026-000001", DataRecebimento = new DateOnly(2026, 9, 22), CanalRecebimento = "E-mail", ContatoCliente = "Contato", ClienteId = 1, ProdutoId = 1, Descricao = "Teste", UsuarioAbertura = "gq" };
        var naoConformidade = new NaoConformidade { Codigo = "NC-2026-000001", DataAbertura = new DateOnly(2026, 9, 22), Area = "Qualidade", UsuarioAbertura = "gq" };
        context.AddRange(reclamacao, naoConformidade);
        await context.SaveChangesAsync();
        var anexo = new Anexo { ReclamacaoClienteId = reclamacao.Id, NomeOriginal = "evidencia.pdf", NomeArmazenado = "arquivo.pdf", TipoConteudo = "application/pdf", TamanhoBytes = 100, Usuario = "gq", EnviadoEm = DateTimeOffset.UtcNow };
        context.Anexos.Add(anexo);
        await context.SaveChangesAsync();
        var controller = CriarController(context);

        var primeiroResultado = await controller.Vincular(anexo.Id, "Reclamacoes", reclamacao.Id, naoConformidade.Codigo);
        var segundoResultado = await controller.Vincular(anexo.Id, "Reclamacoes", reclamacao.Id, naoConformidade.Codigo);

        Assert.IsType<RedirectToActionResult>(primeiroResultado);
        Assert.IsType<RedirectToActionResult>(segundoResultado);
        Assert.Single(await context.AnexosProcessosVinculos.Where(item => item.AnexoId == anexo.Id && item.NaoConformidadeId == naoConformidade.Id).ToListAsync());
    }

    private static AnexosController CriarController(ApplicationDbContext context, IWebHostEnvironment? environment = null)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "gq"), new Claim(ClaimTypes.Role, "GQ")], "Teste"))
        };
        var controller = new AnexosController(context, environment ?? new AmbienteWebTeste())
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new MemoriaTempDataProvider())
        };
        return controller;
    }

    private static ApplicationDbContext CriarContexto() => new(
        new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
        new HttpContextAccessor());

    private sealed class MemoriaTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }

    private sealed class AmbienteWebTeste : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "SGQ.Web.Tests";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = Path.GetTempPath();
        public string EnvironmentName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
