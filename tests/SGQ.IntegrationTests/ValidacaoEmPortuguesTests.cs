using System.Net;
using System.Text.RegularExpressions;
using SGQ.Web.Security;

namespace SGQ.IntegrationTests;

// Garante que nenhum formulário volta mensagens de validação em inglês, inclusive os "obrigatórios implícitos" do MVC.
public partial class ValidacaoEmPortuguesTests : IClassFixture<SgqWebFactory>
{
    private readonly SgqWebFactory _fabrica;

    public ValidacaoEmPortuguesTests(SgqWebFactory fabrica) => _fabrica = fabrica;

    [FatoPostgres]
    public async Task RecallCreate_FormularioVazio_RetornaMensagensEmPortugues()
    {
        var cliente = _fabrica.CriarCliente("gq", Roles.GarantiaQualidade);
        var token = await ObterTokenAsync(cliente, "/Recalls/Create");

        var resposta = await cliente.PostAsync("/Recalls/Create", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        }));

        var html = WebUtility.HtmlDecode(await resposta.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Contains("Este campo é obrigatório.", html);
        Assert.DoesNotContain("field is required", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("The value", html, StringComparison.OrdinalIgnoreCase);
    }

    [FatoPostgres]
    public async Task RcCreate_FormularioVazio_NaoTemMensagemEmIngles()
    {
        var cliente = _fabrica.CriarCliente("gq", Roles.GarantiaQualidade);
        var token = await ObterTokenAsync(cliente, "/Reclamacoes/Create");

        var resposta = await cliente.PostAsync("/Reclamacoes/Create", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        }));

        var html = WebUtility.HtmlDecode(await resposta.Content.ReadAsStringAsync());
        Assert.DoesNotContain("field is required", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("is not valid", html, StringComparison.OrdinalIgnoreCase);
    }

    [FatoPostgres]
    public async Task AtributosDeValidacaoDoNavegador_UsamMensagensEmPortugues()
    {
        var cliente = _fabrica.CriarCliente("gq", Roles.GarantiaQualidade);

        var html = WebUtility.HtmlDecode(await cliente.GetStringAsync("/Recalls/Create"));

        // O jquery.validate lê data-val-required; precisa estar em português no HTML entregue.
        Assert.Matches(AtributoObrigatorioEmPortugues(), html);
        Assert.DoesNotContain("data-val-required=\"The", html);
    }

    private static async Task<string> ObterTokenAsync(HttpClient cliente, string rota)
    {
        var html = await cliente.GetStringAsync(rota);
        var m = TokenAntiforgery().Match(html);
        Assert.True(m.Success, "Token antiforgery não encontrado na página.");
        return m.Groups[1].Value;
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex TokenAntiforgery();

    [GeneratedRegex("data-val-required=\"Este campo é obrigatório\\.\"")]
    private static partial Regex AtributoObrigatorioEmPortugues();
}
