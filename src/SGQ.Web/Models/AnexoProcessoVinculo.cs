using SGQ.Domain.Entities;
namespace SGQ.Web.Models;

public class AnexoProcessoVinculo
{
    public int Id { get; set; }
    public int AnexoId { get; set; }
    public Anexo Anexo { get; set; } = null!;
    public int? ReclamacaoClienteId { get; set; }
    public ReclamacaoCliente? ReclamacaoCliente { get; set; }
    public int? NaoConformidadeId { get; set; }
    public NaoConformidade? NaoConformidade { get; set; }
    public int? RecallId { get; set; }
    public Recall? Recall { get; set; }
}
