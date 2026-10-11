using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGQ.Web.Security;

namespace SGQ.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class AccessDeniedModel : PageModel
{
    /// <summary>Usuário autenticado, mas sem nenhum perfil reconhecido (conta nova aguardando o Administrador).</summary>
    public bool SemPerfil { get; private set; }

    public bool Autenticado { get; private set; }

    public void OnGet()
    {
        Autenticado = User.Identity?.IsAuthenticated == true;
        SemPerfil = Autenticado && !Roles.Todos.Any(User.IsInRole);
    }
}
