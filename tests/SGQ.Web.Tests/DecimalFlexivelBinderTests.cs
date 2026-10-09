using System.Globalization;
using SGQ.Web.Presentation;

namespace SGQ.Web.Tests;

public class DecimalFlexivelBinderTests
{
    [Theory]
    [InlineData("150.5", 150.5)]
    [InlineData("150,5", 150.5)]
    [InlineData("1.234,5", 1234.5)]
    [InlineData("1,234.5", 1234.5)]
    [InlineData("1000", 1000)]
    [InlineData("0,25", 0.25)]
    [InlineData("12", 12)]
    public void TentarConverter_AceitaPontoOuVirgulaIndependenteDaCultura(string texto, double esperado)
    {
        var culturaAnterior = CultureInfo.CurrentCulture;
        try
        {
            foreach (var cultura in new[] { "pt-BR", "en-US", "" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultura);
                Assert.True(DecimalFlexivelBinder.TentarConverter(texto, out var numero));
                Assert.Equal((decimal)esperado, numero);
            }
        }
        finally { CultureInfo.CurrentCulture = culturaAnterior; }
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("1,2,3x")]
    [InlineData("--5")]
    public void TentarConverter_RejeitaTextoInvalido(string texto) =>
        Assert.False(DecimalFlexivelBinder.TentarConverter(texto, out _));
}
