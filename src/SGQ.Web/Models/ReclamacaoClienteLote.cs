namespace SGQ.Web.Models;

public class ReclamacaoClienteLote
{
    public int ReclamacaoClienteId { get; set; }

    public ReclamacaoCliente ReclamacaoCliente { get; set; } = null!;

    public int LoteId { get; set; }

    public Lote Lote { get; set; } = null!;
}
