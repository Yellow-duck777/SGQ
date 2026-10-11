using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.ViewModels;

public class UsuarioCreateViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe uma senha inicial.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha inicial")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Repita a senha inicial.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar senha inicial")]
    [Compare(nameof(Password), ErrorMessage = "As senhas não são iguais.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MinLength(1, ErrorMessage = "Selecione ao menos um perfil.")]
    public string[] Perfis { get; set; } = [];
}

public class RedefinirSenhaUsuarioViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Repita a nova senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nova senha")]
    [Compare(nameof(Password), ErrorMessage = "As senhas não são iguais.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
