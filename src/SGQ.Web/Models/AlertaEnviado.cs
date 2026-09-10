using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class AlertaEnviado
{
    public long Id { get; set; }

    [Required, StringLength(80)]
    public string Tipo { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Referencia { get; set; } = string.Empty;

    public DateOnly DataReferencia { get; set; }

    [Required, StringLength(2000)]
    public string Destinatarios { get; set; } = string.Empty;

    public DateTimeOffset EnviadoEm { get; set; }
}
