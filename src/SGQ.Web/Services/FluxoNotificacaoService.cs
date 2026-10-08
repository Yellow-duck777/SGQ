using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;

namespace SGQ.Web.Services;

public class FluxoNotificacaoService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    ILogger<FluxoNotificacaoService> logger) : IFluxoNotificacaoService
{
    public async Task NotificarAsync(
        string tipo,
        string referencia,
        IEnumerable<string> perfis,
        string assunto,
        string corpo,
        IEnumerable<string>? usuariosEnvolvidos = null,
        CancellationToken cancellationToken = default)
    {
        if (!emailService.Configurado) return;

        try
        {
            var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
            var jaEnviado = await context.AlertasEnviados.AnyAsync(
                item => item.Tipo == tipo && item.Referencia == referencia && item.DataReferencia == dataReferencia,
                cancellationToken);
            if (jaEnviado) return;

            var destinatarios = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var perfil in perfis.Distinct())
                foreach (var usuario in await userManager.GetUsersInRoleAsync(perfil))
                    if (!string.IsNullOrWhiteSpace(usuario.Email)) destinatarios.Add(usuario.Email);

            foreach (var identificacao in usuariosEnvolvidos?.Where(item => !string.IsNullOrWhiteSpace(item)).Distinct() ?? [])
            {
                var usuario = await userManager.FindByEmailAsync(identificacao) ?? await userManager.FindByNameAsync(identificacao);
                if (!string.IsNullOrWhiteSpace(usuario?.Email)) destinatarios.Add(usuario.Email);
                else if (identificacao.Contains('@')) destinatarios.Add(identificacao);
            }

            if (destinatarios.Count == 0) return;
            if (!await emailService.EnviarAsync(destinatarios, assunto, corpo, cancellationToken)) return;

            context.AlertasEnviados.Add(new AlertaEnviado
            {
                Tipo = tipo,
                Referencia = referencia,
                DataReferencia = dataReferencia,
                Destinatarios = string.Join("; ", destinatarios),
                EnviadoEm = DateTimeOffset.UtcNow
            });
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Falha ao enviar a notificação {Tipo} do processo {Referencia}.", tipo, referencia);
        }
    }
}
