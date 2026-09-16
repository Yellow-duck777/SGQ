using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;

namespace SGQ.Web.Security;

public sealed class DevelopmentTestUsersOptions
{
    public const string SectionName = "DevelopmentTestUsers";

    public bool Enabled { get; init; }
    public string ExpectedDatabase { get; init; } = string.Empty;
    public List<DevelopmentTestUserOptions> Users { get; init; } = [];
}

public sealed class DevelopmentTestUserOptions
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = [];
}

public static class DevelopmentTestUsers
{
    private const string BootstrapArgument = "--bootstrap-test-users";
    private const string RemoveArgument = "--remove-test-users";
    private const string TestEmailDomain = "@sgq.test";

    public static bool HasRequestedOperation(string[] args) =>
        args.Contains(BootstrapArgument, StringComparer.Ordinal) || args.Contains(RemoveArgument, StringComparer.Ordinal);

    public static async Task ExecuteAsync(
        string[] args,
        IServiceProvider services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        var bootstrap = args.Contains(BootstrapArgument, StringComparer.Ordinal);
        var remove = args.Contains(RemoveArgument, StringComparer.Ordinal);
        if (bootstrap == remove)
            throw new InvalidOperationException($"Informe exatamente uma operação: {BootstrapArgument} ou {RemoveArgument}.");

        if (!environment.IsDevelopment())
            throw new InvalidOperationException("Usuários de teste só podem ser gerenciados no ambiente Development.");

        var options = configuration.GetSection(DevelopmentTestUsersOptions.SectionName).Get<DevelopmentTestUsersOptions>()
            ?? new DevelopmentTestUsersOptions();
        ValidateOptions(options, bootstrap);

        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var database = context.Database.GetDbConnection().Database;
        if (!string.Equals(database, options.ExpectedDatabase, StringComparison.Ordinal) || !IsTestDatabase(database))
            throw new InvalidOperationException("A operação foi bloqueada: a conexão deve apontar exatamente para o banco de testes configurado (nome terminado em _test ou _tests).");

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in Roles.Todos)
            if (!await roleManager.RoleExistsAsync(role))
                EnsureSuccess(await roleManager.CreateAsync(new IdentityRole(role)), $"Não foi possível criar o perfil {role} no banco de testes");

        if (bootstrap)
            await BootstrapAsync(options.Users, userManager, roleManager);
        else
            await DisableAsync(options.Users, userManager);
    }

    private static async Task BootstrapAsync(
        IEnumerable<DevelopmentTestUserOptions> configuredUsers,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        foreach (var configuredUser in configuredUsers)
        {
            var email = configuredUser.Email.Trim();
            var expectedRoles = configuredUser.Roles.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
            foreach (var role in expectedRoles)
                if (!await roleManager.RoleExistsAsync(role))
                    throw new InvalidOperationException($"O perfil configurado {role} não existe no banco de testes.");

            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser is not null)
            {
                var currentRoles = (await userManager.GetRolesAsync(existingUser)).Order(StringComparer.Ordinal).ToArray();
                if (!currentRoles.SequenceEqual(expectedRoles, StringComparer.Ordinal))
                    throw new InvalidOperationException($"O usuário de teste {email} já existe com perfis diferentes. Remova-o explicitamente antes de recriá-lo.");

                continue;
            }

            var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            EnsureSuccess(await userManager.CreateAsync(user, configuredUser.Password), $"Não foi possível criar o usuário de teste {email}");
            EnsureSuccess(await userManager.AddToRolesAsync(user, expectedRoles), $"Não foi possível atribuir perfis ao usuário de teste {email}");
        }
    }

    private static async Task DisableAsync(IEnumerable<DevelopmentTestUserOptions> configuredUsers, UserManager<ApplicationUser> userManager)
    {
        foreach (var configuredUser in configuredUsers)
        {
            var email = configuredUser.Email.Trim();
            var user = await userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                EnsureSuccess(await userManager.SetLockoutEnabledAsync(user, true), $"Não foi possível habilitar o bloqueio do usuário de teste {email}");
                EnsureSuccess(await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue), $"Não foi possível bloquear o usuário de teste {email}");
                EnsureSuccess(await userManager.UpdateSecurityStampAsync(user), $"Não foi possível invalidar sessões do usuário de teste {email}");
            }
        }
    }

    private static void ValidateOptions(DevelopmentTestUsersOptions options, bool requiresPasswords)
    {
        if (!options.Enabled)
            throw new InvalidOperationException("Defina DevelopmentTestUsers:Enabled como true nos User Secrets para permitir a operação.");
        if (string.IsNullOrWhiteSpace(options.ExpectedDatabase))
            throw new InvalidOperationException("Defina DevelopmentTestUsers:ExpectedDatabase nos User Secrets.");
        if (options.Users.Count == 0)
            throw new InvalidOperationException("Configure ao menos um usuário de teste nos User Secrets.");

        var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var user in options.Users)
        {
            var email = user.Email.Trim();
            if (!email.EndsWith(TestEmailDomain, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"O e-mail {email} não pertence ao domínio reservado de testes {TestEmailDomain}.");
            if (!emails.Add(email))
                throw new InvalidOperationException($"O e-mail de teste {email} foi informado mais de uma vez.");
            if (requiresPasswords && string.IsNullOrWhiteSpace(user.Password))
                throw new InvalidOperationException($"Defina uma senha para o usuário de teste {email} nos User Secrets.");
            if (user.Roles.Count == 0 || user.Roles.Any(role => !Roles.Todos.Contains(role, StringComparer.Ordinal)))
                throw new InvalidOperationException($"O usuário de teste {email} contém perfil não suportado.");
        }
    }

    private static bool IsTestDatabase(string database) =>
        database.EndsWith("_test", StringComparison.OrdinalIgnoreCase) ||
        database.EndsWith("_tests", StringComparison.OrdinalIgnoreCase);

    private static void EnsureSuccess(IdentityResult result, string message)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException($"{message}: {string.Join("; ", result.Errors.Select(error => error.Description))}");
    }
}
