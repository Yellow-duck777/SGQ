namespace SGQ.Web.Services;

public interface INotificacaoService
{
    Task EnviarParaPapeisAsync(string evento, string codigo, IEnumerable<string> papeis, string mensagem, CancellationToken cancellationToken = default);
    Task EnviarParaUsuariosEPapeisAsync(string evento, string codigo, IEnumerable<string> usuarios, IEnumerable<string> papeis, string mensagem, CancellationToken cancellationToken = default);
}
