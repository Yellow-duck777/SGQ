using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGQ.Web.Models;
using SGQ.Web.Data;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Services;

namespace SGQ.Web.Controllers;

public class HomeController(ApplicationDbContext context, IPrazoService prazoService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var limiteProximoVencimento = await prazoService.AdicionarDiasUteisAsync(hoje, 2);
        ViewBag.RcsAbertas = await context.ReclamacoesClientes.CountAsync(item => item.Status != StatusReclamacao.Encerrada);
        ViewBag.NcsAbertas = await context.NaoConformidades.CountAsync(item => item.Status != StatusNaoConformidade.Encerrada);
        ViewBag.RecallsAtivos = await context.Recalls.CountAsync(item => item.Status != StatusRecall.Encerrado);
        ViewBag.AguardandoAprovacao = await context.NaoConformidades.CountAsync(item => item.Status == StatusNaoConformidade.AguardandoAprovacao)
            + await context.Recalls.CountAsync(item => item.Status == StatusRecall.AguardandoAprovacao);
        ViewBag.ProcessosVencidos = await context.ReclamacoesClientes.CountAsync(item => item.Status != StatusReclamacao.Encerrada && item.DataAlvo.HasValue && item.DataAlvo < hoje)
            + await context.NaoConformidades.CountAsync(item => item.Status != StatusNaoConformidade.Encerrada && item.DataAlvo.HasValue && item.DataAlvo < hoje)
            + await context.Recalls.CountAsync(item => item.Status != StatusRecall.Encerrado && item.DataAlvo.HasValue && item.DataAlvo < hoje);
        ViewBag.ProcessosProximosDoVencimento = await context.ReclamacoesClientes.CountAsync(item => item.Status != StatusReclamacao.Encerrada && item.DataAlvo.HasValue && item.DataAlvo >= hoje && item.DataAlvo <= limiteProximoVencimento)
            + await context.NaoConformidades.CountAsync(item => item.Status != StatusNaoConformidade.Encerrada && item.DataAlvo.HasValue && item.DataAlvo >= hoje && item.DataAlvo <= limiteProximoVencimento)
            + await context.Recalls.CountAsync(item => item.Status != StatusRecall.Encerrado && item.DataAlvo.HasValue && item.DataAlvo >= hoje && item.DataAlvo <= limiteProximoVencimento);
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
