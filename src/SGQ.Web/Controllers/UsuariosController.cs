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
                Perfis = (await userManager.GetRolesAsync(user)).ToArray()
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
        if (remover.Length > 0) await userManager.RemoveFromRolesAsync(user, remover);
        if (adicionar.Length > 0) await userManager.AddToRolesAsync(user, adicionar);
        TempData["Success"] = $"Perfis de {user.Email} atualizados.";
        return RedirectToAction(nameof(Index));
    }
}
