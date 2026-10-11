using System.ComponentModel.DataAnnotations;
using SGQ.Web.Models;

namespace SGQ.Web.ViewModels;

public class RecallCreateViewModel
{
    [Required(ErrorMessage = "Informe a data de abertura."), DataType(DataType.Date)] public DateOnly DataAbertura { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [DataType(DataType.Date)] public DateOnly? DataAlvo { get; set; }
    public OrigemRecall Origem { get; set; }
    public int? NaoConformidadeId { get; set; }
    public int? ReclamacaoClienteId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um produto.")] public int ProdutoId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um lote.")] public int LoteId { get; set; }
    [DataType(DataType.Date)] public DateOnly? DataFabricacao { get; set; }
    [DataType(DataType.Date)] public DateOnly? DataValidade { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Informe um valor igual ou maior que zero.")] public decimal QuantidadeProduzida { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Informe um valor igual ou maior que zero.")] public decimal QuantidadeEstoque { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Informe um valor igual ou maior que zero.")] public decimal QuantidadeDistribuida { get; set; }
    [Required(ErrorMessage = "Informe os clientes envolvidos."), StringLength(4000, ErrorMessage = "Use no máximo 4000 caracteres.")] public string ClientesEnvolvidos { get; set; } = string.Empty;
    [Required(ErrorMessage = "Descreva o que aconteceu."), StringLength(4000, ErrorMessage = "Use no máximo 4000 caracteres.")] public string NaturezaOcorrencia { get; set; } = string.Empty;
    [Required(ErrorMessage = "Descreva o risco potencial."), StringLength(4000, ErrorMessage = "Use no máximo 4000 caracteres.")] public string RiscoPotencial { get; set; } = string.Empty;
    public DecisaoRecall Decisao { get; set; }
    [Required(ErrorMessage = "Informe a justificativa da decisão."), StringLength(4000, ErrorMessage = "Use no máximo 4000 caracteres.")] public string JustificativaDecisao { get; set; } = string.Empty;
}
