using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace SGQ.IntegrationTests;

/// <summary>
/// Sobe a aplicação real contra um PostgreSQL de verdade, com um banco descartável por fixture.
/// A variável de ambiente <see cref="VariavelConexao"/> aponta para o servidor, sem o nome do banco,
/// por exemplo: Host=127.0.0.1;Port=5432;Username=postgres;Password=postgres
/// </summary>
public sealed class SgqWebFactory : WebApplicationFactory<Program>
{
    public const string VariavelConexao = "SGQ_TEST_PG";

    private readonly string _servidor = Environment.GetEnvironmentVariable(VariavelConexao)
        ?? throw new InvalidOperationException($"Defina {VariavelConexao} para executar os testes de integração.");
    private readonly string _banco = "sgq_it_" + Guid.NewGuid().ToString("N");

    public string ConnectionString => new NpgsqlConnectionStringBuilder(_servidor) { Database = _banco }.ConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        builder.ConfigureTestServices(services =>
            services.AddAuthentication(opcoes =>
            {
                opcoes.DefaultAuthenticateScheme = AutenticacaoDeTeste.Esquema;
                opcoes.DefaultChallengeScheme = AutenticacaoDeTeste.Esquema;
                opcoes.DefaultForbidScheme = AutenticacaoDeTeste.Esquema;
            }).AddScheme<AuthenticationSchemeOptions, AutenticacaoDeTeste>(AutenticacaoDeTeste.Esquema, _ => { }));
    }

    /// <summary>Cliente autenticado como o usuário e os perfis informados (sem perfis = conta sem acesso).</summary>
    public HttpClient CriarCliente(string? usuario = null, params string[] perfis)
    {
        var cliente = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        if (usuario is not null) cliente.DefaultRequestHeaders.Add(AutenticacaoDeTeste.CabecalhoUsuario, usuario);
        if (perfis.Length > 0) cliente.DefaultRequestHeaders.Add(AutenticacaoDeTeste.CabecalhoPerfis, string.Join(',', perfis));
        return cliente;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;
        try
        {
            NpgsqlConnection.ClearAllPools();
            using var conexao = new NpgsqlConnection(_servidor);
            conexao.Open();
            using var comando = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{_banco}\" WITH (FORCE)", conexao);
            comando.ExecuteNonQuery();
        }
        catch
        {
            // O banco é descartável; falha na limpeza não deve mascarar o resultado dos testes.
        }
    }
}

/// <summary>Autenticação de teste: usuário e perfis vêm de cabeçalhos; sem cabeçalho de usuário a requisição é anônima.</summary>
public sealed class AutenticacaoDeTeste(
    IOptionsMonitor<AuthenticationSchemeOptions> opcoes, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(opcoes, logger, encoder)
{
    public const string Esquema = "Teste";
    public const string CabecalhoUsuario = "X-Teste-Usuario";
    public const string CabecalhoPerfis = "X-Teste-Perfis";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(CabecalhoUsuario, out var usuario))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, usuario.ToString()), new(ClaimTypes.Name, usuario.ToString()) };
        if (Request.Headers.TryGetValue(CabecalhoPerfis, out var perfis))
            claims.AddRange(perfis.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(perfil => new Claim(ClaimTypes.Role, perfil)));

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, Esquema)), Esquema);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
