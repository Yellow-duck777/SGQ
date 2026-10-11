using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SGQ.Web.Presentation;

/// <summary>
/// Texto legível e cor dos valores de enumeração. Use <c>Rotulo()</c> em vez de <c>ToString()</c> em qualquer tela
/// e <c>Html.StatusBadge(valor)</c> para exibir situações e classificações como selo colorido.
/// </summary>
public static partial class EnumRotulos
{
    private static readonly ConcurrentDictionary<Enum, string> Cache = new();
    private static readonly Lazy<IReadOnlyDictionary<string, string>> PorNome = new(ConstruirIndicePorNome);

    /// <summary>Nome de exibição definido por <c>[Display(Name = ...)]</c>; sem ele, separa o PascalCase em palavras.</summary>
    public static string Rotulo(this Enum valor) => Cache.GetOrAdd(valor, static v =>
    {
        var membro = v.GetType().GetMember(v.ToString()).FirstOrDefault();
        return membro?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? SepararPalavras(v.ToString());
    });

    public static string Rotulo<T>(this T? valor, string vazio = "—") where T : struct, Enum => valor.HasValue ? valor.Value.Rotulo() : vazio;

    /// <summary>Converte o nome de um membro de enum (como gravado na auditoria) em rótulo; devolve o texto original se não conhecer.</summary>
    public static string RotuloPorNome(string nome) => PorNome.Value.TryGetValue(nome, out var rotulo) ? rotulo : nome;

    /// <summary>Tom visual da situação: neutro, info, alerta, sucesso ou perigo.</summary>
    public static string Tom(this Enum valor) => valor.ToString() switch
    {
        "Encerrada" or "Encerrado" or "Favoravel" or "Procedente" => "sucesso",
        "Critica" or "Desfavoravel" => "perigo",
        "Maior" or "InformacoesPendentes" or "AguardandoValidacaoGq" or "AguardandoConclusao" or "AguardandoLaboratorioExterno"
            or "AguardandoAprovacao" or "AguardandoDecisaoCq" or "AguardandoRetorno" or "AguardandoEncerramento" => "alerta",
        "EmInvestigacao" or "EmTratamento" or "EmAvaliacao" or "EmRecolhimento" or "EmAvaliacaoDeDestinacao" => "info",
        _ => "neutro"
    };

    /// <summary>Selo colorido (<c>status-badge</c>) com o rótulo do valor; vazio exibe um traço discreto.</summary>
    public static IHtmlContent StatusBadge(this IHtmlHelper _, Enum? valor)
    {
        if (valor is null) return new HtmlString("<span class=\"text-muted\">—</span>");
        return new HtmlString($"<span class=\"status-badge\" data-tom=\"{valor.Tom()}\">{WebUtility.HtmlEncode(valor.Rotulo())}</span>");
    }

    private static string SepararPalavras(string texto) =>
        char.ToUpperInvariant(texto[0]) + SeparadorDeMaiusculas().Replace(texto[1..], " $1").ToLowerInvariant();

    private static IReadOnlyDictionary<string, string> ConstruirIndicePorNome()
    {
        var indice = new Dictionary<string, string>(StringComparer.Ordinal);
        var tipos = typeof(EnumRotulos).Assembly.GetTypes().Concat(typeof(SGQ.Domain.DomainAssembly).Assembly.GetTypes()).Where(t => t.IsEnum);
        foreach (var tipo in tipos)
            foreach (var valor in Enum.GetValues(tipo).Cast<Enum>())
                indice.TryAdd(valor.ToString(), valor.Rotulo());
        return indice;
    }

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex SeparadorDeMaiusculas();
}
