using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SGQ.Web.Models;
using SGQ.Web.Presentation;

namespace SGQ.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResetPasswordModel(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> opcoesIdentity) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool LinkInvalido { get; private set; }

    public IReadOnlyList<string> RequisitosSenha => PoliticaSenhaTexto.Descrever(opcoesIdentity.Value.Password);

    public IActionResult OnGet(string? code = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            LinkInvalido = true;
            return Page();
        }

        try
        {
            Input = new InputModel { Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)) };
        }
        catch (FormatException)
        {
            LinkInvalido = true;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var usuario = await userManager.FindByEmailAsync(Input.Email);
        // Conta inexistente recebe a mesma resposta de sucesso: não revela quais e-mails existem.
        if (usuario is null) return RedirectToPage("./ResetPasswordConfirmation");

        var resultado = await userManager.ResetPasswordAsync(usuario, Input.Code, Input.Password);
        if (resultado.Succeeded) return RedirectToPage("./ResetPasswordConfirmation");

        foreach (var erro in resultado.Errors) ModelState.AddModelError(string.Empty, erro.Description);
        return Page();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Crie uma nova senha.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre {2} e {1} caracteres.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repita a nova senha.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não são iguais.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }
}
