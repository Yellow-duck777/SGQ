using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SGQ.Web.Models;

namespace SGQ.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var usuario = await userManager.FindByEmailAsync(Input.Email);
        // Mesma resposta exista ou não a conta: não revela quais e-mails estão cadastrados.
        if (usuario is null || !await userManager.IsEmailConfirmedAsync(usuario))
            return RedirectToPage("./ForgotPasswordConfirmation");

        var codigo = await userManager.GeneratePasswordResetTokenAsync(usuario);
        codigo = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
        var link = Url.Page("/Account/ResetPassword", pageHandler: null, values: new { area = "Identity", code = codigo }, protocol: Request.Scheme);
        await emailSender.SendEmailAsync(Input.Email, "SGQ · Redefinição de senha",
            $"Para redefinir sua senha, <a href='{HtmlEncoder.Default.Encode(link!)}'>clique aqui</a>.");

        return RedirectToPage("./ForgotPasswordConfirmation");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;
    }
}
