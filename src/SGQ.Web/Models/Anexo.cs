using System.ComponentModel.DataAnnotations;
namespace SGQ.Web.Models;
public class Anexo
{
    public int Id { get; set; }
    public int? ReclamacaoClienteId { get; set; }
    public int? NaoConformidadeId { get; set; }
    public int? RecallId { get; set; }
    public ReclamacaoCliente? ReclamacaoCliente { get; set; }
    public NaoConformidade? NaoConformidade { get; set; }
    public Recall? Recall { get; set; }
    [Required, StringLength(260)] public string NomeOriginal { get; set; } = string.Empty;
    [Required, StringLength(100)] public string TipoConteudo { get; set; } = string.Empty;
    [Required, StringLength(260)] public string NomeArmazenado { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    [StringLength(1000)] public string? Descricao { get; set; }
    [Required, StringLength(256)] public string Usuario { get; set; } = string.Empty;
    public DateTimeOffset EnviadoEm { get; set; }
    public bool Critico { get; set; }
    public bool Ativo { get; set; } = true;
    [StringLength(1000)] public string? JustificativaAnulacao { get; set; }
    [StringLength(256)] public string? UsuarioAnulacao { get; set; }
    public DateTimeOffset? AnuladoEm { get; set; }
    public ICollection<AnexoProcessoVinculo> Vinculos { get; set; } = new List<AnexoProcessoVinculo>();
}
