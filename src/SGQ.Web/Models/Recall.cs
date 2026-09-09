using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class Recall
{
    public int Id { get; set; }

    [Required, StringLength(20)] public string Codigo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public int SequenciaAnual { get; set; }
    public OrigemRecall Origem { get; set; }
    public int? NaoConformidadeId { get; set; }
    public NaoConformidade? NaoConformidade { get; set; }
    public int? ReclamacaoClienteId { get; set; }
    public ReclamacaoCliente? ReclamacaoCliente { get; set; }
    public int ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;
    public int LoteId { get; set; }
    public Lote Lote { get; set; } = null!;
    public DateOnly DataAbertura { get; set; }
    public DateOnly? DataAlvo { get; set; }
    public DateOnly? DataFabricacao { get; set; }
    public DateOnly? DataValidade { get; set; }
    public decimal QuantidadeProduzida { get; set; }
    public decimal QuantidadeEstoque { get; set; }
    public decimal QuantidadeDistribuida { get; set; }
    [Required, StringLength(4000)] public string ClientesEnvolvidos { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string NaturezaOcorrencia { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string RiscoPotencial { get; set; } = string.Empty;
    public DecisaoRecall Decisao { get; set; }
    [Required, StringLength(4000)] public string JustificativaDecisao { get; set; } = string.Empty;
    public StatusRecall Status { get; set; } = StatusRecall.EmAvaliacao;
    public bool BloqueioRegistrado { get; set; }
    [StringLength(2000)] public string? ComunicacaoClientes { get; set; }
    [StringLength(200)] public string? AutoridadeSanitaria { get; set; }
    [StringLength(200)] public string? ProtocoloAutoridade { get; set; }
    public DateTimeOffset? ComunicadaAutoridadeEm { get; set; }
    public bool AprovadaRt { get; set; }
    public bool AprovadaGq { get; set; }
    [StringLength(1000)] public string? Destinacao { get; set; }
    [StringLength(1000)] public string? EvidenciaDestinacao { get; set; }
    [StringLength(256)] public string? UsuarioEncerramento { get; set; }
    public DateTimeOffset? EncerradaEm { get; set; }
    [Required, StringLength(256)] public string UsuarioAbertura { get; set; } = string.Empty;
    public DateTimeOffset CriadaEm { get; set; }
    public ICollection<RetornoRecall> Retornos { get; set; } = new List<RetornoRecall>();
}
