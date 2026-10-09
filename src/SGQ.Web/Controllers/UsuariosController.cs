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

        if (remover.Length > 0)
        {
            var remocao = await userManager.RemoveFromRolesAsync(user, remover);
            if (!remocao.Succeeded) return FalhaAoAtualizarPerfis(user, remocao);
        }
        if (adicionar.Length > 0)
        {
            var inclusao = await userManager.AddToRolesAsync(user, adicionar);
            if (!inclusao.Succeeded) return FalhaAoAtualizarPerfis(user, inclusao);
        }

        // Verificação final contra corrida: dois Administradores removendo o perfil um do outro ao mesmo tempo
        // passariam juntos pela checagem acima. Se o sistema ficou sem Administrador, devolve o perfil a esta conta.
        if (remover.Contains(Roles.Administrador) && (await userManager.GetUsersInRoleAsync(Roles.Administrador)).Count == 0)
        {
            await userManager.AddToRoleAsync(user, Roles.Administrador);
            TempData["Error"] = $"O perfil Administrador de {user.Email} foi mantido: o sistema não pode ficar sem Administrador.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = desejados.Length == 0
            ? $"Perfis de {user.Email} removidos. A conta ficará bloqueada até que um perfil seja atribuído."
            : $"Perfis de {user.Email} atualizados.";
        return RedirectToAction(nameof(Index));
    }

    private IActionResult FalhaAoAtualizarPerfis(ApplicationUser user, IdentityResult resultado)
    {
        TempData["Error"] = $"Não foi possível atualizar os perfis de {user.Email}: {string.Join("; ", resultado.Errors.Select(erro => erro.Description))}";
        return RedirectToAction(nameof(Index));
    }
}
