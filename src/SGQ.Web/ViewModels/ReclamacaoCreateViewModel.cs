using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.ViewModels;

public class ReclamacaoCreateViewModel
{
    [Required(ErrorMessage = "Informe a data de recebimento.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de recebimento")]
    public DateOnly DataRecebimento { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Informe o canal de recebimento.")]
    [StringLength(100)]
    [Display(Name = "Canal de recebimento")]
    public string CanalRecebimento { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um cliente.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Informe o contato do cliente.")]
    [StringLength(150)]
    [Display(Name = "Contato do cliente")]
    public string ContatoCliente { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um produto.")]
    [Display(Name = "Produto")]
    public int ProdutoId { get; set; }

    [Display(Name = "Lotes envolvidos")]
    public List<int> LoteIds { get; set; } = [];

    [Required(ErrorMessage = "Descreva a reclamação.")]
    [StringLength(4000)]
    [Display(Name = "Descrição da ocorrência")]
    public string Descricao { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Data de fabricação")]
    public DateOnly? DataFabricacao { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data de validade")]
    public DateOnly? DataValidade { get; set; }

    [Display(Name = "Quantidade envolvida")]
    public decimal? QuantidadeEnvolvida { get; set; }

    [StringLength(200)]
    [Display(Name = "Local de aquisição")]
    public string? LocalAquisicao { get; set; }

    [Display(Name = "Produto disponível para análise")]
    public bool ProdutoDisponivel { get; set; }

    [Display(Name = "Quantidade disponível")]
    public decimal? QuantidadeDisponivel { get; set; }

    [StringLength(100)]
    [Display(Name = "Volume disponível")]
    public string? VolumeDisponivel { get; set; }
}
