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

    [HttpGet]
    public IActionResult Create() => View(new UsuarioCreateViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel model)
    {
        model.Email = model.Email.Trim();
        model.Perfis = PerfisReconhecidos(model.Perfis);
        if (model.Perfis.Length == 0) ModelState.AddModelError(nameof(model.Perfis), "Selecione ao menos um perfil.");
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
        var criacao = await userManager.CreateAsync(user, model.Password);
        if (!criacao.Succeeded)
        {
            AdicionarErros(criacao);
            return View(model);
        }

        var perfis = await userManager.AddToRolesAsync(user, model.Perfis);
        if (!perfis.Succeeded)
        {
            AdicionarErros(perfis);
            return View(model);
        }

        TempData["Success"] = $"Conta de {user.Email} criada com os perfis selecionados.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarPerfis(string id, string[]? perfis)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        var desejados = PerfisReconhecidos(perfis);
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

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RedefinirSenha(RedefinirSenhaUsuarioViewModel model)
    {
        if (!ModelState.IsValid) return RedirecionarComErroDeSenha(model.Id, "Preencha a nova senha e a confirmação corretamente.");

        var user = await userManager.FindByIdAsync(model.Id);
        if (user is null) return NotFound();
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resultado = await userManager.ResetPasswordAsync(user, token, model.Password);
        if (!resultado.Succeeded) return FalhaAoRedefinirSenha(user, resultado);

        TempData["Success"] = $"Senha de {user.Email} redefinida.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AlternarBloqueio(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        var bloqueado = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;
        if (bloqueado)
        {
            var desbloqueio = await userManager.SetLockoutEndDateAsync(user, null);
            if (!desbloqueio.Succeeded) return FalhaAoAtualizarPerfis(user, desbloqueio);
            TempData["Success"] = $"Conta de {user.Email} desbloqueada.";
            return RedirectToAction(nameof(Index));
        }

        if (await EhUnicoAdministradorAtivoAsync(user))
        {
            TempData["Error"] = $"Não é possível bloquear {user.Email}: esta é a única conta Administrador ativa.";
            return RedirectToAction(nameof(Index));
        }

        var bloqueio = await userManager.SetLockoutEnabledAsync(user, true);
        if (bloqueio.Succeeded) bloqueio = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        if (!bloqueio.Succeeded) return FalhaAoAtualizarPerfis(user, bloqueio);

        TempData["Success"] = $"Conta de {user.Email} bloqueada.";
        return RedirectToAction(nameof(Index));
    }

    private static string[] PerfisReconhecidos(IEnumerable<string>? perfis) =>
        (perfis ?? []).Where(Roles.Todos.Contains).Distinct().ToArray();

    private async Task<bool> EhUnicoAdministradorAtivoAsync(ApplicationUser user)
    {
        if (!await userManager.IsInRoleAsync(user, Roles.Administrador)) return false;
        var administradores = await userManager.GetUsersInRoleAsync(Roles.Administrador);
        return administradores.Count(admin => !admin.LockoutEnd.HasValue || admin.LockoutEnd.Value <= DateTimeOffset.UtcNow) <= 1;
    }

    private IActionResult RedirecionarComErroDeSenha(string id, string mensagem)
    {
        TempData["Error"] = mensagem;
        return RedirectToAction(nameof(Index), new { id });
    }

    private IActionResult FalhaAoRedefinirSenha(ApplicationUser user, IdentityResult resultado)
    {
        TempData["Error"] = $"Não foi possível redefinir a senha de {user.Email}: {string.Join("; ", resultado.Errors.Select(erro => erro.Description))}";
        return RedirectToAction(nameof(Index));
    }

    private void AdicionarErros(IdentityResult resultado)
    {
        foreach (var erro in resultado.Errors) ModelState.AddModelError(string.Empty, erro.Description);
    }

    private IActionResult FalhaAoAtualizarPerfis(ApplicationUser user, IdentityResult resultado)
    {
        TempData["Error"] = $"Não foi possível atualizar os perfis de {user.Email}: {string.Join("; ", resultado.Errors.Select(erro => erro.Description))}";
        return RedirectToAction(nameof(Index));
    }
}
