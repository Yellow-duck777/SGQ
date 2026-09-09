using System.ComponentModel.DataAnnotations;
using SGQ.Web.Models;

namespace SGQ.Web.ViewModels;

public class ReclamacaoCreateViewModel
{
    [Required(ErrorMessage = "Informe a data de recebimento.")]
    [DataType(DataType.Date)]
    public DateOnly DataRecebimento { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Informe o canal de recebimento.")]
    [StringLength(100)]
    public string CanalRecebimento { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um cliente.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Informe o contato do cliente.")]
    [StringLength(150)]
    public string ContatoCliente { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um produto.")]
    public int ProdutoId { get; set; }

    public List<int> LoteIds { get; set; } = [];

    [Required(ErrorMessage = "Descreva a reclamação.")]
    [StringLength(4000)]
    public string Descricao { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateOnly? DataFabricacao { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? DataValidade { get; set; }

    public decimal? QuantidadeEnvolvida { get; set; }

    [StringLength(200)]
    public string? LocalAquisicao { get; set; }

    public bool ProdutoDisponivel { get; set; }

    public decimal? QuantidadeDisponivel { get; set; }

    [StringLength(100)]
    public string? VolumeDisponivel { get; set; }
}
