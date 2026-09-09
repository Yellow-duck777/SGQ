using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;

namespace SGQ.Web.Controllers;

[Authorize]
public class ClientesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
        => View(await context.Clientes.OrderBy(cliente => cliente.Nome).ToListAsync());

    public IActionResult Create() => View(new Cliente());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        if (!ModelState.IsValid)
            return View(cliente);

        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();
        TempData["Success"] = "Cliente cadastrado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var cliente = await context.Clientes.FindAsync(id);
        return cliente is null ? NotFound() : View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cliente cliente)
    {
        if (id != cliente.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(cliente);

        context.Update(cliente);
        await context.SaveChangesAsync();
        TempData["Success"] = "Cliente atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }
}
