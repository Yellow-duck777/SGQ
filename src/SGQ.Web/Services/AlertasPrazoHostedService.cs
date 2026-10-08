using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGQ.Domain.Entities;
using SGQ.Domain.Enums;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Security;

namespace SGQ.Web.Services;

public class AlertasPrazoHostedService(IServiceScopeFactory scopeFactory, ILogger<AlertasPrazoHostedService> logger) : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcessarAsync(stoppingToken); }
            catch (Exception exception) { logger.LogError(exception, "Falha ao processar alertas de prazo."); }
            await Task.Delay(Intervalo, stoppingToken);
        }
    }

    private async Task ProcessarAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var prazoService = scope.ServiceProvider.GetRequiredService<IPrazoService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (!emailService.Configurado) return;

        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var limite = await prazoService.AdicionarDiasUteisAsync(hoje, 2, cancellationToken);
        var destinatariosPrazo = await ObterDestinatariosAsync(userManager, [Roles.GarantiaQualidade, Roles.ResponsavelTecnico]);

        var processos = new List<AlertaPendente>();
        var rcs = await context.ReclamacoesClientes.Where(item => item.Status != StatusReclamacao.Encerrada && item.DataAlvo >= hoje && item.DataAlvo <= limite).Select(item => new { item.Codigo, DataAlvo = item.DataAlvo!.Value }).AsNoTracking().ToListAsync(cancellationToken);
        processos.AddRange(rcs.Select(item => new AlertaPendente("PrazoRC", item.Codigo, item.DataAlvo)));
        var ncs = await context.NaoConformidades.Where(item => item.Status != StatusNaoConformidade.Encerrada && item.DataAlvo >= hoje && item.DataAlvo <= limite).Select(item => new { item.Codigo, DataAlvo = item.DataAlvo!.Value }).AsNoTracking().ToListAsync(cancellationToken);
        processos.AddRange(ncs.Select(item => new AlertaPendente("PrazoNC", item.Codigo, item.DataAlvo)));
        var recalls = await context.Recalls.Where(item => item.Status != StatusRecall.Encerrado && item.DataAlvo >= hoje && item.DataAlvo <= limite).Select(item => new { item.Codigo, DataAlvo = item.DataAlvo!.Value }).AsNoTracking().ToListAsync(cancellationToken);
        processos.AddRange(recalls.Select(item => new AlertaPendente("PrazoRecall", item.Codigo, item.DataAlvo)));

        foreach (var processo in processos)
            await EnviarUmaVezAsync(context, emailService, processo.Tipo, processo.Codigo, processo.DataAlvo, destinatariosPrazo,
                $"SGQ: prazo próximo do vencimento — {processo.Codigo}", $"O processo {processo.Codigo} possui data-alvo em {processo.DataAlvo:dd/MM/yyyy}.", cancellationToken);

        var acoesVencidas = await context.AcoesNaoConformidade.Include(item => item.NaoConformidade)
            .Where(item => item.DataConclusao == null && item.Prazo < hoje).AsNoTracking().ToListAsync(cancellationToken);
        foreach (var acao in acoesVencidas)
        {
            var destinatarios = await ObterDestinatariosAcaoAsync(userManager, acao);
            var diasEmAtraso = hoje.DayNumber - acao.Prazo.DayNumber;
            await EnviarUmaVezAsync(context, emailService, "AcaoNCVencida", $"{acao.NaoConformidade.Codigo}/Ação-{acao.Id}", acao.Prazo, destinatarios,
                $"SGQ: ação de NC vencida — {acao.NaoConformidade.Codigo}", $"A ação '{acao.Descricao}' está vencida há {diasEmAtraso} dia(s). Responsável: {acao.Responsavel}. Prazo original: {acao.Prazo:dd/MM/yyyy}. Classificação: {acao.NaoConformidade.Classificacao}.", cancellationToken);
        }
    }

    private static async Task<List<string>> ObterDestinatariosAsync(UserManager<ApplicationUser> userManager, IEnumerable<string> roles)
    {
        var emails = new List<string>();
        foreach (var role in roles) emails.AddRange((await userManager.GetUsersInRoleAsync(role)).Select(user => user.Email).OfType<string>());
        return emails.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static async Task<List<string>> ObterDestinatariosAcaoAsync(UserManager<ApplicationUser> userManager, AcaoNaoConformidade acao)
    {
        var destinatarios = await ObterDestinatariosAsync(userManager, acao.NaoConformidade.Classificacao == ClassificacaoOcorrencia.Critica
            ? [Roles.GarantiaQualidade, Roles.ResponsavelTecnico] : [Roles.GarantiaQualidade]);
        var responsavel = await userManager.FindByEmailAsync(acao.Responsavel) ?? await userManager.FindByNameAsync(acao.Responsavel);
        if (!string.IsNullOrWhiteSpace(responsavel?.Email)) destinatarios.Add(responsavel.Email);
        else if (acao.Responsavel.Contains('@')) destinatarios.Add(acao.Responsavel);
        return destinatarios.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static async Task EnviarUmaVezAsync(ApplicationDbContext context, IEmailService emailService, string tipo, string referencia, DateOnly dataReferencia, List<string> destinatarios, string assunto, string corpo, CancellationToken cancellationToken)
    {
        if (destinatarios.Count == 0 || await context.AlertasEnviados.AnyAsync(item => item.Tipo == tipo && item.Referencia == referencia && item.DataReferencia == dataReferencia, cancellationToken)) return;
        if (!await emailService.EnviarAsync(destinatarios, assunto, corpo, cancellationToken)) return;
        context.AlertasEnviados.Add(new AlertaEnviado { Tipo = tipo, Referencia = referencia, DataReferencia = dataReferencia, Destinatarios = string.Join("; ", destinatarios), EnviadoEm = DateTimeOffset.UtcNow });
        await context.SaveChangesAsync(cancellationToken);
    }

    private sealed record AlertaPendente(string Tipo, string Codigo, DateOnly DataAlvo);
}
