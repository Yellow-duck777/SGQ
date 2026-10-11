using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SGQ.Web.Models;
using SGQ.Web.Presentation;

namespace SGQ.Web.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ChangePasswordModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<IdentityOptions> opcoesIdentity,
    ILogger<ChangePasswordModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<string> RequisitosSenha => PoliticaSenhaTexto.Descrever(opcoesIdentity.Value.Password);

    public async Task<IActionResult> OnGetAsync()
    {
        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null) return NotFound("Usuário não encontrado.");
        return await userManager.HasPasswordAsync(usuario) ? Page() : RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null) return NotFound("Usuário não encontrado.");

        var resultado = await userManager.ChangePasswordAsync(usuario, Input.OldPassword, Input.NewPassword);
        if (!resultado.Succeeded)
        {
            foreach (var erro in resultado.Errors) ModelState.AddModelError(string.Empty, erro.Description);
            return Page();
        }

        await signInManager.RefreshSignInAsync(usuario);
        logger.LogInformation("Usuário alterou a própria senha.");
        TempData["Success"] = "Senha alterada com sucesso.";
        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Informe a senha atual.")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Crie uma nova senha.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre {2} e {1} caracteres.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repita a nova senha.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "As senhas não são iguais.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
