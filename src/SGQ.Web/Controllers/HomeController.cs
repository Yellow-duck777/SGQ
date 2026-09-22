using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Controllers;

public class HomeController(ApplicationDbContext context) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var model = new DashboardViewModel
        {
            ReclamacoesAbertas = await context.ReclamacoesClientes
                .CountAsync(item => item.Status != StatusReclamacao.Encerrada),
            NaoConformidadesAbertas = await context.NaoConformidades
                .CountAsync(item => item.Status != StatusNaoConformidade.Encerrada),
            ClientesAtivos = await context.Clientes.CountAsync(),
            ProdutosCadastrados = await context.Produtos.CountAsync(),
            LotesCadastrados = await context.Lotes.CountAsync(),
            ReclamacoesRecentes = await context.ReclamacoesClientes
                .AsNoTracking()
                .OrderByDescending(item => item.CriadaEm)
                .Take(5)
                .Select(item => new DashboardReclamacaoViewModel
                {
                    Id = item.Id,
                    Codigo = item.Codigo,
                    Cliente = item.Cliente.Nome,
                    Produto = item.Produto.Nome,
                    Status = item.Status,
                    CriadaEm = item.CriadaEm
                })
                .ToListAsync()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
