using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class ProrrogacaoPrazo
{
    public int Id { get; set; }
    public int? ReclamacaoClienteId { get; set; }
    public int? NaoConformidadeId { get; set; }
    public int? RecallId { get; set; }
    [Required] public DateOnly DataAnterior { get; set; }
    [Required] public DateOnly NovaData { get; set; }
    [Required, StringLength(2000)] public string Motivo { get; set; } = string.Empty;
    public bool ClienteComunicado { get; set; }
    [StringLength(1000)] public string? RegistroComunicacaoCliente { get; set; }
    [Required, StringLength(256)] public string Usuario { get; set; } = string.Empty;
    public DateTimeOffset RegistradaEm { get; set; }
}
