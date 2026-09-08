using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Contato { get; set; } = string.Empty;
}
