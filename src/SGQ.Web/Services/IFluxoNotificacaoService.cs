namespace SGQ.Web.Services;

public interface IFluxoNotificacaoService
{
    Task NotificarAsync(
        string tipo,
        string referencia,
        IEnumerable<string> perfis,
        string assunto,
        string corpo,
        IEnumerable<string>? usuariosEnvolvidos = null,
        CancellationToken cancellationToken = default);
}
