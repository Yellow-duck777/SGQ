using System.Globalization;
using System.Security.Claims;
using SGQ.Web.Security;

namespace SGQ.Web.Presentation;

/// <summary>Textos de apresentação do usuário autenticado: nome para saudação e rótulos dos perfis.</summary>
public static class UsuarioApresentacao
{
    /// <summary>
    /// Nome amigável derivado do e-mail/usuário (a conta não guarda nome completo): "maria.silva@empresa.com" vira "Maria Silva".
    /// </summary>
    public static string NomeExibicao(string? usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario)) return "Usuário";
        var local = usuario.Split('@')[0];
        var partes = local.Split(['.', '_', '-', ' ', '+'], StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 0) return "Usuário";
        return string.Join(' ', partes.Select(parte => CultureInfo.GetCultureInfo("pt-BR").TextInfo.ToTitleCase(parte.ToLowerInvariant())));
    }

    /// <summary>Primeiro nome para a saudação ("Maria Silva" vira "Maria").</summary>
    public static string PrimeiroNome(string? usuario) => NomeExibicao(usuario).Split(' ')[0];

    public static string RotuloPerfil(string perfil) => perfil switch
    {
        Roles.Administrador => "Administrador",
        Roles.GarantiaQualidade => "Garantia da Qualidade",
        Roles.ResponsavelTecnico => "Responsável Técnico",
        Roles.ControleQualidade => "Controle de Qualidade",
        Roles.Auditor => "Auditor",
        _ => perfil
    };

    /// <summary>Perfis reconhecidos do usuário, na ordem oficial e já em português.</summary>
    public static IReadOnlyList<string> PerfisDoUsuario(ClaimsPrincipal usuario) =>
        Roles.Todos.Where(usuario.IsInRole).Select(RotuloPerfil).ToList();
}
