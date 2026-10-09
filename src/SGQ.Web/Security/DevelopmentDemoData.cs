using Microsoft.EntityFrameworkCore;
using SGQ.Domain.Entities;
using SGQ.Domain.Enums;
using SGQ.Web.Data;
using SGQ.Web.Models;

namespace SGQ.Web.Security;

/// <summary>
/// Carrega dados FICTÍCIOS de demonstração (clientes, produtos, lotes, calendário, RC, NC e Recall em vários estados)
/// para testar o sistema e revisar as telas. Mesmas proteções das contas de teste: só roda em Development, exige
/// <c>DevelopmentTestUsers:Enabled</c> e a conexão deve apontar para o banco <c>DevelopmentTestUsers:ExpectedDatabase</c>,
/// cujo nome termina em <c>_test</c> ou <c>_tests</c>. Nunca use dados reais.
/// </summary>
public static class DevelopmentDemoData
{
    private const string SeedArgument = "--seed-demo-data";
    private const string DemoContactDomain = "@demo.sgq.test";

    public static bool HasRequestedOperation(string[] args) => args.Contains(SeedArgument, StringComparer.Ordinal);

    public static async Task ExecuteAsync(IServiceProvider services, IHostEnvironment environment, IConfiguration configuration)
    {
        if (!environment.IsDevelopment())
            throw new InvalidOperationException("Os dados de demonstração só podem ser carregados no ambiente Development.");

        var options = configuration.GetSection(DevelopmentTestUsersOptions.SectionName).Get<DevelopmentTestUsersOptions>()
            ?? new DevelopmentTestUsersOptions();
        if (!options.Enabled || string.IsNullOrWhiteSpace(options.ExpectedDatabase))
            throw new InvalidOperationException("Defina DevelopmentTestUsers:Enabled e DevelopmentTestUsers:ExpectedDatabase para permitir a operação.");

        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var database = context.Database.GetDbConnection().Database;
        var isTestDatabase = database.EndsWith("_test", StringComparison.OrdinalIgnoreCase) || database.EndsWith("_tests", StringComparison.OrdinalIgnoreCase);
        if (!string.Equals(database, options.ExpectedDatabase, StringComparison.Ordinal) || !isTestDatabase)
            throw new InvalidOperationException("A operação foi bloqueada: a conexão deve apontar exatamente para o banco de testes configurado (nome terminado em _test ou _tests).");

        await context.Database.MigrateAsync();
        if (await context.Clientes.AnyAsync(cliente => cliente.Contato.EndsWith(DemoContactDomain)))
        {
            Console.WriteLine("Os dados de demonstração já existem; nada foi alterado.");
            return;
        }

        await SeedAsync(context);
        Console.WriteLine("Dados de demonstração criados.");
    }

    private static async Task SeedAsync(ApplicationDbContext context)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var ano = hoje.Year;
        var agora = DateTimeOffset.UtcNow;

        // Cadastros
        var clientes = new[]
        {
            new Cliente { Nome = "Distribuidora Horizonte (demo)", Contato = "compras" + DemoContactDomain },
            new Cliente { Nome = "Rede Vida Saudável (demo)", Contato = "qualidade" + DemoContactDomain },
            new Cliente { Nome = "Farmácia Central (demo)", Contato = "atendimento" + DemoContactDomain }
        };
        var produtos = new[]
        {
            new Produto { Nome = "Produto Exemplo Alfa" },
            new Produto { Nome = "Produto Exemplo Beta" },
            new Produto { Nome = "Produto Exemplo Gama" }
        };
        context.Clientes.AddRange(clientes);
        context.Produtos.AddRange(produtos);
        await context.SaveChangesAsync();
        var lotes = new[]
        {
            new Lote { Numero = "ALFA-2026-001", ProdutoId = produtos[0].Id },
            new Lote { Numero = "ALFA-2026-002", ProdutoId = produtos[0].Id },
            new Lote { Numero = "BETA-2026-001", ProdutoId = produtos[1].Id },
            new Lote { Numero = "GAMA-2026-001", ProdutoId = produtos[2].Id }
        };
        context.Lotes.AddRange(lotes);

        foreach (var (data, descricao) in new[]
        {
            (new DateOnly(ano, 11, 2), "Finados"), (new DateOnly(ano, 11, 15), "Proclamação da República"),
            (new DateOnly(ano, 11, 20), "Consciência Negra"), (new DateOnly(ano, 12, 25), "Natal"),
            (new DateOnly(ano + 1, 1, 1), "Confraternização Universal")
        })
            context.DiasNaoUteis.Add(new DiaNaoUtil { Data = data, Descricao = descricao + " (demo)", Tipo = TipoDiaNaoUtil.Nacional, Ano = data.Year });
        await context.SaveChangesAsync();

        // Reclamações de cliente
        ReclamacaoCliente Rc(int seq, StatusReclamacao status, ClassificacaoOcorrencia? classificacao, int diasAtras, int diasParaPrazo, string descricao, Cliente cliente, Produto produto, Lote lote) => new()
        {
            Ano = ano, SequenciaAnual = seq, Codigo = $"RC-{ano}-{seq:D6}", DataRecebimento = hoje.AddDays(-diasAtras),
            DataAlvo = hoje.AddDays(diasParaPrazo), CanalRecebimento = "Telefone", ClienteId = cliente.Id, ContatoCliente = cliente.Contato,
            ProdutoId = produto.Id, Descricao = descricao, QuantidadeEnvolvida = 12, ProdutoDisponivel = true, QuantidadeDisponivel = 3,
            Classificacao = classificacao, Status = status, UsuarioAbertura = "gq" + "@sgq.test", CriadaEm = agora.AddDays(-diasAtras),
            Lotes = { new ReclamacaoClienteLote { LoteId = lote.Id } }
        };
        var rc1 = Rc(1, StatusReclamacao.AguardandoValidacaoGq, null, 2, 13, "Cliente relata embalagem violada na entrega do lote.", clientes[0], produtos[0], lotes[0]);
        var rc2 = Rc(2, StatusReclamacao.EmInvestigacao, ClassificacaoOcorrencia.Maior, 9, 6, "Odor diferente do habitual percebido ao abrir o produto.", clientes[1], produtos[1], lotes[2]);
        rc2.UsuarioValidacao = "gq@sgq.test"; rc2.ValidadaEm = agora.AddDays(-8);
        var rc3 = Rc(3, StatusReclamacao.AguardandoLaboratorioExterno, ClassificacaoOcorrencia.Critica, 20, -3, "Suspeita de contaminação relatada por consumidor final.", clientes[2], produtos[2], lotes[3]);
        rc3.UsuarioValidacao = "gq@sgq.test"; rc3.ValidadaEm = agora.AddDays(-19);
        rc3.LaboratorioExterno = "Laboratório Parceiro (demo)"; rc3.DataEnvioAmostraLaboratorio = hoje.AddDays(-10);
        var rc4 = Rc(4, StatusReclamacao.Encerrada, ClassificacaoOcorrencia.Menor, 40, -10, "Rótulo com impressão pouco nítida.", clientes[0], produtos[0], lotes[1]);
        rc4.UsuarioValidacao = "gq@sgq.test"; rc4.ValidadaEm = agora.AddDays(-39); rc4.Investigacao = "Falha pontual na impressora do rótulo, corrigida.";
        rc4.Resultado = ResultadoReclamacao.Procedente; rc4.TratamentoAplicado = "Troca do produto e ajuste da impressora.";
        rc4.RespostaCliente = "Cliente informado sobre a causa e a troca."; rc4.DataRespostaCliente = hoje.AddDays(-12);
        rc4.UsuarioEncerramento = "gq@sgq.test"; rc4.EncerradaEm = agora.AddDays(-11);
        context.ReclamacoesClientes.AddRange(rc1, rc2, rc3, rc4);
        await context.SaveChangesAsync();

        // Não conformidades
        NaoConformidade Nc(int seq, StatusNaoConformidade status, ClassificacaoOcorrencia classificacao, OrigemNaoConformidade origem, string area, string descricao, int diasAtras, int? diasParaPrazo) => new()
        {
            Ano = ano, SequenciaAnual = seq, Codigo = $"NC-{ano}-{seq:D6}", Origem = origem, DataAbertura = hoje.AddDays(-diasAtras),
            DataAlvo = diasParaPrazo is null ? null : hoje.AddDays(diasParaPrazo.Value), Area = area, Descricao = descricao,
            Classificacao = classificacao, Status = status, UsuarioAbertura = "gq@sgq.test", CriadaEm = agora.AddDays(-diasAtras)
        };
        var nc1 = Nc(1, StatusNaoConformidade.EmInvestigacao, ClassificacaoOcorrencia.Maior, OrigemNaoConformidade.ReclamacaoCliente, "Qualidade", "NC aberta a partir da " + rc2.Codigo + ".", 8, 22);
        nc1.ReclamacaoClienteId = rc2.Id;
        var nc2 = Nc(2, StatusNaoConformidade.EmTratamento, ClassificacaoOcorrencia.Menor, OrigemNaoConformidade.Auditoria, "Produção", "Registro de limpeza de linha sem assinatura do responsável.", 15, 15);
        nc2.Contencao = "Treinamento imediato da equipe do turno."; nc2.Investigacao = "Falha de rotina no turno da noite."; nc2.CausaRaiz = "Checklist sem campo obrigatório."; nc2.MetodoAnalise = "5 Porquês";
        nc2.Acoes.Add(new AcaoNaoConformidade { Descricao = "Revisar o checklist de limpeza de linha.", Responsavel = "rt@sgq.test", Prazo = hoje.AddDays(-2), Obrigatoria = true });
        nc2.Acoes.Add(new AcaoNaoConformidade { Descricao = "Treinar a equipe do turno da noite.", Responsavel = "gq@sgq.test", Prazo = hoje.AddDays(5), Obrigatoria = true, DataConclusao = hoje.AddDays(-1), Evidencia = "Lista de presença anexada ao registro." });
        var nc3 = Nc(3, StatusNaoConformidade.AguardandoAprovacao, ClassificacaoOcorrencia.Critica, OrigemNaoConformidade.Inspecao, "Controle de Qualidade", "Resultado fora da especificação em inspeção de recebimento.", 25, null);
        nc3.Eficaz = true; nc3.AprovadaRt = true; nc3.UsuarioParecerRt = "rt@sgq.test";
        var nc4 = Nc(4, StatusNaoConformidade.AguardandoDecisaoCq, ClassificacaoOcorrencia.Maior, OrigemNaoConformidade.MonitoramentoDeProcesso, "Produção", "Desvio de temperatura em etapa de envase.", 30, -4);
        nc4.Eficaz = true; nc4.AprovadaRt = true; nc4.ReprovadaGq = true; nc4.UsuarioParecerRt = "rt@sgq.test"; nc4.UsuarioParecerGq = "gq@sgq.test";
        var nc5 = Nc(5, StatusNaoConformidade.Encerrada, ClassificacaoOcorrencia.Menor, OrigemNaoConformidade.OutroDesvio, "Almoxarifado", "Etiqueta de identificação ilegível em prateleira.", 60, -30);
        nc5.Eficaz = true; nc5.AprovadaRt = true; nc5.AprovadaGq = true; nc5.UsuarioParecerRt = "rt@sgq.test"; nc5.UsuarioParecerGq = "gq@sgq.test";
        nc5.UsuarioEncerramento = "gq@sgq.test"; nc5.EncerradaEm = agora.AddDays(-31);
        context.NaoConformidades.AddRange(nc1, nc2, nc3, nc4, nc5);
        await context.SaveChangesAsync();

        // Recalls
        Recall Rec(int seq, StatusRecall status, DecisaoRecall decisao, OrigemRecall origem, Produto produto, Lote lote, int diasAtras, int? diasParaPrazo, string natureza) => new()
        {
            Ano = ano, SequenciaAnual = seq, Codigo = $"REC-{ano}-{seq:D6}", Origem = origem, ProdutoId = produto.Id, LoteId = lote.Id,
            DataAbertura = hoje.AddDays(-diasAtras), DataAlvo = diasParaPrazo is null ? null : hoje.AddDays(diasParaPrazo.Value),
            QuantidadeProduzida = 5000, QuantidadeEstoque = 1200, QuantidadeDistribuida = 3800,
            ClientesEnvolvidos = "Distribuidora Horizonte (demo); Rede Vida Saudável (demo)", NaturezaOcorrencia = natureza,
            RiscoPotencial = "Risco avaliado como moderado, conforme análise técnica fictícia.", Decisao = decisao,
            JustificativaDecisao = "Decisão registrada para fins de demonstração.", Status = status, UsuarioAbertura = "gq@sgq.test", CriadaEm = agora.AddDays(-diasAtras)
        };
        var rec1 = Rec(1, StatusRecall.EmAvaliacao, DecisaoRecall.Aplicavel, OrigemRecall.AvaliacaoTecnica, produtos[0], lotes[0], 3, 12, "Possível desvio de rotulagem no lote.");
        var rec2 = Rec(2, StatusRecall.AguardandoAprovacao, DecisaoRecall.Aplicavel, OrigemRecall.FalhaEmbalagem, produtos[1], lotes[2], 7, 8, "Falha na vedação de parte das embalagens.");
        var rec3 = Rec(3, StatusRecall.EmRecolhimento, DecisaoRecall.Aplicavel, OrigemRecall.ReclamacaoCliente, produtos[2], lotes[3], 14, 1, "Suspeita de contaminação confirmada em análise preliminar.");
        rec3.ReclamacaoClienteId = rc3.Id; rec3.AprovadaRt = true; rec3.AprovadaGq = true; rec3.UsuarioParecerRt = "rt@sgq.test"; rec3.UsuarioParecerGq = "gq@sgq.test";
        var rec4 = Rec(4, StatusRecall.Encerrado, DecisaoRecall.NaoAplicavel, OrigemRecall.AvaliacaoTecnica, produtos[0], lotes[1], 45, null, "Avaliação preventiva sem impacto na saúde.");
        rec4.UsuarioEncerramento = "gq@sgq.test"; rec4.EncerradaEm = agora.AddDays(-45);
        context.Recalls.AddRange(rec1, rec2, rec3, rec4);
        await context.SaveChangesAsync();
    }
}
