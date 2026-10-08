using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.FileProviders;
using SGQ.Domain.Entities;
using SGQ.Domain.Enums;
using SGQ.Web.Controllers;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Security;
using SGQ.Web.Services;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Tests;

// Cobre as correções da branch de segurança e fluxos (DEM-2026-008): segregação RT/GQ, rodadas de aprovação
// limpas, reabertura sem estado residual, deadlock do Recall, auditoria sem credenciais e rotas indevidas.
public class CorrecoesSegurancaFluxosTests
{
    // ---------- Rotas e autorização declarada ----------

    [Fact]
    public void ReclamacoesController_NaoExpoeMaisAvancarStatus()
    {
        Assert.Null(typeof(ReclamacoesController).GetMethod("AvancarStatus"));
    }

    [Fact]
    public void Classificar_ExigeGestaoDaQualidade()
    {
        var atributo = typeof(ReclamacoesController).GetMethod(nameof(ReclamacoesController.Classificar))!
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(atributo);
        Assert.Equal(Roles.GestaoQualidade, atributo!.Roles);
    }

    [Theory]
    [InlineData(typeof(NaoConformidadesController))]
    [InlineData(typeof(RecallsController))]
    public void AprovarEReprovar_NaoPermitemAdministrador(Type controller)
    {
        foreach (var nome in new[] { "Aprovar", "Reprovar" })
        {
            var perfis = controller.GetMethod(nome)!.GetCustomAttribute<AuthorizeAttribute>()!.Roles!.Split(',');
            Assert.DoesNotContain(Roles.Administrador, perfis);
            Assert.Contains(Roles.ResponsavelTecnico, perfis);
            Assert.Contains(Roles.GarantiaQualidade, perfis);
        }
    }

    [Fact]
    public void Csv_NeutralizaInjecaoDeFormula()
    {
        var csv = typeof(RelatoriosController).GetMethod("Csv", BindingFlags.NonPublic | BindingFlags.Static)!;

        Assert.Equal("\"'=HYPERLINK(\"\"x\"\")\"", csv.Invoke(null, ["=HYPERLINK(\"x\")"]));
        Assert.Equal("\"'@SUM(A1)\"", csv.Invoke(null, ["@SUM(A1)"]));
        Assert.Equal("\"'+1\"", csv.Invoke(null, ["+1"]));
        Assert.Equal("\"Cliente comum\"", csv.Invoke(null, ["Cliente comum"]));
    }

    // ---------- Segregação RT/GQ ----------

    [Fact]
    public async Task NcAprovar_AdministradorNaoPodeAprovarComoRt()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.AguardandoAprovacao);
        var controller = CriarNc(context, "admin", Roles.Administrador);

        var resultado = await controller.Aprovar(nc.Id, "RT");

        Assert.IsType<ForbidResult>(resultado);
        Assert.False((await context.NaoConformidades.FindAsync(nc.Id))!.AprovadaRt);
    }

    [Fact]
    public async Task NcAprovar_MesmoUsuarioNaoAprovaPelosDoisPerfis()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.AguardandoAprovacao);
        var controller = CriarNc(context, "maria", Roles.ResponsavelTecnico, Roles.GarantiaQualidade);

        await controller.Aprovar(nc.Id, "RT");
        await controller.Aprovar(nc.Id, "GQ");

        var atualizada = (await context.NaoConformidades.FindAsync(nc.Id))!;
        Assert.True(atualizada.AprovadaRt);
        Assert.Equal("maria", atualizada.UsuarioParecerRt);
        Assert.False(atualizada.AprovadaGq);
        Assert.Null(atualizada.UsuarioParecerGq);
    }

    [Fact]
    public async Task NcAprovar_ContasDistintasCompletamAAprovacaoERegistramAutores()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.AguardandoAprovacao);

        await CriarNc(context, "rita", Roles.ResponsavelTecnico).Aprovar(nc.Id, "RT");
        await CriarNc(context, "gabi", Roles.GarantiaQualidade).Aprovar(nc.Id, "GQ");

        var atualizada = (await context.NaoConformidades.FindAsync(nc.Id))!;
        Assert.True(atualizada.AprovadaRt && atualizada.AprovadaGq);
        Assert.Equal("rita", atualizada.UsuarioParecerRt);
        Assert.Equal("gabi", atualizada.UsuarioParecerGq);
    }

    [Fact]
    public async Task RecallAprovar_MesmoUsuarioNaoAprovaPelosDoisPerfis()
    {
        await using var context = CriarContexto();
        var recall = await AdicionarRecallAsync(context, StatusRecall.AguardandoAprovacao);
        var controller = CriarRecall(context, "maria", Roles.ResponsavelTecnico, Roles.GarantiaQualidade);

        await controller.Aprovar(recall.Id, "RT");
        await controller.Aprovar(recall.Id, "GQ");

        var atualizado = (await context.Recalls.FindAsync(recall.Id))!;
        Assert.True(atualizado.AprovadaRt);
        Assert.False(atualizado.AprovadaGq);
        Assert.Equal(StatusRecall.AguardandoAprovacao, atualizado.Status);
    }

    [Fact]
    public async Task RecallAprovar_ContasDistintasLevamParaRecolhimento()
    {
        await using var context = CriarContexto();
        var recall = await AdicionarRecallAsync(context, StatusRecall.AguardandoAprovacao);

        await CriarRecall(context, "rita", Roles.ResponsavelTecnico).Aprovar(recall.Id, "RT");
        await CriarRecall(context, "gabi", Roles.GarantiaQualidade).Aprovar(recall.Id, "GQ");

        var atualizado = (await context.Recalls.FindAsync(recall.Id))!;
        Assert.Equal(StatusRecall.EmRecolhimento, atualizado.Status);
        Assert.Equal("rita", atualizado.UsuarioParecerRt);
        Assert.Equal("gabi", atualizado.UsuarioParecerGq);
    }

    // ---------- Rodadas de aprovação e reabertura ----------

    [Fact]
    public async Task NcReabrir_AposDecisaoFavoravelDoCq_NaoDeixaEstadoResidual()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.Encerrada, aprovadaRt: true);
        nc.ReprovadaGq = true;
        nc.DecisaoCq = StatusDecisaoCq.Favoravel;
        nc.JustificativaDecisaoCq = "Decisão anterior do CQ.";
        nc.UsuarioDecisaoCq = "cq";
        nc.DecididaPeloCqEm = DateTimeOffset.UtcNow;
        nc.UsuarioParecerRt = "rita";
        nc.UsuarioEncerramento = "gq";
        nc.EncerradaEm = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        await CriarNc(context, "gq", Roles.GarantiaQualidade).Reabrir(new NaoConformidadeReaberturaViewModel
        {
            Id = nc.Id,
            Justificativa = "Nova evidência exige revisão da investigação."
        });

        var atualizada = (await context.NaoConformidades.FindAsync(nc.Id))!;
        Assert.Equal(StatusNaoConformidade.EmInvestigacao, atualizada.Status);
        Assert.False(atualizada.AprovadaRt);
        Assert.False(atualizada.ReprovadaGq);
        Assert.Null(atualizada.DecisaoCq);
        Assert.Null(atualizada.JustificativaDecisaoCq);
        Assert.Null(atualizada.UsuarioDecisaoCq);
        Assert.Null(atualizada.DecididaPeloCqEm);
        Assert.Null(atualizada.UsuarioParecerRt);
        Assert.Null(atualizada.EncerradaEm);
        Assert.Null(atualizada.UsuarioEncerramento);
    }

    [Fact]
    public async Task NcAvaliarEficacia_IniciaNovaRodadaSemPareceresAnteriores()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.EmTratamento);
        nc.ReprovadaRt = true;
        nc.UsuarioParecerRt = "rita";
        nc.DecisaoCq = StatusDecisaoCq.Desfavoravel;
        nc.Acoes.Add(new AcaoNaoConformidade { Descricao = "Ação", Responsavel = "resp", Prazo = new DateOnly(2026, 10, 1), Obrigatoria = true, DataConclusao = new DateOnly(2026, 9, 30), Evidencia = "ok" });
        await context.SaveChangesAsync();

        await CriarNc(context, "gq", Roles.GarantiaQualidade).AvaliarEficacia(nc.Id, eficaz: true);

        var atualizada = (await context.NaoConformidades.FindAsync(nc.Id))!;
        Assert.Equal(StatusNaoConformidade.AguardandoAprovacao, atualizada.Status);
        Assert.False(atualizada.ReprovadaRt);
        Assert.Null(atualizada.UsuarioParecerRt);
        Assert.Null(atualizada.DecisaoCq);
    }

    [Fact]
    public async Task NcAprovar_ComDecisaoFavoravelDoCq_NaoReabreADivergencia()
    {
        await using var context = CriarContexto();
        var nc = await AdicionarNcAsync(context, StatusNaoConformidade.AguardandoAprovacao, aprovadaRt: true);
        nc.ReprovadaGq = true;
        nc.DecisaoCq = StatusDecisaoCq.Favoravel;
        await context.SaveChangesAsync();

        await CriarNc(context, "rita", Roles.ResponsavelTecnico).Aprovar(nc.Id, "RT");

        Assert.Equal(StatusNaoConformidade.AguardandoAprovacao, (await context.NaoConformidades.FindAsync(nc.Id))!.Status);
    }

    [Fact]
    public async Task RecallEncerrar_ComDivergenciaResolvidaPeloCq_Encerra()
    {
        await using var context = CriarContexto();
        var recall = await AdicionarRecallAsync(context, StatusRecall.AguardandoEncerramento);
        recall.AprovadaRt = true;
        recall.ReprovadaGq = true;
        recall.DecisaoCq = StatusDecisaoCq.Favoravel;
        recall.ComunicadaAutoridadeEm = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        await CriarRecall(context, "gq", Roles.GarantiaQualidade).Encerrar(recall.Id);

        var atualizado = (await context.Recalls.FindAsync(recall.Id))!;
        Assert.Equal(StatusRecall.Encerrado, atualizado.Status);
        Assert.Equal("gq", atualizado.UsuarioEncerramento);
        Assert.NotNull(atualizado.EncerradaEm);
    }

    [Fact]
    public async Task RecallEncerrar_SemAprovacoesNemDecisaoDoCq_NaoEncerra()
    {
        await using var context = CriarContexto();
        var recall = await AdicionarRecallAsync(context, StatusRecall.AguardandoEncerramento);
        recall.AprovadaRt = true;
        recall.ComunicadaAutoridadeEm = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        await CriarRecall(context, "gq", Roles.GarantiaQualidade).Encerrar(recall.Id);

        Assert.Equal(StatusRecall.AguardandoEncerramento, (await context.Recalls.FindAsync(recall.Id))!.Status);
    }

    [Fact]
    public async Task RecallReabrir_LimpaPareceresDecisaoDoCqEEncerramento()
    {
        await using var context = CriarContexto();
        var recall = await AdicionarRecallAsync(context, StatusRecall.Encerrado);
        recall.ReprovadaRt = true;
        recall.AprovadaGq = true;
        recall.UsuarioParecerRt = "rita";
        recall.UsuarioParecerGq = "gabi";
        recall.DecisaoCq = StatusDecisaoCq.Favoravel;
        recall.JustificativaDecisaoCq = "Decisão anterior.";
        recall.UsuarioEncerramento = "gq";
        recall.EncerradaEm = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        await CriarRecall(context, "rt", Roles.ResponsavelTecnico).Reabrir(new RecallReaberturaViewModel
        {
            Id = recall.Id,
            Motivo = "Nova ocorrência identificada",
            Justificativa = "A rastreabilidade exige uma nova avaliação técnica."
        });

        var atualizado = (await context.Recalls.FindAsync(recall.Id))!;
        Assert.Equal(StatusRecall.EmAvaliacao, atualizado.Status);
        Assert.False(atualizado.ReprovadaRt);
        Assert.False(atualizado.AprovadaGq);
        Assert.Null(atualizado.UsuarioParecerRt);
        Assert.Null(atualizado.UsuarioParecerGq);
        Assert.Null(atualizado.DecisaoCq);
        Assert.Null(atualizado.JustificativaDecisaoCq);
        Assert.Null(atualizado.EncerradaEm);
        Assert.Null(atualizado.UsuarioEncerramento);
    }

    [Fact]
    public async Task RecallCreate_NaoAplicavel_RegistraResponsavelEDataDeEncerramento()
    {
        await using var context = CriarContexto();
        var produto = new Produto { Nome = "Produto de teste" };
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        var lote = new Lote { Numero = "L-1", ProdutoId = produto.Id };
        context.Lotes.Add(lote);
        await context.SaveChangesAsync();

        var resultado = await CriarRecall(context, "gq", Roles.GarantiaQualidade).Create(new RecallCreateViewModel
        {
            DataAbertura = new DateOnly(2026, 10, 8),
            ProdutoId = produto.Id,
            LoteId = lote.Id,
            ClientesEnvolvidos = "Cliente de teste",
            NaturezaOcorrencia = "Ocorrência de teste",
            RiscoPotencial = "Risco de teste",
            Decisao = DecisaoRecall.NaoAplicavel,
            JustificativaDecisao = "Sem risco à saúde, conforme avaliação documentada."
        });

        Assert.IsType<RedirectToActionResult>(resultado);
        var recall = await context.Recalls.SingleAsync();
        Assert.Equal(StatusRecall.Encerrado, recall.Status);
        Assert.Equal("gq", recall.UsuarioEncerramento);
        Assert.NotNull(recall.EncerradaEm);
    }

    // ---------- Reclamação ----------

    [Fact]
    public async Task RcClassificar_ReclamacaoEncerrada_NaoAlteraClassificacao()
    {
        await using var context = CriarContexto();
        var reclamacao = await AdicionarRcAsync(context, StatusReclamacao.Encerrada, ClassificacaoOcorrencia.Critica);

        await CriarRc(context, "gq", Roles.GarantiaQualidade).Classificar(reclamacao.Id, ClassificacaoOcorrencia.Menor);

        Assert.Equal(ClassificacaoOcorrencia.Critica, (await context.ReclamacoesClientes.FindAsync(reclamacao.Id))!.Classificacao);
    }

    [Fact]
    public async Task RcClassificar_SemValor_NaoAlteraClassificacao()
    {
        await using var context = CriarContexto();
        var reclamacao = await AdicionarRcAsync(context, StatusReclamacao.EmInvestigacao, ClassificacaoOcorrencia.Maior);

        await CriarRc(context, "gq", Roles.GarantiaQualidade).Classificar(reclamacao.Id, null);

        Assert.Equal(ClassificacaoOcorrencia.Maior, (await context.ReclamacoesClientes.FindAsync(reclamacao.Id))!.Classificacao);
    }

    [Fact]
    public async Task RcClassificar_EmAndamento_AtualizaClassificacao()
    {
        await using var context = CriarContexto();
        var reclamacao = await AdicionarRcAsync(context, StatusReclamacao.EmInvestigacao, ClassificacaoOcorrencia.Maior);

        await CriarRc(context, "gq", Roles.GarantiaQualidade).Classificar(reclamacao.Id, ClassificacaoOcorrencia.Critica);

        Assert.Equal(ClassificacaoOcorrencia.Critica, (await context.ReclamacoesClientes.FindAsync(reclamacao.Id))!.Classificacao);
    }

    [Fact]
    public async Task RcValidar_SemClassificacao_NaoValidaNemCriaNc()
    {
        await using var context = CriarContexto();
        var reclamacao = await AdicionarRcAsync(context, StatusReclamacao.AguardandoValidacaoGq, null);

        await CriarRc(context, "gq", Roles.GarantiaQualidade).Validar(reclamacao.Id, null);

        Assert.Equal(StatusReclamacao.AguardandoValidacaoGq, (await context.ReclamacoesClientes.FindAsync(reclamacao.Id))!.Status);
        Assert.Empty(await context.NaoConformidades.ToListAsync());
    }

    [Fact]
    public async Task RcReabrir_LimpaCarimboDeEncerramento()
    {
        await using var context = CriarContexto();
        var reclamacao = await AdicionarRcAsync(context, StatusReclamacao.Encerrada, ClassificacaoOcorrencia.Maior);
        reclamacao.UsuarioEncerramento = "gq";
        reclamacao.EncerradaEm = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        await CriarRc(context, "gq", Roles.GarantiaQualidade).Reabrir(new ReclamacaoReaberturaViewModel
        {
            Id = reclamacao.Id,
            Justificativa = "Nova informação do cliente exige reanálise."
        });

        var atualizada = (await context.ReclamacoesClientes.FindAsync(reclamacao.Id))!;
        Assert.Equal(StatusReclamacao.EmInvestigacao, atualizada.Status);
        Assert.Null(atualizada.EncerradaEm);
        Assert.Null(atualizada.UsuarioEncerramento);
    }

    // ---------- Auditoria e anexos ----------

    [Fact]
    public async Task Auditoria_NaoGravaHashDeSenhaNemCarimbosDeSeguranca()
    {
        await using var context = CriarContexto();
        context.Users.Add(new ApplicationUser
        {
            UserName = "maria",
            Email = "maria@sgq.test",
            PasswordHash = "HASH-SECRETO-DE-SENHA",
            SecurityStamp = "STAMP-SECRETO"
        });
        await context.SaveChangesAsync();

        var registros = await context.HistoricosAuditoria.Where(item => item.Entidade == nameof(ApplicationUser)).ToListAsync();

        Assert.NotEmpty(registros);
        Assert.All(registros, registro =>
        {
            Assert.DoesNotContain("HASH-SECRETO-DE-SENHA", registro.Alteracoes);
            Assert.DoesNotContain("STAMP-SECRETO", registro.Alteracoes);
            Assert.DoesNotContain("PasswordHash", registro.Alteracoes);
        });
    }

    [Theory]
    [InlineData(Roles.ControleQualidade, false)]
    [InlineData(Roles.ResponsavelTecnico, false)]
    [InlineData(Roles.GarantiaQualidade, true)]
    [InlineData(Roles.Auditor, true)]
    [InlineData(Roles.Administrador, true)]
    public async Task Baixar_AnexoAnulado_SoParaGestaoEAuditoria(string perfil, bool permitido)
    {
        var raiz = Path.Combine(Path.GetTempPath(), "sgq-teste-" + Guid.NewGuid().ToString("N"));
        var pasta = Path.Combine(raiz, "App_Data", "uploads");
        Directory.CreateDirectory(pasta);
        try
        {
            await File.WriteAllTextAsync(Path.Combine(pasta, "arquivo.pdf"), "conteudo");
            await using var context = CriarContexto();
            var anexo = new Anexo { NomeOriginal = "evidencia.pdf", NomeArmazenado = "arquivo.pdf", TipoConteudo = "application/pdf", TamanhoBytes = 8, Usuario = "gq", EnviadoEm = DateTimeOffset.UtcNow, Ativo = false };
            context.Anexos.Add(anexo);
            await context.SaveChangesAsync();
            var controller = CriarAnexos(context, raiz, "usuario", perfil);

            var resultado = await controller.Baixar(anexo.Id);

            if (permitido) Assert.IsType<FileContentResult>(resultado);
            else Assert.IsType<NotFoundResult>(resultado);
        }
        finally
        {
            Directory.Delete(raiz, recursive: true);
        }
    }

    [Fact]
    public async Task Baixar_AnexoAtivo_EstaDisponivelParaQualquerPerfilReconhecido()
    {
        var raiz = Path.Combine(Path.GetTempPath(), "sgq-teste-" + Guid.NewGuid().ToString("N"));
        var pasta = Path.Combine(raiz, "App_Data", "uploads");
        Directory.CreateDirectory(pasta);
        try
        {
            await File.WriteAllTextAsync(Path.Combine(pasta, "arquivo.pdf"), "conteudo");
            await using var context = CriarContexto();
            var anexo = new Anexo { NomeOriginal = "evidencia.pdf", NomeArmazenado = "arquivo.pdf", TipoConteudo = "application/pdf", TamanhoBytes = 8, Usuario = "gq", EnviadoEm = DateTimeOffset.UtcNow };
            context.Anexos.Add(anexo);
            await context.SaveChangesAsync();

            var resultado = await CriarAnexos(context, raiz, "cq", Roles.ControleQualidade).Baixar(anexo.Id);

            Assert.IsType<FileContentResult>(resultado);
        }
        finally
        {
            Directory.Delete(raiz, recursive: true);
        }
    }

    // ---------- Apoio ----------

    private static async Task<NaoConformidade> AdicionarNcAsync(ApplicationDbContext context, StatusNaoConformidade status, bool aprovadaRt = false, bool aprovadaGq = false)
    {
        var nc = new NaoConformidade
        {
            Codigo = "NC-2026-000001",
            DataAbertura = new DateOnly(2026, 10, 8),
            Area = "Qualidade",
            UsuarioAbertura = "gq",
            Status = status,
            AprovadaRt = aprovadaRt,
            AprovadaGq = aprovadaGq
        };
        context.NaoConformidades.Add(nc);
        await context.SaveChangesAsync();
        return nc;
    }

    private static async Task<Recall> AdicionarRecallAsync(ApplicationDbContext context, StatusRecall status)
    {
        var recall = new Recall
        {
            Codigo = "REC-2026-000001",
            DataAbertura = new DateOnly(2026, 10, 8),
            ProdutoId = 1,
            LoteId = 1,
            ClientesEnvolvidos = "Cliente de teste",
            NaturezaOcorrencia = "Ocorrência de teste",
            RiscoPotencial = "Risco de teste",
            JustificativaDecisao = "Justificativa de teste",
            UsuarioAbertura = "gq",
            Status = status
        };
        context.Recalls.Add(recall);
        await context.SaveChangesAsync();
        return recall;
    }

    private static async Task<ReclamacaoCliente> AdicionarRcAsync(ApplicationDbContext context, StatusReclamacao status, ClassificacaoOcorrencia? classificacao)
    {
        var cliente = new Cliente { Nome = "Cliente de teste", Contato = "contato@cliente.test" };
        var produto = new Produto { Nome = "Produto de teste" };
        context.AddRange(cliente, produto);
        await context.SaveChangesAsync();
        var reclamacao = new ReclamacaoCliente
        {
            Codigo = "RC-2026-000001",
            Ano = 2026,
            SequenciaAnual = 1,
            DataRecebimento = new DateOnly(2026, 10, 1),
            ClienteId = cliente.Id,
            ProdutoId = produto.Id,
            Descricao = "Descrição de teste",
            UsuarioAbertura = "gq",
            Status = status,
            Classificacao = classificacao
        };
        context.ReclamacoesClientes.Add(reclamacao);
        await context.SaveChangesAsync();
        return reclamacao;
    }

    private static ControllerContext ContextoDe(string usuario, string[] perfis)
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, usuario) };
        claims.AddRange(perfis.Select(perfil => new Claim(ClaimTypes.Role, perfil)));
        return new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Teste")) } };
    }

    private static NaoConformidadesController CriarNc(ApplicationDbContext context, string usuario, params string[] perfis)
    {
        var controllerContext = ContextoDe(usuario, perfis);
        return new NaoConformidadesController(context, new PrazoServiceFalso())
        {
            ControllerContext = controllerContext,
            TempData = new TempDataDictionary(controllerContext.HttpContext, new MemoriaTempDataProvider())
        };
    }

    private static RecallsController CriarRecall(ApplicationDbContext context, string usuario, params string[] perfis)
    {
        var controllerContext = ContextoDe(usuario, perfis);
        return new RecallsController(context)
        {
            ControllerContext = controllerContext,
            TempData = new TempDataDictionary(controllerContext.HttpContext, new MemoriaTempDataProvider())
        };
    }

    private static ReclamacoesController CriarRc(ApplicationDbContext context, string usuario, params string[] perfis)
    {
        var controllerContext = ContextoDe(usuario, perfis);
        return new ReclamacoesController(context, new PrazoServiceFalso())
        {
            ControllerContext = controllerContext,
            TempData = new TempDataDictionary(controllerContext.HttpContext, new MemoriaTempDataProvider())
        };
    }

    private static AnexosController CriarAnexos(ApplicationDbContext context, string raiz, string usuario, params string[] perfis)
    {
        var controllerContext = ContextoDe(usuario, perfis);
        return new AnexosController(context, new AmbienteWebTeste { ContentRootPath = raiz })
        {
            ControllerContext = controllerContext,
            TempData = new TempDataDictionary(controllerContext.HttpContext, new MemoriaTempDataProvider())
        };
    }

    private static ApplicationDbContext CriarContexto() => new(
        new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).ConfigureWarnings(avisos => avisos.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options,
        new HttpContextAccessor());

    private sealed class PrazoServiceFalso : IPrazoService
    {
        public Task<DateOnly> AdicionarDiasUteisAsync(DateOnly dataInicial, int diasUteis, CancellationToken cancellationToken = default) => Task.FromResult(dataInicial);
        public Task<DateOnly> CalcularPrazoReclamacaoAsync(DateOnly dataInformacoesCompletas, CancellationToken cancellationToken = default) => Task.FromResult(dataInformacoesCompletas);
        public Task<DateOnly?> CalcularPrazoNaoConformidadeAsync(DateOnly dataAbertura, ClassificacaoOcorrencia classificacao, CancellationToken cancellationToken = default) => Task.FromResult<DateOnly?>(dataAbertura);
    }

    private sealed class MemoriaTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }

    private sealed class AmbienteWebTeste : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "SGQ.Web.Tests";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = Path.GetTempPath();
        public string EnvironmentName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
