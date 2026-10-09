using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Models;
using SGQ.Web.Security;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class UsuariosController(UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users.OrderBy(user => user.Email).ToListAsync();
        var model = new List<UsuarioPerfilViewModel>();
        foreach (var user in users)
            model.Add(new UsuarioPerfilViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                NomeUsuario = user.UserName ?? string.Empty,
                Perfis = (await userManager.GetRolesAsync(user)).ToArray(),
                Bloqueado = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow
            });
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarPerfis(string id, string[]? perfis)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        var desejados = (perfis ?? []).Where(Roles.Todos.Contains).Distinct().ToArray();
        var atuais = await userManager.GetRolesAsync(user);
        var remover = atuais.Except(desejados).ToArray();
        var adicionar = desejados.Except(atuais).ToArray();

        // Salvaguarda: o sistema nunca pode ficar sem um Administrador (ninguém mais conseguiria atribuir perfis).
        if (remover.Contains(Roles.Administrador) && (await userManager.GetUsersInRoleAsync(Roles.Administrador)).Count <= 1)
        {
            TempData["Error"] = $"Não é possível remover o perfil Administrador de {user.Email}: é o único Administrador do sistema. Atribua o perfil a outra pessoa antes.";
            return RedirectToAction(nameof(Index));
        }

        if (remover.Length > 0) await userManager.RemoveFromRolesAsync(user, remover);
        if (adicionar.Length > 0) await userManager.AddToRolesAsync(user, adicionar);
        TempData["Success"] = desejados.Length == 0
            ? $"Perfis de {user.Email} removidos. A conta ficará bloqueada até que um perfil seja atribuído."
            : $"Perfis de {user.Email} atualizados.";
        return RedirectToAction(nameof(Index));
    }
}
