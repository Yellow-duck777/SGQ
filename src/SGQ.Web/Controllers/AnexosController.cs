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
    public async Task<IActionResult> Enviar(IFormFile arquivo, string processo, int id, string? descricao, bool critico, bool laudoLaboratorio = false)
    {
        if (arquivo is null || arquivo.Length == 0 || arquivo.Length > 25 * 1024 * 1024 || !ExtensoesPermitidas.Contains(Path.GetExtension(arquivo.FileName).ToLowerInvariant()))
        { TempData["Error"] = "Arquivo inválido. São aceitos os formatos definidos, com até 25 MB."; return RedirectToAction("Details", processo, new { id }); }
        var anexo = new Anexo { NomeOriginal = Path.GetFileName(arquivo.FileName), TipoConteudo = arquivo.ContentType ?? "application/octet-stream", NomeArmazenado = $"{Guid.NewGuid():N}{Path.GetExtension(arquivo.FileName).ToLowerInvariant()}", TamanhoBytes = arquivo.Length, Descricao = descricao, Critico = critico, Usuario = User.Identity?.Name ?? "Usuário autenticado", EnviadoEm = DateTimeOffset.UtcNow };
        object? processoDestino = processo switch
        {
            "Reclamacoes" => await context.ReclamacoesClientes.FindAsync(id),
            "NaoConformidades" => await context.NaoConformidades.FindAsync(id),
            "Recalls" => await context.Recalls.FindAsync(id),
            _ => null
        };
        if (processoDestino is null) return BadRequest();

        if (laudoLaboratorio && processoDestino is not ReclamacaoCliente { Status: StatusReclamacao.AguardandoLaboratorioExterno }
            && processoDestino is not NaoConformidade { Status: StatusNaoConformidade.AguardandoLaboratorioExterno })
            return BadRequest();

        if (processo == "Reclamacoes") anexo.ReclamacaoClienteId = id;
        else if (processo == "NaoConformidades") anexo.NaoConformidadeId = id;
        else anexo.RecallId = id;
        var directory = Path.Combine(environment.ContentRootPath, "App_Data", "uploads"); Directory.CreateDirectory(directory);
        await using var stream = System.IO.File.Create(Path.Combine(directory, anexo.NomeArmazenado)); await arquivo.CopyToAsync(stream);
        anexo.Critico |= laudoLaboratorio;
        context.Anexos.Add(anexo); await context.SaveChangesAsync();
        if (laudoLaboratorio)
        {
            if (processoDestino is ReclamacaoCliente reclamacao) reclamacao.LaudoLaboratorioAnexoId = anexo.Id;
            else if (processoDestino is NaoConformidade naoConformidade) naoConformidade.LaudoLaboratorioAnexoId = anexo.Id;
            await context.SaveChangesAsync();
        }
        TempData["Success"] = laudoLaboratorio ? "Laudo crítico enviado. Registre o resultado para retomar a investigação." : "Anexo enviado.";
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
    public async Task<IActionResult> Vincular(int id, string processo, int processoId, string codigoDestino)
    {
        var anexo = await context.Anexos.FindAsync(id);
        if (anexo is null) return NotFound();
        var codigo = codigoDestino?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(codigo))
        {
            TempData["Error"] = "Informe o código do processo de destino.";
            return RedirectToAction("Details", processo, new { id = processoId });
        }

        AnexoProcessoVinculo? vinculo = null;
        if (codigo.StartsWith("RC-"))
        {
            var destino = await context.ReclamacoesClientes.SingleOrDefaultAsync(item => item.Codigo == codigo);
            if (destino is not null && anexo.ReclamacaoClienteId != destino.Id && !await context.AnexosProcessosVinculos.AnyAsync(item => item.AnexoId == id && item.ReclamacaoClienteId == destino.Id)) vinculo = new AnexoProcessoVinculo { AnexoId = id, ReclamacaoClienteId = destino.Id };
        }
        else if (codigo.StartsWith("NC-"))
        {
            var destino = await context.NaoConformidades.SingleOrDefaultAsync(item => item.Codigo == codigo);
            if (destino is not null && anexo.NaoConformidadeId != destino.Id && !await context.AnexosProcessosVinculos.AnyAsync(item => item.AnexoId == id && item.NaoConformidadeId == destino.Id)) vinculo = new AnexoProcessoVinculo { AnexoId = id, NaoConformidadeId = destino.Id };
        }
        else if (codigo.StartsWith("REC-"))
        {
            var destino = await context.Recalls.SingleOrDefaultAsync(item => item.Codigo == codigo);
            if (destino is not null && anexo.RecallId != destino.Id && !await context.AnexosProcessosVinculos.AnyAsync(item => item.AnexoId == id && item.RecallId == destino.Id)) vinculo = new AnexoProcessoVinculo { AnexoId = id, RecallId = destino.Id };
        }

        if (vinculo is null)
        {
            TempData["Error"] = "Não foi possível vincular: processo inexistente, tipo inválido ou vínculo já criado.";
            return RedirectToAction("Details", processo, new { id = processoId });
        }

        context.AnexosProcessosVinculos.Add(vinculo);
        await context.SaveChangesAsync();
        TempData["Success"] = $"Evidência vinculada ao processo {codigo}.";
        return RedirectToAction("Details", processo, new { id = processoId });
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
