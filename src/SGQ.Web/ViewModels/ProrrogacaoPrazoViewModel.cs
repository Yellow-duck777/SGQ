using System.ComponentModel.DataAnnotations;
namespace SGQ.Web.ViewModels;
public class ProrrogacaoPrazoViewModel
{
    public int Id { get; set; }
    [Required] public DateOnly? NovaData { get; set; }
    [Required, StringLength(2000, MinimumLength = 10)] public string Motivo { get; set; } = string.Empty;
    public bool ClienteComunicado { get; set; }
    [StringLength(1000)] public string? RegistroComunicacaoCliente { get; set; }
}
