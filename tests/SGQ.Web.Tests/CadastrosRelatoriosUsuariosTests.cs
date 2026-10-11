using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SGQ.Domain.Entities;
using SGQ.Domain.Enums;
using SGQ.Web.Controllers;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Security;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Tests;

public class CadastrosRelatoriosUsuariosTests
{
    [Fact]
    public async Task Relatorio_UsaRotulosLegiveisEmVezDeNomesDeEnumeracao()
    {
        await using var context = CriarContexto();
        await SemearProcessosAsync(context);
        var controller = new RelatoriosController(context);

        var resultado = Assert.IsType<ViewResult>(await controller.Index(null, null, null, null));
        var model = Assert.IsType<RelatorioProcessosViewModel>(resultado.Model);

        var rc = Assert.Single(model.Itens, item => item.Tipo == "RC");
        Assert.Equal("Aguardando validação da GQ", rc.Situacao);
        Assert.Equal("Crítica", rc.Classificacao);
        Assert.Equal("perigo", rc.ClassificacaoTom);
        var recall = Assert.Single(model.Itens, item => item.Tipo == "Recall");
        Assert.Equal("Aplicável", recall.Classificacao);
        Assert.Equal("Reclamação de cliente", recall.AreaOuCliente);
        Assert.All(model.Itens, item =>
        {
            Assert.DoesNotContain("AguardandoValidacaoGq", item.Situacao);
            Assert.DoesNotContain("NaoAplicavel", item.Classificacao ?? string.Empty);
        });
    }

    [Fact]
    public async Task ExportarCsv_GravaRotulosLegiveis()
    {
        await using var context = CriarContexto();
        await SemearProcessosAsync(context);
        var controller = new RelatoriosController(context);

        var arquivo = Assert.IsType<FileContentResult>(await controller.ExportarCsv(null, null, null, null));
        var csv = Encoding.UTF8.GetString(arquivo.FileContents);

        Assert.Contains("Aguardando validação da GQ", csv);
        Assert.Contains("Crítica", csv);
        Assert.Contains("Aplicável", csv);
        Assert.Contains("Reclamação de cliente", csv);
        Assert.DoesNotContain("AguardandoValidacaoGq", csv);
        Assert.DoesNotContain("Critica", csv);
        Assert.DoesNotContain("ReclamacaoCliente", csv);
    }

    [Fact]
    public async Task Lotes_Index_IndicaLotesEmUsoPorRecalls()
    {
        await using var context = CriarContexto();
        await SemearProcessosAsync(context);
        var livre = new Lote { Numero = "LIVRE-1", ProdutoId = context.Produtos.First().Id };
        context.Lotes.Add(livre);
        await context.SaveChangesAsync();
        var controller = new LotesController(context);

        var resultado = Assert.IsType<ViewResult>(await controller.Index());

        var emUso = Assert.IsType<HashSet<int>>(controller.ViewBag.LotesEmUso);
        var usado = context.Lotes.First(lote => lote.Numero == "L-001");
        Assert.Contains(usado.Id, emUso);
        Assert.DoesNotContain(livre.Id, emUso);
        Assert.Equal(2, ((IEnumerable<Lote>)resultado.Model!).Count());
    }

    [Fact]
    public async Task Lotes_Create_IgnoraErroImplicitoDaNavegacaoProdutoEValidaExistenciaDoProduto()
    {
        await using var context = CriarContexto();
        var produto = new Produto { Nome = "Produto" };
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        var controller = CriarLotesController(context);
        // Reproduz o que o MVC faz ao tratar a propriedade de navegação não anulável como obrigatória.
        controller.ModelState.AddModelError(nameof(Lote.Produto), "The Produto field is required.");

        var valido = await controller.Create(new Lote { Numero = "L-9", ProdutoId = produto.Id });

        Assert.IsType<RedirectToActionResult>(valido);
        Assert.Single(context.Lotes);

        var invalido = CriarLotesController(context);
        var resposta = await invalido.Create(new Lote { Numero = "L-10", ProdutoId = produto.Id + 99 });
        Assert.IsType<ViewResult>(resposta);
        Assert.True(invalido.ModelState.ContainsKey(nameof(Lote.ProdutoId)));
        Assert.Single(context.Lotes);
    }

    private static LotesController CriarLotesController(ApplicationDbContext context)
    {
        var httpContext = new DefaultHttpContext();
        return new LotesController(context)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new MemoriaTempDataProvider())
        };
    }

    [Fact]
    public async Task Usuarios_NaoPermiteRemoverPerfilDoUnicoAdministrador()
    {
        await using var context = CriarContexto();
        var (controller, userManager) = await CriarUsuariosControllerAsync(context);
        var admin = await CriarUsuarioAsync(userManager, "admin@teste", Roles.Administrador);

        var resultado = await controller.AtualizarPerfis(admin.Id, [Roles.GarantiaQualidade]);

        Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Contains(Roles.Administrador, await userManager.GetRolesAsync(admin));
        Assert.Contains("único Administrador", (string)controller.TempData["Error"]!);
    }

    [Fact]
    public async Task Usuarios_PermiteRemoverAdministradorQuandoHaOutro()
    {
        await using var context = CriarContexto();
        var (controller, userManager) = await CriarUsuariosControllerAsync(context);
        var admin1 = await CriarUsuarioAsync(userManager, "admin1@teste", Roles.Administrador);
        await CriarUsuarioAsync(userManager, "admin2@teste", Roles.Administrador);

        await controller.AtualizarPerfis(admin1.Id, [Roles.Auditor]);

        var perfis = await userManager.GetRolesAsync(admin1);
        Assert.Equal([Roles.Auditor], perfis);
    }

    [Fact]
    public async Task Usuarios_AtribuiPerfilAContaSemPerfil()
    {
        await using var context = CriarContexto();
        var (controller, userManager) = await CriarUsuariosControllerAsync(context);
        var novo = await CriarUsuarioAsync(userManager, "novo@teste");

        await controller.AtualizarPerfis(novo.Id, [Roles.ControleQualidade, "PerfilInexistente"]);

        Assert.Equal([Roles.ControleQualidade], await userManager.GetRolesAsync(novo));
        var lista = Assert.IsType<ViewResult>(await controller.Index());
        var usuario = Assert.Single((IEnumerable<UsuarioPerfilViewModel>)lista.Model!, item => item.Email == "novo@teste");
        Assert.False(usuario.Bloqueado);
    }

    [Fact]
    public async Task Usuarios_Create_CriaContaComPerfil()
    {
        await using var context = CriarContexto();
        var (controller, userManager) = await CriarUsuariosControllerAsync(context);
        var model = new UsuarioCreateViewModel
        {
            Email = "gq@teste",
            Password = "Senha-Forte-123!",
            ConfirmPassword = "Senha-Forte-123!",
            Perfis = [Roles.GarantiaQualidade]
        };

        var resultado = await controller.Create(model);

        Assert.IsType<RedirectToActionResult>(resultado);
        var usuario = await userManager.FindByEmailAsync("gq@teste");
        Assert.NotNull(usuario);
        Assert.Contains(Roles.GarantiaQualidade, await userManager.GetRolesAsync(usuario));
        Assert.True(await userManager.CheckPasswordAsync(usuario, "Senha-Forte-123!"));
    }

    [Fact]
    public async Task Usuarios_Create_RejeitaContaSemPerfil()
    {
        await using var context = CriarContexto();
        var (controller, userManager) = await CriarUsuariosControllerAsync(context);
        var model = new UsuarioCreateViewModel
        {
            Email = "sem-perfil@teste",
            Password = "Senha-Forte-123!",
            ConfirmPassword = "Senha-Forte-123!"
        };

        var resultado = await controller.Create(model);

        Assert.IsType<ViewResult>(resultado);
        Assert.Null(await userManager.FindByEmailAsync("sem-perfil@teste"));
        Assert.True(controller.ModelState.ContainsKey(nameof(model.Perfis)));
    }

    [Fact]
    public async Task Usuarios_RedefinirSenha_AlteraSenhaDaConta()
    {
        await using var context = CriarContexto();
        var (controller, userManager) = await CriarUsuariosControllerAsync(context);
        var usuario = await CriarUsuarioAsync(userManager, "usuario@teste", "Senha-Antiga-123!", Roles.Auditor);

        var resultado = await controller.RedefinirSenha(new RedefinirSenhaUsuarioViewModel
        {
            Id = usuario.Id,
            Password = "Senha-Nova-123!",
            ConfirmPassword = "Senha-Nova-123!"
        });

        Assert.IsType<RedirectToActionResult>(resultado);
        Assert.True(await userManager.CheckPasswordAsync(usuario, "Senha-Nova-123!"));
        Assert.False(await userManager.CheckPasswordAsync(usuario, "Senha-Antiga-123!"));
    }

    [Fact]
    public async Task Usuarios_AlternarBloqueio_BloqueiaEDesbloqueiaConta()
    {
        await using var context = CriarContexto();
        var (controller, userManager) = await CriarUsuariosControllerAsync(context);
        await CriarUsuarioAsync(userManager, "admin@teste", Roles.Administrador);
        var usuario = await CriarUsuarioAsync(userManager, "auditor@teste", Roles.Auditor);

        await controller.AlternarBloqueio(usuario.Id);

        usuario = (await userManager.FindByIdAsync(usuario.Id))!;
        Assert.True(usuario.LockoutEnd > DateTimeOffset.UtcNow);

        await controller.AlternarBloqueio(usuario.Id);

        usuario = (await userManager.FindByIdAsync(usuario.Id))!;
        Assert.Null(usuario.LockoutEnd);
    }

    [Fact]
    public async Task Usuarios_AlternarBloqueio_NaoBloqueiaUnicoAdministradorAtivo()
    {
        await using var context = CriarContexto();
        var (controller, userManager) = await CriarUsuariosControllerAsync(context);
        var administrador = await CriarUsuarioAsync(userManager, "admin@teste", Roles.Administrador);

        var resultado = await controller.AlternarBloqueio(administrador.Id);

        Assert.IsType<RedirectToActionResult>(resultado);
        administrador = (await userManager.FindByIdAsync(administrador.Id))!;
        Assert.Null(administrador.LockoutEnd);
        Assert.Contains("única conta Administrador ativa", (string)controller.TempData["Error"]!);
    }

    private static async Task SemearProcessosAsync(ApplicationDbContext context)
    {
        var cliente = new Cliente { Nome = "Cliente Teste", Contato = "contato@teste" };
        var produto = new Produto { Nome = "Produto Teste" };
        var lote = new Lote { Numero = "L-001", Produto = produto };
        context.AddRange(cliente, produto, lote);
        await context.SaveChangesAsync();
        context.ReclamacoesClientes.Add(new ReclamacaoCliente
        {
            Codigo = "RC-2026-000001", DataRecebimento = new DateOnly(2026, 9, 1), CanalRecebimento = "E-mail", Cliente = cliente,
            ContatoCliente = "Contato", Produto = produto, Descricao = "Teste", UsuarioAbertura = "gq",
            Status = StatusReclamacao.AguardandoValidacaoGq, Classificacao = ClassificacaoOcorrencia.Critica
        });
        context.Recalls.Add(new Recall
        {
            Codigo = "REC-2026-000001", Origem = OrigemRecall.ReclamacaoCliente, Produto = produto, Lote = lote,
            DataAbertura = new DateOnly(2026, 9, 2), ClientesEnvolvidos = "x", NaturezaOcorrencia = "x", RiscoPotencial = "x",
            Decisao = DecisaoRecall.Aplicavel, JustificativaDecisao = "x", Status = StatusRecall.EmAvaliacao, UsuarioAbertura = "gq"
        });
        await context.SaveChangesAsync();
    }

    private static async Task<(UsuariosController Controller, UserManager<ApplicationUser> UserManager)> CriarUsuariosControllerAsync(ApplicationDbContext context)
    {
        var store = new UserStore<ApplicationUser, IdentityRole, ApplicationDbContext>(context);
        var tokenProviders = new Dictionary<string, IUserTwoFactorTokenProvider<ApplicationUser>>
        {
            [TokenOptions.DefaultProvider] = new TokenProvider()
        };
        var userManager = new UserManager<ApplicationUser>(store, Options.Create(new IdentityOptions()), new PasswordHasher<ApplicationUser>(), [], [],
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), tokenProviders, NullLogger<UserManager<ApplicationUser>>.Instance);
        foreach (var perfil in Roles.Todos)
            context.Roles.Add(new IdentityRole(perfil) { NormalizedName = perfil.ToUpperInvariant() });
        await context.SaveChangesAsync();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "admin"), new Claim(ClaimTypes.Role, Roles.Administrador)], "Teste"))
        };
        var controller = new UsuariosController(userManager)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new MemoriaTempDataProvider())
        };
        return (controller, userManager);
    }

    private static async Task<ApplicationUser> CriarUsuarioAsync(UserManager<ApplicationUser> userManager, string email, params string[] perfis) =>
        await CriarUsuarioAsync(userManager, email, null, perfis);

    private static async Task<ApplicationUser> CriarUsuarioAsync(UserManager<ApplicationUser> userManager, string email, string? senha, params string[] perfis)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var resultado = senha is null ? await userManager.CreateAsync(user) : await userManager.CreateAsync(user, senha);
        Assert.True(resultado.Succeeded, string.Join("; ", resultado.Errors.Select(erro => erro.Description)));
        if (perfis.Length > 0) Assert.True((await userManager.AddToRolesAsync(user, perfis)).Succeeded);
        return user;
    }

    private static ApplicationDbContext CriarContexto() => new(
        new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
        new HttpContextAccessor());

    private sealed class MemoriaTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }

    private sealed class TokenProvider : IUserTwoFactorTokenProvider<ApplicationUser>
    {
        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user) => Task.FromResult(true);
        public Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user) => Task.FromResult("token-de-teste");
        public Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user) => Task.FromResult(token == "token-de-teste");
    }
}
