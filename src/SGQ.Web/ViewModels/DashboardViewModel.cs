using SGQ.Domain.Enums;
using SGQ.Web.Models;

namespace SGQ.Web.ViewModels;

public class DashboardViewModel
{
    public int ReclamacoesAbertas { get; init; }
    public int NaoConformidadesAbertas { get; init; }
    public int RecallsAtivos { get; init; }
    public int AguardandoAprovacao { get; init; }
    public int ProcessosVencidos { get; init; }
    public int ProcessosProximosDoVencimento { get; init; }
    public int ClientesAtivos { get; init; }
    public int ProdutosCadastrados { get; init; }
    public int LotesCadastrados { get; init; }
    public IReadOnlyList<DashboardReclamacaoViewModel> ReclamacoesRecentes { get; init; } = [];

    /// <summary>Primeiro nome para a saudação e perfis (em português) do usuário.</summary>
    public string NomeUsuario { get; init; } = string.Empty;
    public IReadOnlyList<string> Perfis { get; init; } = [];

    /// <summary>Resumo de cada tipo de processo (em aberto, vencidos, vencendo).</summary>
    public DashboardResumoProcesso ResumoReclamacoes { get; init; } = new();
    public DashboardResumoProcesso ResumoNaoConformidades { get; init; } = new();
    public DashboardResumoProcesso ResumoRecalls { get; init; } = new();

    /// <summary>Pendências específicas do perfil ("Precisa da sua atenção").</summary>
    public IReadOnlyList<DashboardAtencaoViewModel> Atencao { get; init; } = [];

    /// <summary>Últimos processos abertos, de qualquer tipo.</summary>
    public IReadOnlyList<DashboardMovimentoViewModel> Movimentos { get; init; } = [];
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

public class DashboardResumoProcesso
{
    public int Abertos { get; init; }
    public int Vencidos { get; init; }
    public int VencendoEmBreve { get; init; }
}

public class DashboardAtencaoViewModel
{
    public string Titulo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public int Contagem { get; init; }
    public string Sigla { get; init; } = string.Empty;
    /// <summary>Nome do controller da lista (Reclamacoes, NaoConformidades, Recalls).</summary>
    public string Controller { get; init; } = string.Empty;
    /// <summary>Nome do valor de enum a filtrar na lista (parâmetro <c>status</c>); vazio abre a lista inteira.</summary>
    public string? Status { get; init; }
    public DateOnly? MenorPrazo { get; init; }
    public bool Critico { get; init; }
}

public class DashboardMovimentoViewModel
{
    public string Sigla { get; init; } = string.Empty;
    public string Controller { get; init; } = string.Empty;
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Subtitulo { get; init; } = string.Empty;
    public Enum Status { get; init; } = StatusReclamacao.Rascunho;
    public bool Encerrado { get; init; }
    public DateOnly? DataAlvo { get; init; }
    public DateTimeOffset CriadaEm { get; init; }
}
