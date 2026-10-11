using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGQ.Domain.Entities;
using SGQ.Web.Data;

namespace SGQ.Web.Controllers;

[Authorize]
public class LotesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var lotes = await context.Lotes.Include(lote => lote.Produto).OrderBy(lote => lote.Numero).ToListAsync();
        // Apresentação: indica quais lotes já estão vinculados a reclamações ou recalls.
        var emReclamacoes = await context.ReclamacoesClientes.SelectMany(item => item.Lotes).Select(item => item.LoteId).Distinct().ToListAsync();
        var emRecalls = await context.Recalls.Select(item => item.LoteId).Distinct().ToListAsync();
        ViewBag.LotesEmUso = emReclamacoes.Concat(emRecalls).ToHashSet();
        return View(lotes);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateProdutos();
        return View(new Lote());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lote lote)
    {
        await ValidarProdutoAsync(lote);
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

        await ValidarProdutoAsync(lote);
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

    // A propriedade de navegação Produto não vem do formulário (apenas ProdutoId); sem isto o MVC a trata como obrigatória
    // e nenhum lote pode ser salvo. O vínculo é validado pela existência do produto informado.
    private async Task ValidarProdutoAsync(Lote lote)
    {
        ModelState.Remove(nameof(Lote.Produto));
        if (lote.ProdutoId > 0 && !await context.Produtos.AnyAsync(produto => produto.Id == lote.ProdutoId))
            ModelState.AddModelError(nameof(Lote.ProdutoId), "O produto selecionado não existe mais. Escolha outro produto.");
    }

    private async Task PopulateProdutos(int? selectedId = null)
    {
        var produtos = await context.Produtos.OrderBy(produto => produto.Nome).ToListAsync();
        ViewBag.Produtos = new SelectList(produtos, nameof(Produto.Id), nameof(Produto.Nome), selectedId);
    }
}
