using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class Lote
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o número do lote.")]
    [StringLength(80)]
    public string Numero { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um produto.")]
    public int ProdutoId { get; set; }

    public Produto Produto { get; set; } = null!;
}
