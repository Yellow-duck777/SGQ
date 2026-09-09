using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;

namespace SGQ.Web.Controllers;

[Authorize]
public class ProdutosController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
        => View(await context.Produtos.Include(produto => produto.Lotes).OrderBy(produto => produto.Nome).ToListAsync());

    public IActionResult Create() => View(new Produto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Produto produto)
    {
        if (!ModelState.IsValid)
            return View(produto);

        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        TempData["Success"] = "Produto cadastrado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var produto = await context.Produtos.FindAsync(id);
        return produto is null ? NotFound() : View(produto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Produto produto)
    {
        if (id != produto.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(produto);

        context.Update(produto);
        await context.SaveChangesAsync();
        TempData["Success"] = "Produto atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }
}
