using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Security;

namespace SGQ.Web.Controllers;

[Authorize]
public class DiasNaoUteisController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(int? ano)
    {
        var anoSelecionado = ano ?? DateTime.Today.Year;
        ViewBag.Ano = anoSelecionado;
        return View(await context.DiasNaoUteis.Where(item => item.Ano == anoSelecionado)
            .OrderBy(item => item.Data).ThenBy(item => item.Tipo).ToListAsync());
    }

    [Authorize(Roles = Roles.GestaoQualidade)]
    public IActionResult Create() => View(new DiaNaoUtil { Data = DateOnly.FromDateTime(DateTime.Today), Ano = DateTime.Today.Year });

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Create(DiaNaoUtil diaNaoUtil)
    {
        ValidarAno(diaNaoUtil);
        if (await context.DiasNaoUteis.AnyAsync(item => item.Data == diaNaoUtil.Data && item.Tipo == diaNaoUtil.Tipo))
            ModelState.AddModelError(string.Empty, "Já existe um dia não útil desse tipo para a data informada.");
        if (!ModelState.IsValid) return View(diaNaoUtil);

        context.DiasNaoUteis.Add(diaNaoUtil);
        await context.SaveChangesAsync();
        TempData["Success"] = "Dia não útil cadastrado.";
        return RedirectToAction(nameof(Index), new { ano = diaNaoUtil.Ano });
    }

    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Edit(int id)
    {
        var diaNaoUtil = await context.DiasNaoUteis.FindAsync(id);
        return diaNaoUtil is null ? NotFound() : View(diaNaoUtil);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Edit(int id, DiaNaoUtil diaNaoUtil)
    {
        if (id != diaNaoUtil.Id) return NotFound();
        ValidarAno(diaNaoUtil);
        if (await context.DiasNaoUteis.AnyAsync(item => item.Id != id && item.Data == diaNaoUtil.Data && item.Tipo == diaNaoUtil.Tipo))
            ModelState.AddModelError(string.Empty, "Já existe um dia não útil desse tipo para a data informada.");
        if (!ModelState.IsValid) return View(diaNaoUtil);

        context.Update(diaNaoUtil);
        await context.SaveChangesAsync();
        TempData["Success"] = "Dia não útil atualizado.";
        return RedirectToAction(nameof(Index), new { ano = diaNaoUtil.Ano });
    }

    private void ValidarAno(DiaNaoUtil diaNaoUtil)
    {
        if (diaNaoUtil.Data.Year != diaNaoUtil.Ano)
            ModelState.AddModelError(nameof(diaNaoUtil.Ano), "O ano deve corresponder à data informada.");
    }
}
