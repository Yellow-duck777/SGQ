using SGQ.Web.Models;

namespace SGQ.Web.Services;

public interface IPrazoService
{
    Task<DateOnly> AdicionarDiasUteisAsync(DateOnly dataInicial, int diasUteis, CancellationToken cancellationToken = default);
    Task<DateOnly> CalcularPrazoReclamacaoAsync(DateOnly dataInformacoesCompletas, CancellationToken cancellationToken = default);
    Task<DateOnly?> CalcularPrazoNaoConformidadeAsync(DateOnly dataAbertura, ClassificacaoOcorrencia classificacao, CancellationToken cancellationToken = default);
}
