using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do cliente.")]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe um contato.")]
    [StringLength(150)]
    public string Contato { get; set; } = string.Empty;
}
