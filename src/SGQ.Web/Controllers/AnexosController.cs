using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Security;

namespace SGQ.Web.Controllers;

[Authorize]
public class AnexosController(ApplicationDbContext context, IWebHostEnvironment environment) : Controller
{
    private static readonly HashSet<string> ExtensoesPermitidas = [".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".csv", ".txt", ".eml", ".msg", ".mp4"];

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Enviar(IFormFile arquivo, string processo, int id, string? descricao, bool critico)
    {
        if (arquivo is null || arquivo.Length == 0 || arquivo.Length > 25 * 1024 * 1024 || !ExtensoesPermitidas.Contains(Path.GetExtension(arquivo.FileName).ToLowerInvariant()))
        { TempData["Error"] = "Arquivo inválido. São aceitos os formatos definidos, com até 25 MB."; return RedirectToAction("Details", processo, new { id }); }
        var anexo = new Anexo { NomeOriginal = Path.GetFileName(arquivo.FileName), TipoConteudo = arquivo.ContentType ?? "application/octet-stream", NomeArmazenado = $"{Guid.NewGuid():N}{Path.GetExtension(arquivo.FileName).ToLowerInvariant()}", TamanhoBytes = arquivo.Length, Descricao = descricao, Critico = critico, Usuario = User.Identity?.Name ?? "Usuário autenticado", EnviadoEm = DateTimeOffset.UtcNow };
        if (processo == "Reclamacoes") anexo.ReclamacaoClienteId = id;
        else if (processo == "NaoConformidades") anexo.NaoConformidadeId = id;
        else if (processo == "Recalls") anexo.RecallId = id;
        else return BadRequest();
        var directory = Path.Combine(environment.ContentRootPath, "App_Data", "uploads"); Directory.CreateDirectory(directory);
        await using var stream = System.IO.File.Create(Path.Combine(directory, anexo.NomeArmazenado)); await arquivo.CopyToAsync(stream);
        context.Anexos.Add(anexo); await context.SaveChangesAsync(); TempData["Success"] = "Anexo enviado.";
        return RedirectToAction("Details", processo, new { id });
    }

    public async Task<IActionResult> Baixar(int id)
    {
        var anexo = await context.Anexos.FindAsync(id); if (anexo is null) return NotFound();
        var path = Path.Combine(environment.ContentRootPath, "App_Data", "uploads", anexo.NomeArmazenado); if (!System.IO.File.Exists(path)) return NotFound();
        return File(await System.IO.File.ReadAllBytesAsync(path), anexo.TipoConteudo, anexo.NomeOriginal);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Anular(int id, string processo, int processoId, string justificativa)
    {
        var anexo = await context.Anexos.FindAsync(id); if (anexo is null) return NotFound();
        if (string.IsNullOrWhiteSpace(justificativa)) { TempData["Error"] = "Informe a justificativa da anulação."; return RedirectToAction("Details", processo, new { id = processoId }); }
        anexo.Ativo = false; anexo.JustificativaAnulacao = justificativa; anexo.UsuarioAnulacao = User.Identity?.Name ?? "Usuário autenticado"; anexo.AnuladoEm = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(); return RedirectToAction("Details", processo, new { id = processoId });
    }
}
