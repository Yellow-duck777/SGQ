using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Controllers;

[Authorize]
public class ReclamacoesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var reclamacoes = await context.ReclamacoesClientes
            .Include(reclamacao => reclamacao.Cliente)
            .Include(reclamacao => reclamacao.Produto)
            .OrderByDescending(reclamacao => reclamacao.CriadaEm)
            .ToListAsync();

        return View(reclamacoes);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateOptions();
        return View(new ReclamacaoCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReclamacaoCreateViewModel model)
    {
        var loteIds = model.LoteIds.Distinct().ToList();
        var lotes = await context.Lotes.Where(lote => loteIds.Contains(lote.Id)).ToListAsync();

        if (lotes.Count != loteIds.Count || lotes.Any(lote => lote.ProdutoId != model.ProdutoId))
            ModelState.AddModelError(nameof(model.LoteIds), "Os lotes selecionados devem pertencer ao produto informado.");

        if (!await context.Clientes.AnyAsync(cliente => cliente.Id == model.ClienteId))
            ModelState.AddModelError(nameof(model.ClienteId), "Cliente inválido.");

        if (!await context.Produtos.AnyAsync(produto => produto.Id == model.ProdutoId))
            ModelState.AddModelError(nameof(model.ProdutoId), "Produto inválido.");

        if (!ModelState.IsValid)
        {
            await PopulateOptions(model.ClienteId, model.ProdutoId, loteIds);
            return View(model);
        }

        var year = model.DataRecebimento.Year;
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var lastSequence = await context.ReclamacoesClientes
            .Where(reclamacao => reclamacao.Ano == year)
            .MaxAsync(reclamacao => (int?)reclamacao.SequenciaAnual) ?? 0;

        var sequence = lastSequence + 1;
        var reclamacao = new ReclamacaoCliente
        {
            Ano = year,
            SequenciaAnual = sequence,
            Codigo = $"RC-{year}-{sequence:D6}",
            DataRecebimento = model.DataRecebimento,
            CanalRecebimento = model.CanalRecebimento,
            ClienteId = model.ClienteId,
            ContatoCliente = model.ContatoCliente,
            ProdutoId = model.ProdutoId,
            Descricao = model.Descricao,
            DataFabricacao = model.DataFabricacao,
            DataValidade = model.DataValidade,
            QuantidadeEnvolvida = model.QuantidadeEnvolvida,
            LocalAquisicao = model.LocalAquisicao,
            ProdutoDisponivel = model.ProdutoDisponivel,
            QuantidadeDisponivel = model.QuantidadeDisponivel,
            VolumeDisponivel = model.VolumeDisponivel,
            UsuarioAbertura = User.Identity?.Name ?? "Usuário autenticado",
            CriadaEm = DateTimeOffset.UtcNow
        };

        foreach (var lote in lotes)
            reclamacao.Lotes.Add(new ReclamacaoClienteLote { LoteId = lote.Id });

        context.ReclamacoesClientes.Add(reclamacao);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        TempData["Success"] = $"Reclamação {reclamacao.Codigo} criada como rascunho.";
        return RedirectToAction(nameof(Details), new { id = reclamacao.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var reclamacao = await context.ReclamacoesClientes
            .Include(item => item.Cliente)
            .Include(item => item.Produto)
            .Include(item => item.Lotes).ThenInclude(item => item.Lote)
            .SingleOrDefaultAsync(item => item.Id == id);

        return reclamacao is null ? NotFound() : View(reclamacao);
    }

    private async Task PopulateOptions(int? selectedCliente = null, int? selectedProduto = null, IEnumerable<int>? selectedLotes = null)
    {
        ViewBag.Clientes = new SelectList(await context.Clientes.OrderBy(cliente => cliente.Nome).ToListAsync(), nameof(Cliente.Id), nameof(Cliente.Nome), selectedCliente);
        ViewBag.Produtos = new SelectList(await context.Produtos.OrderBy(produto => produto.Nome).ToListAsync(), nameof(Produto.Id), nameof(Produto.Nome), selectedProduto);
        ViewBag.Lotes = new MultiSelectList(
            await context.Lotes.Include(lote => lote.Produto).OrderBy(lote => lote.Numero).Select(lote => new { lote.Id, Nome = $"{lote.Numero} — {lote.Produto.Nome}" }).ToListAsync(),
            "Id", "Nome", selectedLotes);
    }
}
