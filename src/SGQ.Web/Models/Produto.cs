using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class Produto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do produto.")]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    public ICollection<Lote> Lotes { get; set; } = new List<Lote>();
}
