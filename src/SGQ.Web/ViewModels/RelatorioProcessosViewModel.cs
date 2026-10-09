using SGQ.Web.Models;

namespace SGQ.Web.ViewModels;

public class RelatorioProcessosViewModel
{
    public DateOnly? Inicio { get; set; }
    public DateOnly? Fim { get; set; }
    public int? ProdutoId { get; set; }
    public string? Tipo { get; set; }
    public List<RelatorioProcessoItemViewModel> Itens { get; set; } = [];
    public int ProcessosAbertos { get; set; }
    public int ProcessosAtrasados { get; set; }
    public double? TempoMedioEncerramentoDias { get; set; }
}

public class RelatorioProcessoItemViewModel
{
    public int Id { get; init; }
    public required string Tipo { get; init; }
    public required string Codigo { get; init; }
    public required DateOnly DataAbertura { get; init; }
    public DateOnly? DataAlvo { get; init; }
    public string? Produto { get; init; }
    public string? Lote { get; init; }
    public required string Situacao { get; init; }
    public string? Classificacao { get; init; }
    public string SituacaoTom { get; init; } = "neutro";
    public string ClassificacaoTom { get; init; } = "neutro";
    public string? AreaOuCliente { get; init; }
    public DateTimeOffset? EncerradaEm { get; init; }
}
