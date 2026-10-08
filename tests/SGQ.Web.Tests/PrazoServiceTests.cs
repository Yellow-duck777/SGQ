using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SGQ.Domain.Enums;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Services;

namespace SGQ.Web.Tests;

public class PrazoServiceTests
{
    [Fact]
    public async Task AdicionarDiasUteisAsync_PulaFimDeSemana()
    {
        await using var context = CriarContexto();
        var service = new PrazoService(context);

        var resultado = await service.AdicionarDiasUteisAsync(new DateOnly(2026, 9, 25), 1);

        Assert.Equal(new DateOnly(2026, 9, 28), resultado);
    }

    [Fact]
    public async Task AdicionarDiasUteisAsync_PulaDiaNaoUtilAtivo()
    {
        await using var context = CriarContexto();
        context.DiasNaoUteis.Add(new DiaNaoUtil { Data = new DateOnly(2026, 9, 28), Ano = 2026, Tipo = TipoDiaNaoUtil.Nacional, Descricao = "Feriado" });
        await context.SaveChangesAsync();
        var service = new PrazoService(context);

        var resultado = await service.AdicionarDiasUteisAsync(new DateOnly(2026, 9, 25), 1);

        Assert.Equal(new DateOnly(2026, 9, 29), resultado);
    }

    [Fact]
    public async Task CalcularPrazoNaoConformidadeAsync_NaoDefinePrazoParaClassificacaoCritica()
    {
        await using var context = CriarContexto();
        var service = new PrazoService(context);

        var resultado = await service.CalcularPrazoNaoConformidadeAsync(new DateOnly(2026, 9, 22), ClassificacaoOcorrencia.Critica);

        Assert.Null(resultado);
    }

    private static ApplicationDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options, new HttpContextAccessor());
    }
}
