using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace SGQ.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LockoutModel(IOptions<IdentityOptions> opcoes) : PageModel
{
    /// <summary>Duração do bloqueio em minutos, conforme a configuração do Identity.</summary>
    public int Minutos => (int)Math.Ceiling(opcoes.Value.Lockout.DefaultLockoutTimeSpan.TotalMinutes);

    public void OnGet()
    {
    }
}
