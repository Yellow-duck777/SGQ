using System.ComponentModel.DataAnnotations;
using SGQ.Domain.Enums;
using SGQ.Web.Models;

namespace SGQ.Web.ViewModels;

public class ReclamacaoConclusaoViewModel
{
    public int Id { get; set; }
    [Required, StringLength(4000)] public string Investigacao { get; set; } = string.Empty;
    [Required] public ResultadoReclamacao Resultado { get; set; }
    [Required, StringLength(4000)] public string TratamentoAplicado { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string RespostaCliente { get; set; } = string.Empty;
    [Required, DataType(DataType.Date)] public DateOnly DataRespostaCliente { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}
