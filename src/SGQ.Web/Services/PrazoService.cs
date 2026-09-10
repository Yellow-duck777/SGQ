using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;

namespace SGQ.Web.Services;

public class PrazoService(ApplicationDbContext context) : IPrazoService
{
    public async Task<DateOnly> AdicionarDiasUteisAsync(DateOnly dataInicial, int diasUteis, CancellationToken cancellationToken = default)
    {
        if (diasUteis < 0) throw new ArgumentOutOfRangeException(nameof(diasUteis));

        var diasNaoUteis = await context.DiasNaoUteis
            .Where(item => item.Ativo && item.Data > dataInicial)
            .Select(item => item.Data)
            .ToHashSetAsync(cancellationToken);

        var data = dataInicial;
        var restantes = diasUteis;
        while (restantes > 0)
        {
            data = data.AddDays(1);
            if (data.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || diasNaoUteis.Contains(data)) continue;
            restantes--;
        }

        return data;
    }

    // A contagem começa 24 horas após o recebimento das informações completas.
    public Task<DateOnly> CalcularPrazoReclamacaoAsync(DateOnly dataInformacoesCompletas, CancellationToken cancellationToken = default)
        => AdicionarDiasUteisAsync(dataInformacoesCompletas, 15, cancellationToken);

    public async Task<DateOnly?> CalcularPrazoNaoConformidadeAsync(DateOnly dataAbertura, ClassificacaoOcorrencia classificacao, CancellationToken cancellationToken = default)
        => classificacao switch
        {
            ClassificacaoOcorrencia.Maior => await AdicionarDiasUteisAsync(dataAbertura, 15, cancellationToken),
            ClassificacaoOcorrencia.Menor => await AdicionarDiasUteisAsync(dataAbertura, 30, cancellationToken),
            ClassificacaoOcorrencia.Critica => null,
            _ => throw new ArgumentOutOfRangeException(nameof(classificacao))
        };
}
