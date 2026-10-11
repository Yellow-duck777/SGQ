using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGQ.Domain.Entities;
using SGQ.Domain.Enums;
using SGQ.Web.Controllers;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Presentation;
using SGQ.Web.Security;
using SGQ.Web.Services;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Tests;

public class HomeControllerTests
{
    private static readonly DateOnly Hoje = DateOnly.FromDateTime(DateTime.Today);

    [Fact]
    public async Task Index_ParaGq_MostraValidacaoDeRcEAprovacoesPendentesDeGq()
    {
        await using var context = await CriarContextoComDadosAsync();

        var model = await ObterModeloAsync(context, "maria.silva@empresa.com", "GQ");

        Assert.Equal("Maria", model.NomeUsuario);
        Assert.Equal(["Garantia da Qualidade"], model.Perfis);
        var validacao = Assert.Single(model.Atencao, item => item.Status == nameof(StatusReclamacao.AguardandoValidacaoGq));
        Assert.Equal(2, validacao.Contagem);
        Assert.Equal("Reclamacoes", validacao.Controller);
        // A NC que a GQ já aprovou não aparece como pendência dela; a outra sim.
        var aprovacaoNc = Assert.Single(model.Atencao, item => item.Controller == "NaoConformidades" && item.Status == nameof(StatusNaoConformidade.AguardandoAprovacao));
        Assert.Equal(1, aprovacaoNc.Contagem);
        Assert.DoesNotContain(model.Atencao, item => item.Status == nameof(StatusNaoConformidade.AguardandoDecisaoCq));
    }

    [Fact]
    public async Task Index_ParaRt_MostraApenasAprovacoesPendentesDeRt()
    {
        await using var context = await CriarContextoComDadosAsync();

        var model = await ObterModeloAsync(context, "rt@empresa.com", "RT");

        Assert.DoesNotContain(model.Atencao, item => item.Controller == "Reclamacoes" && item.Status is not null);
        var nc = Assert.Single(model.Atencao, item => item.Controller == "NaoConformidades" && item.Status == nameof(StatusNaoConformidade.AguardandoAprovacao));
        Assert.Equal(2, nc.Contagem);
        var recall = Assert.Single(model.Atencao, item => item.Controller == "Recalls" && item.Status == nameof(StatusRecall.AguardandoAprovacao));
        Assert.Equal(1, recall.Contagem);
    }

    [Fact]
    public async Task Index_ParaCq_MostraDecisoesDoCq()
    {
        await using var context = await CriarContextoComDadosAsync();

        var model = await ObterModeloAsync(context, "cq@empresa.com", "CQ");

        Assert.Equal(1, Assert.Single(model.Atencao, item => item.Status == nameof(StatusNaoConformidade.AguardandoDecisaoCq) && item.Controller == "NaoConformidades").Contagem);
        Assert.Equal(1, Assert.Single(model.Atencao, item => item.Controller == "Recalls" && item.Status == nameof(StatusRecall.AguardandoDecisaoCq)).Contagem);
        Assert.DoesNotContain(model.Atencao, item => item.Status == nameof(StatusReclamacao.AguardandoValidacaoGq));
    }

    [Fact]
    public async Task Index_ParaAuditor_MostraSomenteProcessosVencidos()
    {
        await using var context = await CriarContextoComDadosAsync();

        var model = await ObterModeloAsync(context, "auditor@empresa.com", "Auditor");

        Assert.NotEmpty(model.Atencao);
        Assert.All(model.Atencao, item => { Assert.True(item.Critico); Assert.Null(item.Status); });
    }

    [Fact]
    public async Task Index_ParaAdministrador_VeTodasAsPendencias()
    {
        await using var context = await CriarContextoComDadosAsync();

        var model = await ObterModeloAsync(context, "admin@empresa.com", "Administrador");

        Assert.Equal(2, model.Atencao.Single(item => item.Status == nameof(StatusReclamacao.AguardandoValidacaoGq)).Contagem);
        Assert.Equal(2, model.Atencao.Single(item => item.Controller == "NaoConformidades" && item.Status == nameof(StatusNaoConformidade.AguardandoAprovacao)).Contagem);
        Assert.Contains(model.Atencao, item => item.Status == nameof(StatusRecall.AguardandoDecisaoCq));
    }

    [Fact]
    public async Task Index_CalculaResumoDosTresProcessos()
    {
        await using var context = await CriarContextoComDadosAsync();

        var model = await ObterModeloAsync(context, "gq@empresa.com", "GQ");

        // RC: 2 aguardando validação + 1 vencida em aberto + 1 encerrada (não conta).
        Assert.Equal(3, model.ResumoReclamacoes.Abertos);
        Assert.Equal(1, model.ResumoReclamacoes.Vencidos);
        Assert.Equal(3, model.ResumoNaoConformidades.Abertos);
        Assert.Equal(2, model.ResumoRecalls.Abertos);
        Assert.Equal(1, model.ResumoRecalls.Vencidos);
        Assert.Equal(model.ResumoReclamacoes.Vencidos + model.ResumoNaoConformidades.Vencidos + model.ResumoRecalls.Vencidos, model.ProcessosVencidos);
        Assert.Equal(model.ResumoRecalls.Abertos, model.RecallsAtivos);
    }

    [Fact]
    public async Task Index_ListaMovimentosDosTresTiposMaisRecentesPrimeiro()
    {
        await using var context = await CriarContextoComDadosAsync();

        var model = await ObterModeloAsync(context, "gq@empresa.com", "GQ");

        Assert.Contains(model.Movimentos, item => item.Sigla == "RC");
        Assert.Contains(model.Movimentos, item => item.Sigla == "NC");
        Assert.Contains(model.Movimentos, item => item.Sigla == "RE");
        Assert.Equal(model.Movimentos.OrderByDescending(item => item.CriadaEm), model.Movimentos);
        Assert.True(model.Movimentos.Count <= 8);
    }

    [Fact]
    public async Task Index_SemPerfilReconhecido_NegaAcesso()
    {
        await using var context = CriarContexto();
        var controller = CriarController(context, "novo@empresa.com");

        Assert.IsType<ForbidResult>(await controller.Index());
    }

    private static async Task<DashboardViewModel> ObterModeloAsync(ApplicationDbContext context, string usuario, params string[] perfis)
    {
        var resultado = await CriarController(context, usuario, perfis).Index();
        return Assert.IsType<DashboardViewModel>(Assert.IsType<ViewResult>(resultado).Model);
    }

    private static async Task<ApplicationDbContext> CriarContextoComDadosAsync()
    {
        var context = CriarContexto();
        var cliente = new Cliente { Nome = "Cliente Teste", Contato = "contato@teste.local" };
        var produto = new Produto { Nome = "Produto Teste" };
        var lote = new Lote { Numero = "L-1", Produto = produto };
        context.AddRange(cliente, produto, lote);

        var agora = DateTimeOffset.UtcNow;
        context.ReclamacoesClientes.AddRange(
            NovaRc("RC-1", StatusReclamacao.AguardandoValidacaoGq, cliente, produto, Hoje.AddDays(5), agora.AddMinutes(-1)),
            NovaRc("RC-2", StatusReclamacao.AguardandoValidacaoGq, cliente, produto, Hoje.AddDays(9), agora.AddMinutes(-2)),
            NovaRc("RC-3", StatusReclamacao.EmInvestigacao, cliente, produto, Hoje.AddDays(-3), agora.AddMinutes(-3)),
            NovaRc("RC-4", StatusReclamacao.Encerrada, cliente, produto, Hoje.AddDays(-10), agora.AddMinutes(-4)));

        context.NaoConformidades.AddRange(
            NovaNc("NC-1", StatusNaoConformidade.AguardandoAprovacao, produto, aprovadaGq: true, aprovadaRt: false, agora.AddMinutes(-5)),
            NovaNc("NC-2", StatusNaoConformidade.AguardandoAprovacao, produto, aprovadaGq: false, aprovadaRt: false, agora.AddMinutes(-6)),
            NovaNc("NC-3", StatusNaoConformidade.AguardandoDecisaoCq, produto, aprovadaGq: true, aprovadaRt: true, agora.AddMinutes(-7)),
            NovaNc("NC-4", StatusNaoConformidade.Encerrada, produto, aprovadaGq: true, aprovadaRt: true, agora.AddMinutes(-8)));

        context.Recalls.AddRange(
            NovoRecall("RE-1", StatusRecall.AguardandoAprovacao, produto, lote, aprovadaGq: true, aprovadaRt: false, Hoje.AddDays(-1), agora.AddSeconds(-30)),
            NovoRecall("RE-2", StatusRecall.AguardandoDecisaoCq, produto, lote, aprovadaGq: true, aprovadaRt: true, Hoje.AddDays(7), agora.AddSeconds(-90)),
            NovoRecall("RE-3", StatusRecall.Encerrado, produto, lote, aprovadaGq: true, aprovadaRt: true, Hoje.AddDays(-30), agora.AddSeconds(-150)));

        await context.SaveChangesAsync();
        return context;
    }

    private static ReclamacaoCliente NovaRc(string codigo, StatusReclamacao status, Cliente cliente, Produto produto, DateOnly prazo, DateTimeOffset criada) => new()
    {
        Codigo = codigo, Status = status, Cliente = cliente, Produto = produto, DataAlvo = prazo, CriadaEm = criada,
        DataRecebimento = Hoje, CanalRecebimento = "Telefone", ContatoCliente = "Contato", Descricao = "Descrição", UsuarioAbertura = "teste"
    };

    private static NaoConformidade NovaNc(string codigo, StatusNaoConformidade status, Produto produto, bool aprovadaGq, bool aprovadaRt, DateTimeOffset criada) => new()
    {
        Codigo = codigo, Status = status, Produto = produto, AprovadaGq = aprovadaGq, AprovadaRt = aprovadaRt, CriadaEm = criada,
        Area = "Produção", DataAbertura = Hoje, DataAlvo = Hoje.AddDays(10), UsuarioAbertura = "teste"
    };

    private static Recall NovoRecall(string codigo, StatusRecall status, Produto produto, Lote lote, bool aprovadaGq, bool aprovadaRt, DateOnly prazo, DateTimeOffset criada) => new()
    {
        Codigo = codigo, Status = status, Produto = produto, Lote = lote, AprovadaGq = aprovadaGq, AprovadaRt = aprovadaRt, DataAlvo = prazo,
        CriadaEm = criada, DataAbertura = Hoje, UsuarioAbertura = "teste"
    };

    private static HomeController CriarController(ApplicationDbContext context, string usuario, params string[] perfis)
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, usuario) };
        claims.AddRange(perfis.Select(perfil => new Claim(ClaimTypes.Role, perfil)));
        return new HomeController(context, new PrazoServiceFalso())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Teste")) } }
        };
    }

    private static ApplicationDbContext CriarContexto() => new(
        new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
        new HttpContextAccessor());

    private sealed class PrazoServiceFalso : IPrazoService
    {
        public Task<DateOnly> AdicionarDiasUteisAsync(DateOnly dataInicial, int diasUteis, CancellationToken cancellationToken = default) => Task.FromResult(dataInicial.AddDays(diasUteis));
        public Task<DateOnly> CalcularPrazoReclamacaoAsync(DateOnly dataInformacoesCompletas, CancellationToken cancellationToken = default) => Task.FromResult(dataInformacoesCompletas);
        public Task<DateOnly?> CalcularPrazoNaoConformidadeAsync(DateOnly dataAbertura, ClassificacaoOcorrencia classificacao, CancellationToken cancellationToken = default) => Task.FromResult<DateOnly?>(dataAbertura);
    }
}

public class ApresentacaoDeUsuarioTests
{
    [Theory]
    [InlineData("maria.silva@empresa.com", "Maria Silva")]
    [InlineData("JOAO_PEREIRA@x.com", "Joao Pereira")]
    [InlineData("gq@sgq.test", "Gq")]
    [InlineData("", "Usuário")]
    [InlineData(null, "Usuário")]
    public void NomeExibicao_DerivaDoEmail(string? usuario, string esperado) => Assert.Equal(esperado, UsuarioApresentacao.NomeExibicao(usuario));

    [Fact]
    public void PerfisDoUsuario_TraduzEMantemOrdemOficial()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Role, Roles.ResponsavelTecnico), new Claim(ClaimTypes.Role, Roles.GarantiaQualidade)], "Teste"));

        Assert.Equal(["Garantia da Qualidade", "Responsável Técnico"], UsuarioApresentacao.PerfisDoUsuario(principal));
    }

    [Fact]
    public void PoliticaSenha_DescreveRequisitosConfigurados()
    {
        var itens = PoliticaSenhaTexto.Descrever(new PasswordOptions { RequiredLength = 8, RequireDigit = true, RequireUppercase = false, RequireLowercase = true, RequireNonAlphanumeric = false });

        Assert.Equal(["Pelo menos 8 caracteres", "Uma letra minúscula", "Um número"], itens);
    }

    [Fact]
    public void ErrosDoIdentity_VemEmPortugues()
    {
        var descritor = new PortugueseIdentityErrorDescriber();

        Assert.Equal("A senha deve ter pelo menos 6 caracteres.", descritor.PasswordTooShort(6).Description);
        Assert.Contains("maiúscula", descritor.PasswordRequiresUpper().Description);
        Assert.Contains("já está cadastrado", descritor.DuplicateEmail("a@b.c").Description);
    }
}
