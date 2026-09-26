using Microsoft.AspNetCore.Identity;
using SGQ.Web.Models;

namespace SGQ.Web.Services;

public sealed class NotificacaoService(
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    ILogger<NotificacaoService> logger) : INotificacaoService
{
    public Task EnviarParaPapeisAsync(string evento, string codigo, IEnumerable<string> papeis, string mensagem, CancellationToken cancellationToken = default) =>
        EnviarParaUsuariosEPapeisAsync(evento, codigo, [], papeis, mensagem, cancellationToken);

    public async Task EnviarParaUsuariosEPapeisAsync(string evento, string codigo, IEnumerable<string> usuarios, IEnumerable<string> papeis, string mensagem, CancellationToken cancellationToken = default)
    {
        if (!emailService.Configurado) return;

        try
        {
            var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var papel in papeis.Distinct(StringComparer.Ordinal))
                foreach (var usuario in await userManager.GetUsersInRoleAsync(papel))
                    if (!string.IsNullOrWhiteSpace(usuario.Email)) emails.Add(usuario.Email);

            foreach (var nome in usuarios.Where(nome => !string.IsNullOrWhiteSpace(nome)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var usuario = await userManager.FindByNameAsync(nome);
                if (!string.IsNullOrWhiteSpace(usuario?.Email)) emails.Add(usuario.Email);
            }

            if (emails.Count == 0) return;
            if (!await emailService.EnviarAsync(emails, $"SGQ: {evento} — {codigo}", $"{mensagem}\n\nReferência: {codigo}", cancellationToken))
                logger.LogWarning("Não foi possível enviar a notificação {Evento} do processo {Codigo}.", evento, codigo);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Falha ao preparar a notificação {Evento} do processo {Codigo}.", evento, codigo);
        }
    }
}
