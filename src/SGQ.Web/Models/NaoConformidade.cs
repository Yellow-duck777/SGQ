using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

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

    [Required, StringLength(150)]
    public string Area { get; set; } = string.Empty;

    public int? ProdutoId { get; set; }

    public Produto? Produto { get; set; }

    [StringLength(4000)]
    public string? Descricao { get; set; }

    public ClassificacaoOcorrencia? Classificacao { get; set; }

    public StatusNaoConformidade Status { get; set; } = StatusNaoConformidade.EmInvestigacao;

    [Required, StringLength(256)]
    public string UsuarioAbertura { get; set; } = string.Empty;

    public DateTimeOffset CriadaEm { get; set; }
}
