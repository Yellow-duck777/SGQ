using SGQ.Web.Presentation;

namespace SGQ.Web.Tests;

public class AuditoriaFormatadorTests
{
    [Fact]
    public void Interpretar_Modificacao_MostraSoOQueMudouComNomesELabelsLegiveis()
    {
        var texto = "Status: EmTratamento → AguardandoAprovacao; Eficaz:  → True; Ano: 2026 → 2026; ConcurrencyStamp: a → b; AprovadaRt: False → True; ProdutoId: 1 → 2";

        var alteracoes = AuditoriaFormatador.Interpretar(texto);

        Assert.Collection(alteracoes,
            item => { Assert.Equal("Situação", item.Campo); Assert.Equal("Em tratamento", item.De); Assert.Equal("Aguardando aprovação", item.Para); },
            item => { Assert.Equal("Eficácia comprovada", item.Campo); Assert.Null(item.De); Assert.Equal("Sim", item.Para); },
            item => { Assert.Equal("Aprovação do RT", item.Campo); Assert.Equal("Não", item.De); Assert.Equal("Sim", item.Para); });
    }

    [Fact]
    public void Interpretar_Criacao_ListaCamposPreenchidosEOcultaVaziosTecnicosEFalsos()
    {
        var texto = "Id: 7 → 7; Codigo: NC-2026-000003 → NC-2026-000003; Origem: Inspecao → Inspecao; Descricao:  → ; AprovadaGq: False → False; Area: Qualidade → Qualidade";

        var alteracoes = AuditoriaFormatador.Interpretar(texto, criacao: true);

        Assert.Equal(["Código", "Origem", "Área"], alteracoes.Select(item => item.Campo));
        Assert.Equal("Inspeção", alteracoes[1].Para);
        Assert.All(alteracoes, item => Assert.Null(item.De));
    }

    [Fact]
    public void Interpretar_ValorComPontoEVirgula_NaoQuebraOCampo()
    {
        var alteracoes = AuditoriaFormatador.Interpretar("Descricao: antigo → Texto com; ponto e vírgula; e mais; Status: Rascunho → EmInvestigacao");

        Assert.Equal(2, alteracoes.Count);
        Assert.Equal("Texto com; ponto e vírgula; e mais", alteracoes[0].Para);
    }

    [Fact]
    public void Rotulo_UsaDisplayEFazFallbackParaPalavras()
    {
        Assert.Equal("Aguardando decisão do CQ", SGQ.Domain.Enums.StatusNaoConformidade.AguardandoDecisaoCq.Rotulo());
        Assert.Equal("Encerrada", SGQ.Domain.Enums.StatusNaoConformidade.Encerrada.Rotulo());
        Assert.Equal("Crítica", SGQ.Domain.Enums.ClassificacaoOcorrencia.Critica.Rotulo());
        Assert.Equal("perigo", SGQ.Domain.Enums.ClassificacaoOcorrencia.Critica.Tom());
        Assert.Equal("—", ((SGQ.Domain.Enums.ClassificacaoOcorrencia?)null).Rotulo());
    }
}
