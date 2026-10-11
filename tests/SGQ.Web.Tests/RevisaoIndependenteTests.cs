using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SGQ.Web.Controllers;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Presentation;

namespace SGQ.Web.Tests;

// Cobre os achados verificados da revisão independente: justificativa de anulação, decimais e células de planilha.
public class RevisaoIndependenteTests
{
    [Theory]
    [InlineData("curta", false)]
    [InlineData("         ", false)]
    [InlineData("Arquivo enviado no processo errado.", true)]
    public async Task Anular_ExigeJustificativaDeDezAMilCaracteres(string justificativa, bool deveAnular)
    {
        await using var context = CriarContexto();
        var anexo = new Anexo { NomeOriginal = "laudo.pdf", NomeArmazenado = "x.pdf", TipoConteudo = "application/pdf", TamanhoBytes = 10, Usuario = "gq", EnviadoEm = DateTimeOffset.UtcNow };
        context.Anexos.Add(anexo);
        await context.SaveChangesAsync();
        var controller = CriarAnexos(context);

        await controller.Anular(anexo.Id, "Reclamacoes", 1, justificativa);

        var salvo = await context.Anexos.FindAsync(anexo.Id);
        Assert.Equal(!deveAnular, salvo!.Ativo);
        if (deveAnular) Assert.Equal(justificativa.Trim(), salvo.JustificativaAnulacao);
    }

    [Fact]
    public async Task Anular_JustificativaAcimaDoLimite_NaoAnula()
    {
        await using var context = CriarContexto();
        var anexo = new Anexo { NomeOriginal = "laudo.pdf", NomeArmazenado = "x.pdf", TipoConteudo = "application/pdf", TamanhoBytes = 10, Usuario = "gq", EnviadoEm = DateTimeOffset.UtcNow };
        context.Anexos.Add(anexo);
        await context.SaveChangesAsync();

        await CriarAnexos(context).Anular(anexo.Id, "Reclamacoes", 1, new string('x', 1001));

        Assert.True((await context.Anexos.FindAsync(anexo.Id))!.Ativo);
    }

    [Theory]
    [InlineData("", true, false)]
    [InlineData("", false, true)]
    [InlineData("150,5", true, false)]
    public async Task Binder_VazioEmDecimalNaoNuloGeraErro_EmNuloGeraNulo(string valor, bool anulavel, bool esperaErro)
    {
        var provedor = new EmptyModelMetadataProvider();
        var metadata = provedor.GetMetadataForType(anulavel ? typeof(decimal?) : typeof(decimal));
        var estado = new ModelStateDictionary();
        var contexto = new DefaultModelBindingContext
        {
            ModelName = "q",
            ModelMetadata = metadata,
            ModelState = estado,
            ValueProvider = new QueryStringValueProvider(BindingSource.Form, new Microsoft.AspNetCore.Http.QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { ["q"] = valor }), System.Globalization.CultureInfo.InvariantCulture)
        };

        await new DecimalFlexivelBinder().BindModelAsync(contexto);

        Assert.Equal(esperaErro, estado.ErrorCount > 0);
        if (valor == "150,5") Assert.Equal(150.5m, contexto.Result.Model);
        if (valor == "" && anulavel) Assert.True(contexto.Result.IsModelSet && contexto.Result.Model is null);
    }

    [Fact]
    public void ClosedXml_TextoIniciadoPorIgualNaoViraFormula()
    {
        using var planilha = new XLWorkbook();
        var celula = planilha.AddWorksheet("T").Cell(1, 1);

        celula.Value = "=HYPERLINK(\"http://x\",\"x\")";

        // Se a biblioteca tratasse como fórmula, a exportação precisaria neutralizar o texto como no CSV.
        Assert.False(celula.HasFormula);
        Assert.Equal(XLDataType.Text, celula.DataType);
    }

    private static AnexosController CriarAnexos(ApplicationDbContext context)
    {
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "gq"), new Claim(ClaimTypes.Role, "GQ")], "Teste")) };
        return new AnexosController(context, new Ambiente())
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new Memoria())
        };
    }

    private static ApplicationDbContext CriarContexto() => new(
        new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
        new HttpContextAccessor());

    private sealed class Memoria : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }

    private sealed class Ambiente : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "t";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = Path.GetTempPath();
        public string EnvironmentName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
