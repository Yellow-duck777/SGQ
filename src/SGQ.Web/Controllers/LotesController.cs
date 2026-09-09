using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;

namespace SGQ.Web.Controllers;

[Authorize]
public class LotesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
        => View(await context.Lotes.Include(lote => lote.Produto).OrderBy(lote => lote.Numero).ToListAsync());

    public async Task<IActionResult> Create()
    {
        await PopulateProdutos();
        return View(new Lote());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lote lote)
    {
        if (!ModelState.IsValid)
        {
            await PopulateProdutos(lote.ProdutoId);
            return View(lote);
        }

        context.Lotes.Add(lote);
        await context.SaveChangesAsync();
        TempData["Success"] = "Lote cadastrado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var lote = await context.Lotes.FindAsync(id);
        if (lote is null)
            return NotFound();

        await PopulateProdutos(lote.ProdutoId);
        return View(lote);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Lote lote)
    {
        if (id != lote.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateProdutos(lote.ProdutoId);
            return View(lote);
        }

        context.Update(lote);
        await context.SaveChangesAsync();
        TempData["Success"] = "Lote atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateProdutos(int? selectedId = null)
    {
        var produtos = await context.Produtos.OrderBy(produto => produto.Nome).ToListAsync();
        ViewBag.Produtos = new SelectList(produtos, nameof(Produto.Id), nameof(Produto.Nome), selectedId);
    }
}
