namespace SGQ.Web.Services;

public interface IEmailService
{
    bool Configurado { get; }
    Task<bool> EnviarAsync(IEnumerable<string> destinatarios, string assunto, string corpo, CancellationToken cancellationToken = default);
}
