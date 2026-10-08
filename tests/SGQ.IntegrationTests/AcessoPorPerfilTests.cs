using System.Net;
using SGQ.Web.Security;

namespace SGQ.IntegrationTests;

// Exercita o pipeline real (política global de acesso, [Authorize], roteamento) contra PostgreSQL.
// Antes desta classe a regra "conta sem perfil recebe 403" não tinha teste automatizado.
public class AcessoPorPerfilTests : IClassFixture<SgqWebFactory>
{
    private readonly SgqWebFactory _fabrica;

    public AcessoPorPerfilTests(SgqWebFactory fabrica) => _fabrica = fabrica;

    [TeoriaPostgres]
    [InlineData("/Reclamacoes")]
    [InlineData("/NaoConformidades")]
    [InlineData("/Recalls")]
    [InlineData("/Clientes")]
    [InlineData("/Relatorios")]
    public async Task Anonimo_RotasDeNegocio_ExigemAutenticacao(string rota)
    {
        var resposta = await _fabrica.CriarCliente().GetAsync(rota);

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [TeoriaPostgres]
    [InlineData("/")]
    [InlineData("/Reclamacoes")]
    [InlineData("/NaoConformidades")]
    [InlineData("/Recalls")]
    [InlineData("/Clientes")]
    [InlineData("/Produtos")]
    [InlineData("/Lotes")]
    [InlineData("/Relatorios")]
    [InlineData("/DiasNaoUteis")]
    [InlineData("/Home/Privacy")]
    public async Task AutenticadoSemPerfil_RecebeAcessoNegado(string rota)
    {
        var resposta = await _fabrica.CriarCliente("conta-sem-perfil").GetAsync(rota);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [TeoriaPostgres]
    [InlineData(Roles.Administrador)]
    [InlineData(Roles.GarantiaQualidade)]
    [InlineData(Roles.ResponsavelTecnico)]
    [InlineData(Roles.ControleQualidade)]
    [InlineData(Roles.Auditor)]
    public async Task QualquerPerfilReconhecido_AcessaListagensEDashboard(string perfil)
    {
        var cliente = _fabrica.CriarCliente("usuario-" + perfil, perfil);

        foreach (var rota in new[] { "/", "/Reclamacoes", "/NaoConformidades", "/Recalls" })
        {
            var resposta = await cliente.GetAsync(rota);
            Assert.True(resposta.StatusCode == HttpStatusCode.OK, $"{perfil} em {rota}: {(int)resposta.StatusCode}");
        }
    }

    [TeoriaPostgres]
    [InlineData(Roles.GarantiaQualidade, HttpStatusCode.Forbidden)]
    [InlineData(Roles.ControleQualidade, HttpStatusCode.Forbidden)]
    [InlineData(Roles.Administrador, HttpStatusCode.OK)]
    public async Task Usuarios_SoAdministrador(string perfil, HttpStatusCode esperado)
    {
        var resposta = await _fabrica.CriarCliente("usuario-" + perfil, perfil).GetAsync("/Usuarios");

        Assert.Equal(esperado, resposta.StatusCode);
    }

    [FatoPostgres]
    public async Task PaginasPublicas_ContinuamAcessiveisSemLogin()
    {
        var cliente = _fabrica.CriarCliente();

        Assert.Equal(HttpStatusCode.OK, (await cliente.GetAsync("/css/site.css")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await cliente.GetAsync("/Identity/Account/Login")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await cliente.GetAsync("/Home/Error")).StatusCode);
    }

    [FatoPostgres]
    public async Task ContaSemPerfil_AindaVePaginaDeErro()
    {
        var resposta = await _fabrica.CriarCliente("conta-sem-perfil").GetAsync("/Home/Error");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [FatoPostgres]
    public async Task AvancarStatus_NaoExisteMais()
    {
        var resposta = await _fabrica.CriarCliente("gq", Roles.GarantiaQualidade)
            .PostAsync("/Reclamacoes/AvancarStatus/1", new FormUrlEncodedContent([]));

        // A ação foi removida: a rota não executa nada (404 por rota inexistente ou 405 por método sem ação correspondente).
        Assert.True(resposta.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.MethodNotAllowed, $"Status inesperado: {(int)resposta.StatusCode}");
    }
}
