using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SGQ.Web.Models;
using SGQ.Web.Presentation;

namespace SGQ.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<IdentityOptions> opcoesIdentity,
    ILogger<RegisterModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string ReturnUrl { get; set; } = string.Empty;

    public IReadOnlyList<string> RequisitosSenha => PoliticaSenhaTexto.Descrever(opcoesIdentity.Value.Password);

    public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl ?? Url.Content("~/");

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        if (!ModelState.IsValid) return Page();

        var usuario = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
        var resultado = await userManager.CreateAsync(usuario, Input.Password);
        if (resultado.Succeeded)
        {
            logger.LogInformation("Nova conta criada; aguarda atribuição de perfil por um Administrador.");
            await signInManager.SignInAsync(usuario, isPersistent: false);
            return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : "~/");
        }

        foreach (var erro in resultado.Errors) ModelState.AddModelError(string.Empty, erro.Description);
        return Page();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Crie uma senha.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre {2} e {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repita a senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [Compare("Password", ErrorMessage = "As senhas não são iguais.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
