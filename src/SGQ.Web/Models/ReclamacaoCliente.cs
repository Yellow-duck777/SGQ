using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class ReclamacaoCliente
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    public int Ano { get; set; }

    public int SequenciaAnual { get; set; }

    [Required]
    public DateOnly DataRecebimento { get; set; }

    [Required]
    [StringLength(100)]
    public string CanalRecebimento { get; set; } = string.Empty;

    [Required]
    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string ContatoCliente { get; set; } = string.Empty;

    [Required]
    public int ProdutoId { get; set; }

    public Produto Produto { get; set; } = null!;

    [Required]
    [StringLength(4000)]
    public string Descricao { get; set; } = string.Empty;

    public DateOnly? DataFabricacao { get; set; }

    public DateOnly? DataValidade { get; set; }

    public decimal? QuantidadeEnvolvida { get; set; }

    [StringLength(200)]
    public string? LocalAquisicao { get; set; }

    public bool ProdutoDisponivel { get; set; }

    public decimal? QuantidadeDisponivel { get; set; }

    [StringLength(100)]
    public string? VolumeDisponivel { get; set; }

    public ClassificacaoOcorrencia? Classificacao { get; set; }

    public StatusReclamacao Status { get; set; } = StatusReclamacao.Rascunho;

    [Required]
    [StringLength(256)]
    public string UsuarioAbertura { get; set; } = string.Empty;

    public DateTimeOffset CriadaEm { get; set; }

    public ICollection<ReclamacaoClienteLote> Lotes { get; set; } = new List<ReclamacaoClienteLote>();

    public NaoConformidade? NaoConformidade { get; set; }
}
