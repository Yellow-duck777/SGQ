using SGQ.Web.Models;

namespace SGQ.Web.ViewModels;

public class DashboardViewModel
{
    public int ReclamacoesAbertas { get; init; }
    public int NaoConformidadesAbertas { get; init; }
    public int ClientesAtivos { get; init; }
    public int ProdutosCadastrados { get; init; }
    public int LotesCadastrados { get; init; }
    public IReadOnlyList<DashboardReclamacaoViewModel> ReclamacoesRecentes { get; init; } = [];
}

public class DashboardReclamacaoViewModel
{
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Cliente { get; init; } = string.Empty;
    public string Produto { get; init; } = string.Empty;
    public StatusReclamacao Status { get; init; }
    public DateTimeOffset CriadaEm { get; init; }
}
