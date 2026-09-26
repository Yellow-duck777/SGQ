using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SGQ.Web.Models;

public class Lote
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o número do lote.")]
    [StringLength(80)]
    [Display(Name = "Número do lote")]
    public string Numero { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um produto.")]
    [Display(Name = "Produto")]
    public int ProdutoId { get; set; }

    [ValidateNever]
    public Produto Produto { get; set; } = null!;
}
