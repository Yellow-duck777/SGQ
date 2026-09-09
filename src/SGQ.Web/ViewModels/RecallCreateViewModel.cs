using System.ComponentModel.DataAnnotations;
using SGQ.Web.Models;

namespace SGQ.Web.ViewModels;

public class RecallCreateViewModel
{
    [Required, DataType(DataType.Date)] public DateOnly DataAbertura { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [DataType(DataType.Date)] public DateOnly? DataAlvo { get; set; }
    public OrigemRecall Origem { get; set; }
    public int? NaoConformidadeId { get; set; }
    public int? ReclamacaoClienteId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um produto.")] public int ProdutoId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um lote.")] public int LoteId { get; set; }
    [DataType(DataType.Date)] public DateOnly? DataFabricacao { get; set; }
    [DataType(DataType.Date)] public DateOnly? DataValidade { get; set; }
    [Range(0, double.MaxValue)] public decimal QuantidadeProduzida { get; set; }
    [Range(0, double.MaxValue)] public decimal QuantidadeEstoque { get; set; }
    [Range(0, double.MaxValue)] public decimal QuantidadeDistribuida { get; set; }
    [Required, StringLength(4000)] public string ClientesEnvolvidos { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string NaturezaOcorrencia { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string RiscoPotencial { get; set; } = string.Empty;
    public DecisaoRecall Decisao { get; set; }
    [Required, StringLength(4000)] public string JustificativaDecisao { get; set; } = string.Empty;
}
