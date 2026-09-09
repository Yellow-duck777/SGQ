using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;

namespace SGQ.Web.Controllers;

[Authorize]
public class NaoConformidadesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
        => View(await context.NaoConformidades
            .Include(nc => nc.ReclamacaoCliente)
            .Include(nc => nc.Produto)
            .OrderByDescending(nc => nc.CriadaEm)
            .ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var nc = await context.NaoConformidades
            .Include(item => item.ReclamacaoCliente)
            .Include(item => item.Produto)
            .SingleOrDefaultAsync(item => item.Id == id);
        return nc is null ? NotFound() : View(nc);
    }
}
