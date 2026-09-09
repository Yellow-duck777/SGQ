using System.ComponentModel.DataAnnotations;
namespace SGQ.Web.Models;
public class RetornoRecall
{
    public int Id { get; set; }
    public int RecallId { get; set; }
    public Recall Recall { get; set; } = null!;
    [Required, StringLength(200)] public string Cliente { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public DateOnly DataRetorno { get; set; }
    [Required, StringLength(500)] public string CondicaoEmbalagem { get; set; } = string.Empty;
    [StringLength(500)] public string? CondicaoLacre { get; set; }
    [StringLength(1000)] public string? Avarias { get; set; }
    [StringLength(500)] public string? DocumentoTransporte { get; set; }
    [StringLength(1000)] public string? AvaliacaoGq { get; set; }
}
