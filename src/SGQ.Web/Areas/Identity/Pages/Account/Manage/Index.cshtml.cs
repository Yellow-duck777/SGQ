using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGQ.Web.Models;
using SGQ.Web.Presentation;

namespace SGQ.Web.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class IndexModel(UserManager<ApplicationUser> userManager) : PageModel
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public IReadOnlyList<string> Perfis { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null) return NotFound("Usuário não encontrado.");

        Email = usuario.Email ?? usuario.UserName ?? string.Empty;
        Nome = UsuarioApresentacao.NomeExibicao(Email);
        Perfis = UsuarioApresentacao.PerfisDoUsuario(User);
        return Page();
    }
}
