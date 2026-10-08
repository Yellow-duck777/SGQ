using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SGQ.Domain.Entities;
using SGQ.Web.Data;
using SGQ.Web.Models;

namespace SGQ.IntegrationTests;

// Comportamentos que o provedor InMemory dos testes unitários não reproduz: migrations, índices únicos,
// CHECK constraints e auditoria gravada por um SaveChanges real.
public class BancoPostgresTests : IClassFixture<SgqWebFactory>
{
    private readonly SgqWebFactory _fabrica;

    public BancoPostgresTests(SgqWebFactory fabrica) => _fabrica = fabrica;

    private AsyncServiceScope NovoEscopo() => _fabrica.Services.CreateAsyncScope();

    [FatoPostgres]
    public async Task Migrations_AplicadasEmBancoVazio_SemPendenciasDeModelo()
    {
        await using var escopo = NovoEscopo();
        var banco = escopo.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database;

        Assert.Empty(await banco.GetPendingMigrationsAsync());
        Assert.False(banco.HasPendingModelChanges(), "O modelo atual difere do snapshot: gere uma migration.");
    }

    [FatoPostgres]
    public async Task CodigoAnualDeNc_NaoPodeSerDuplicado()
    {
        await using var escopo = NovoEscopo();
        var contexto = escopo.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        contexto.NaoConformidades.Add(NovaNc("NC-2026-900001", 2026, 900001));
        await contexto.SaveChangesAsync();

        contexto.NaoConformidades.Add(NovaNc("NC-2026-900002", 2026, 900001));
        var erro = await Assert.ThrowsAsync<DbUpdateException>(() => contexto.SaveChangesAsync());

        Assert.Equal(PostgresErrorCodes.UniqueViolation, ((PostgresException)erro.InnerException!).SqlState);
    }

    [FatoPostgres]
    public async Task VinculoDeAnexo_ExigeExatamenteUmProcesso()
    {
        await using var escopo = NovoEscopo();
        var contexto = escopo.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        contexto.AnexosProcessosVinculos.Add(new AnexoProcessoVinculo { AnexoId = 1, ReclamacaoClienteId = 1, NaoConformidadeId = 1 });

        var erro = await Assert.ThrowsAsync<DbUpdateException>(() => contexto.SaveChangesAsync());

        Assert.Equal(PostgresErrorCodes.CheckViolation, ((PostgresException)erro.InnerException!).SqlState);
    }

    [FatoPostgres]
    public async Task CriarUsuario_NaoGravaHashNemCarimbosNaAuditoria()
    {
        await using var escopo = NovoEscopo();
        var usuarios = escopo.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var usuario = new ApplicationUser { UserName = "auditoria@sgq.test", Email = "auditoria@sgq.test" };

        var resultado = await usuarios.CreateAsync(usuario, "Senha-Forte-123!");
        Assert.True(resultado.Succeeded, string.Join("; ", resultado.Errors.Select(erro => erro.Description)));
        await usuarios.SetAuthenticationTokenAsync(usuario, "provedor", "nome", "TOKEN-SECRETO-DE-TESTE");
        await usuarios.UpdateSecurityStampAsync(usuario);

        var contexto = escopo.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var registros = await contexto.HistoricosAuditoria.Where(item => item.ChaveRegistro == usuario.Id).ToListAsync();
        var texto = string.Join("\n", registros.Select(item => item.Entidade + " " + item.Alteracoes));

        Assert.Contains(registros, item => item.Entidade == nameof(ApplicationUser));
        Assert.DoesNotContain(usuario.PasswordHash!, texto);
        Assert.DoesNotContain("TOKEN-SECRETO-DE-TESTE", texto);
        Assert.DoesNotContain("PasswordHash", texto);
        Assert.DoesNotContain("SecurityStamp", texto);
        Assert.DoesNotContain(registros, item => item.Entidade.StartsWith("IdentityUserToken", StringComparison.Ordinal));
    }

    [FatoPostgres]
    public async Task AtribuirPerfil_FicaRegistradoNaAuditoria()
    {
        await using var escopo = NovoEscopo();
        var usuarios = escopo.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var usuario = new ApplicationUser { UserName = "perfil@sgq.test", Email = "perfil@sgq.test" };
        Assert.True((await usuarios.CreateAsync(usuario, "Senha-Forte-123!")).Succeeded);

        Assert.True((await usuarios.AddToRoleAsync(usuario, SGQ.Web.Security.Roles.Auditor)).Succeeded);

        var contexto = escopo.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.True(await contexto.HistoricosAuditoria.AnyAsync(item => item.Entidade.StartsWith("IdentityUserRole") && item.Alteracoes!.Contains(usuario.Id)));
    }

    private static NaoConformidade NovaNc(string codigo, int ano, int sequencia) => new()
    {
        Codigo = codigo,
        Ano = ano,
        SequenciaAnual = sequencia,
        DataAbertura = new DateOnly(2026, 10, 8),
        Area = "Qualidade",
        UsuarioAbertura = "gq"
    };
}
