using System.Net;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SGQ.Web.Presentation;

/// <summary>Etapas de um fluxo para o componente <c>_Stepper</c>. <c>Atual</c> é o índice (0) da etapa em andamento; igual ao total = tudo concluído.</summary>
public sealed record StepperModel(IReadOnlyList<string> Etapas, int Atual);

public static class PrazoChip
{
    /// <summary>
    /// Selo de prazo: "Vencido há N dias" (perigo), "Vence hoje"/"Vence em N dias" (alerta, até 3 dias) ou a data (neutro).
    /// Processo encerrado mostra apenas a data, sem alarme. Uso: <c>@Html.PrazoChipHtml(Model.DataAlvo, encerrado)</c>.
    /// </summary>
    public static IHtmlContent PrazoChipHtml(this IHtmlHelper _, DateOnly? prazo, bool encerrado = false, DateOnly? hoje = null)
    {
        if (prazo is null) return new HtmlString("<span class=\"status-badge\" data-tom=\"neutro\">Sem prazo</span>");
        var data = prazo.Value;
        var texto = data.ToString("dd/MM/yyyy");
        if (encerrado) return new HtmlString($"<span class=\"status-badge\" data-tom=\"neutro\" title=\"Prazo\">{texto}</span>");

        var dias = data.DayNumber - (hoje ?? DateOnly.FromDateTime(DateTime.Today)).DayNumber;
        var (tom, rotulo) = dias switch
        {
            < 0 => ("perigo", $"Vencido há {-dias} dia{(-dias == 1 ? "" : "s")}"),
            0 => ("alerta", "Vence hoje"),
            <= 3 => ("alerta", $"Vence em {dias} dia{(dias == 1 ? "" : "s")}"),
            _ => ("neutro", $"No prazo · {data:dd/MM}")
        };
        return new HtmlString($"<span class=\"status-badge\" data-tom=\"{tom}\" title=\"Prazo: {texto}\">{WebUtility.HtmlEncode(rotulo)}</span>");
    }
}
