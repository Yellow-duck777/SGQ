using System.ComponentModel.DataAnnotations;
namespace SGQ.Web.Models;
public class HistoricoAuditoria
{
    public long Id { get; set; }
    [Required, StringLength(150)] public string Entidade { get; set; } = string.Empty;
    [Required, StringLength(100)] public string ChaveRegistro { get; set; } = string.Empty;
    [Required, StringLength(30)] public string Acao { get; set; } = string.Empty;
    [Required, StringLength(256)] public string Usuario { get; set; } = string.Empty;
    public DateTimeOffset OcorridaEm { get; set; }
    public string? Alteracoes { get; set; }
}
