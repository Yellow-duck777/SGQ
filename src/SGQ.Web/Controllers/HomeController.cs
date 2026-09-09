using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGQ.Web.Models;
using SGQ.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace SGQ.Web.Controllers;

public class HomeController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.RcsAbertas = await context.ReclamacoesClientes.CountAsync(item => item.Status != StatusReclamacao.Encerrada);
        ViewBag.NcsAbertas = await context.NaoConformidades.CountAsync(item => item.Status != StatusNaoConformidade.Encerrada);
        ViewBag.RecallsAtivos = await context.Recalls.CountAsync(item => item.Status != StatusRecall.Encerrado);
        ViewBag.AguardandoAprovacao = await context.NaoConformidades.CountAsync(item => item.Status == StatusNaoConformidade.AguardandoAprovacao)
            + await context.Recalls.CountAsync(item => item.Status == StatusRecall.AguardandoAprovacao);
        return View();
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
