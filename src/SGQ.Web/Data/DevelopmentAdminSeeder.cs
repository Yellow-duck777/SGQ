using Microsoft.AspNetCore.Identity;
using SGQ.Web.Models;

namespace SGQ.Web.Data;

public static class DevelopmentAdminSeeder
{
    private const string AdministratorRole = "Administrador";

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        await using var scope = services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(AdministratorRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(AdministratorRole));
            EnsureSucceeded(roleResult, "criar a função de administrador");
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            EnsureSucceeded(createResult, "criar o administrador de desenvolvimento");
        }

        if (!await userManager.IsInRoleAsync(user, AdministratorRole))
        {
            var roleResult = await userManager.AddToRoleAsync(user, AdministratorRole);
            EnsureSucceeded(roleResult, "atribuir a função de administrador");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"Não foi possível {operation}: {errors}");
    }
}
