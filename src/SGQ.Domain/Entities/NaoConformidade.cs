using System.ComponentModel.DataAnnotations;
using SGQ.Domain.Enums;

namespace SGQ.Domain.Entities;

public class NaoConformidade
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    public int Ano { get; set; }

    public int SequenciaAnual { get; set; }

    public OrigemNaoConformidade Origem { get; set; }

    public int? ReclamacaoClienteId { get; set; }

    public ReclamacaoCliente? ReclamacaoCliente { get; set; }

    public DateOnly DataAbertura { get; set; }
    public DateOnly? DataAlvo { get; set; }

    [Required, StringLength(150)]
    public string Area { get; set; } = string.Empty;

    public int? ProdutoId { get; set; }

    public Produto? Produto { get; set; }

    [StringLength(4000)]
    public string? Descricao { get; set; }

    public ClassificacaoOcorrencia? Classificacao { get; set; }

    [StringLength(4000)] public string? Contencao { get; set; }
    [StringLength(4000)] public string? Investigacao { get; set; }
    [StringLength(4000)] public string? CausaProvavel { get; set; }
    [StringLength(4000)] public string? CausaRaiz { get; set; }
    [StringLength(200)] public string? MetodoAnalise { get; set; }
    public bool? Eficaz { get; set; }
    public bool AprovadaRt { get; set; }
    public bool AprovadaGq { get; set; }
    public bool ReprovadaRt { get; set; }
    public bool ReprovadaGq { get; set; }
    [StringLength(256)] public string? UsuarioParecerRt { get; set; }
    [StringLength(256)] public string? UsuarioParecerGq { get; set; }
    public StatusDecisaoCq? DecisaoCq { get; set; }
    [StringLength(2000)] public string? JustificativaDecisaoCq { get; set; }
    [StringLength(256)] public string? UsuarioDecisaoCq { get; set; }
    public DateTimeOffset? DecididaPeloCqEm { get; set; }
    [StringLength(256)] public string? UsuarioEncerramento { get; set; }
    public DateTimeOffset? EncerradaEm { get; set; }
    public StatusNaoConformidade? StatusAnteriorReabertura { get; set; }
    [StringLength(2000)] public string? JustificativaReabertura { get; set; }
    [StringLength(256)] public string? UsuarioReabertura { get; set; }
    public DateTimeOffset? ReabertaEm { get; set; }
    [StringLength(200)] public string? LaboratorioExterno { get; set; }
    public DateOnly? DataEnvioAmostraLaboratorio { get; set; }
    public DateOnly? DataRecebimentoResultadoLaboratorio { get; set; }
    [StringLength(200)] public string? IdentificacaoLaudoLaboratorio { get; set; }
    [StringLength(4000)] public string? ResultadoLaboratorio { get; set; }
    public int? LaudoLaboratorioAnexoId { get; set; }

    public StatusNaoConformidade Status { get; set; } = StatusNaoConformidade.EmInvestigacao;

    [Required, StringLength(256)]
    public string UsuarioAbertura { get; set; } = string.Empty;

    public DateTimeOffset CriadaEm { get; set; }

    public ICollection<AcaoNaoConformidade> Acoes { get; set; } = new List<AcaoNaoConformidade>();
}
