using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace SGQ.Web.Services;

public class SmtpEmailService(IOptions<SmtpOptions> options, ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly SmtpOptions _options = options.Value;

    public bool Configurado => !string.IsNullOrWhiteSpace(_options.Host) && !string.IsNullOrWhiteSpace(_options.From);

    public async Task<bool> EnviarAsync(IEnumerable<string> destinatarios, string assunto, string corpo, CancellationToken cancellationToken = default)
    {
        var emails = destinatarios.Where(email => !string.IsNullOrWhiteSpace(email)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (emails.Count == 0) return false;
        if (!Configurado)
        {
            logger.LogWarning("Alerta por e-mail não enviado: SMTP não configurado.");
            return false;
        }

        using var mensagem = new MailMessage { From = new MailAddress(_options.From!), Subject = assunto, Body = corpo, IsBodyHtml = false };
        mensagem.To.Add(_options.From!);
        foreach (var email in emails) mensagem.Bcc.Add(email);

        using var smtp = new SmtpClient(_options.Host!, _options.Port) { EnableSsl = _options.EnableSsl };
        if (!string.IsNullOrWhiteSpace(_options.UserName)) smtp.Credentials = new NetworkCredential(_options.UserName, _options.Password);
        await smtp.SendMailAsync(mensagem, cancellationToken);
        return true;
    }
}
