using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace SGQ.Web.Presentation;

/// <summary>
/// Lê números decimais enviados com ponto ou vírgula como separador decimal, independentemente da cultura do servidor.
/// Campos <c>type="number"</c> enviam sempre com ponto ("150.5"); em um servidor pt-BR o binder padrão interpretaria o ponto
/// como separador de milhar e gravaria 1505. Quem digita em campo de texto pode usar vírgula ("150,5") ou o formato
/// brasileiro completo ("1.234,5"). Atenção: um único ponto é sempre decimal ("1.500" = 1,5); por isso os campos de
/// quantidade usam <c>type="number"</c>, que nunca envia separador de milhar.
/// </summary>
public sealed class DecimalFlexivelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);
        var valor = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valor == ValueProviderResult.None) return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valor);
        var texto = valor.FirstValue?.Trim();
        if (string.IsNullOrEmpty(texto))
        {
            // Vazio: nulo para decimal?; para decimal não nulo é erro (nunca grava 0 em silêncio).
            if (bindingContext.ModelMetadata.IsReferenceOrNullableType) bindingContext.Result = ModelBindingResult.Success(null);
            else bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Informe um número (ex.: 150,5).");
            return Task.CompletedTask;
        }

        if (TentarConverter(texto, out var numero)) bindingContext.Result = ModelBindingResult.Success(numero);
        else bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Informe um número válido (ex.: 150,5).");
        return Task.CompletedTask;
    }

    public static bool TentarConverter(string texto, out decimal numero)
    {
        var temVirgula = texto.Contains(',');
        var temPonto = texto.Contains('.');
        string normalizado;
        if (temVirgula && temPonto)
        {
            // O último separador é o decimal; o outro é agrupamento de milhar.
            normalizado = texto.LastIndexOf(',') > texto.LastIndexOf('.')
                ? texto.Replace(".", "").Replace(',', '.')
                : texto.Replace(",", "");
        }
        else if (temVirgula) normalizado = texto.Replace(',', '.');
        else if (texto.Count(c => c == '.') > 1) normalizado = texto.Replace(".", ""); // 1.234.567: pontos só podem ser milhar
        else normalizado = texto; // um único ponto (formato do navegador, campo number) ou inteiro

        return decimal.TryParse(normalizado, NumberStyles.Number, CultureInfo.InvariantCulture, out numero);
    }
}

public sealed class DecimalFlexivelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var tipo = context.Metadata.UnderlyingOrModelType;
        return tipo == typeof(decimal) ? new DecimalFlexivelBinder() : null;
    }
}
