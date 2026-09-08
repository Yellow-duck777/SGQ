using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class Lote
{
    public int Id { get; set; }

    [Required]
    public string Numero { get; set; } = string.Empty;

    [Required]
    public int ProdutoId { get; set; }

    public Produto Produto { get; set; } = null!;
}
