using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGQ.Web.Models;

namespace SGQ.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger) : PageModel
{
    public IActionResult OnGet(string? returnUrl = null)
    {
        // Quem já saiu não precisa de confirmação; quem chegou digitando o endereço confirma pelo botão (POST).
        if (!signInManager.IsSignedIn(User)) return RedirectToPage("./Login");
        ReturnUrl = returnUrl;
        return Page();
    }

    public string? ReturnUrl { get; private set; }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        await signInManager.SignOutAsync();
        logger.LogInformation("Usuário saiu do sistema.");
        if (returnUrl is not null && Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl);
        return RedirectToPage("./Login");
    }
}
