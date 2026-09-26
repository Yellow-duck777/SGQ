using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SGQ.Web.Models;

public class ReclamacaoClienteLote
{
    public int ReclamacaoClienteId { get; set; }

    [ValidateNever]
    public ReclamacaoCliente ReclamacaoCliente { get; set; } = null!;

    public int LoteId { get; set; }

    [ValidateNever]
    public Lote Lote { get; set; } = null!;
}
