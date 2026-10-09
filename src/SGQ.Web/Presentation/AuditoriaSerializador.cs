using System.Globalization;
using System.Text;

namespace SGQ.Web.Presentation;

/// <summary>
/// Grava as alterações de auditoria em formato estruturado e independente de cultura. O texto começa com o marcador
/// <see cref="Marcador"/> e separa os campos com <see cref="SeparadorDeCampos"/>; caracteres de controle digitados pelos
/// usuários são trocados por espaço antes de gravar, então texto livre não consegue forjar campos nem linhas do histórico.
/// Registros antigos (sem marcador) continuam sendo lidos pelo formato legado em <see cref="AuditoriaFormatador"/>.
/// </summary>
public static class AuditoriaSerializador
{
    public const char Marcador = '\u001e';
    public const char SeparadorDeCampos = '\u001f';

    public static string Serializar(IEnumerable<(string Nome, object? Antes, object? Depois)> campos) =>
        Marcador + string.Join(SeparadorDeCampos, campos.Select(campo => $"{campo.Nome}: {Valor(campo.Antes)} → {Valor(campo.Depois)}"));

    public static string Valor(object? valor)
    {
        var texto = valor switch
        {
            null => string.Empty,
            DateTimeOffset data => data.ToString("O", CultureInfo.InvariantCulture),
            DateTime data => data.ToString("O", CultureInfo.InvariantCulture),
            DateOnly data => data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly hora => hora.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            IFormattable formatavel => formatavel.ToString(null, CultureInfo.InvariantCulture),
            _ => valor.ToString() ?? string.Empty
        };
        return Limpar(texto);
    }

    private static string Limpar(string texto)
    {
        if (!texto.Any(char.IsControl)) return texto;
        var sb = new StringBuilder(texto.Length);
        foreach (var c in texto) sb.Append(char.IsControl(c) ? ' ' : c);
        return sb.ToString();
    }
}
