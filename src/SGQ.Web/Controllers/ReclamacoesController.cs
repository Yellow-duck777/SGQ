using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.ViewModels;
using SGQ.Web.Security;
using SGQ.Web.Services;

namespace SGQ.Web.Controllers;

[Authorize]
public class ReclamacoesController(ApplicationDbContext context, IPrazoService prazoService) : Controller
{
    public async Task<IActionResult> Index(string? busca, StatusReclamacao? status, ClassificacaoOcorrencia? classificacao, int? produtoId, DateOnly? inicio, DateOnly? fim)
    {
        var query = context.ReclamacoesClientes
            .Include(reclamacao => reclamacao.Cliente)
            .Include(reclamacao => reclamacao.Produto)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim();
            query = query.Where(reclamacao => reclamacao.Codigo.Contains(termo) ||
                reclamacao.Cliente.Nome.Contains(termo) || reclamacao.Produto.Nome.Contains(termo) ||
                reclamacao.Lotes.Any(lote => lote.Lote.Numero.Contains(termo)));
        }
        if (status.HasValue) query = query.Where(reclamacao => reclamacao.Status == status);
        if (classificacao.HasValue) query = query.Where(reclamacao => reclamacao.Classificacao == classificacao);
        if (produtoId.HasValue) query = query.Where(reclamacao => reclamacao.ProdutoId == produtoId);
        if (inicio.HasValue) query = query.Where(reclamacao => reclamacao.DataRecebimento >= inicio);
        if (fim.HasValue) query = query.Where(reclamacao => reclamacao.DataRecebimento <= fim);

        ViewBag.Produtos = new SelectList(await context.Produtos.OrderBy(produto => produto.Nome).ToListAsync(), nameof(Produto.Id), nameof(Produto.Nome), produtoId);
        var reclamacoes = await query.OrderByDescending(reclamacao => reclamacao.CriadaEm).ToListAsync();

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
            DataAlvo = await prazoService.CalcularPrazoReclamacaoAsync(model.DataRecebimento),
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
            .Include(item => item.NaoConformidade)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (reclamacao is null) return NotFound();
        ViewBag.Anexos = await context.Anexos.Where(item => item.ReclamacaoClienteId == id).OrderByDescending(item => item.EnviadoEm).ToListAsync();
        ViewBag.HistoricoAuditoria = await context.HistoricosAuditoria
            .Where(item => item.Entidade == nameof(ReclamacaoCliente) && item.ChaveRegistro == id.ToString())
            .OrderByDescending(item => item.OcorridaEm)
            .ToListAsync();
        return View(reclamacao);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Validar(int id, ClassificacaoOcorrencia classificacao)
    {
        var reclamacao = await context.ReclamacoesClientes.Include(item => item.NaoConformidade).SingleOrDefaultAsync(item => item.Id == id);
        if (reclamacao is null) return NotFound();
        if (reclamacao.Status is not (StatusReclamacao.Rascunho or StatusReclamacao.InformacoesPendentes or StatusReclamacao.AguardandoValidacaoGq))
        {
            TempData["Error"] = "Esta reclamação não está disponível para validação.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        reclamacao.Classificacao = classificacao;
        reclamacao.Status = StatusReclamacao.EmInvestigacao;
        reclamacao.UsuarioValidacao = User.Identity?.Name ?? "Usuário autenticado";
        reclamacao.ValidadaEm = DateTimeOffset.UtcNow;
        if (reclamacao.NaoConformidade is null)
        {
            var year = reclamacao.DataRecebimento.Year;
            var lastSequence = await context.NaoConformidades.Where(item => item.Ano == year).MaxAsync(item => (int?)item.SequenciaAnual) ?? 0;
            var sequence = lastSequence + 1;
            context.NaoConformidades.Add(new NaoConformidade
            {
                Ano = year, SequenciaAnual = sequence, Codigo = $"NC-{year}-{sequence:D6}",
                Origem = OrigemNaoConformidade.ReclamacaoCliente, ReclamacaoClienteId = reclamacao.Id,
                DataAbertura = DateOnly.FromDateTime(DateTime.Today),
                DataAlvo = await prazoService.CalcularPrazoNaoConformidadeAsync(DateOnly.FromDateTime(DateTime.Today), classificacao),
                Area = "Garantia da Qualidade",
                ProdutoId = reclamacao.ProdutoId, Descricao = reclamacao.Descricao, Classificacao = classificacao,
                UsuarioAbertura = reclamacao.UsuarioValidacao, CriadaEm = DateTimeOffset.UtcNow
            });
        }
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Success"] = "Reclamação validada e Não Conformidade criada automaticamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Concluir(ReclamacaoConclusaoViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Details), new { id = model.Id });
        var reclamacao = await context.ReclamacoesClientes.FindAsync(model.Id);
        if (reclamacao is null) return NotFound();
        if (reclamacao.Status != StatusReclamacao.EmInvestigacao)
        {
            TempData["Error"] = "A conclusão só pode ser registrada durante a investigação.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        reclamacao.Investigacao = model.Investigacao;
        reclamacao.Resultado = model.Resultado;
        reclamacao.TratamentoAplicado = model.TratamentoAplicado;
        reclamacao.RespostaCliente = model.RespostaCliente;
        reclamacao.DataRespostaCliente = model.DataRespostaCliente;
        reclamacao.Status = StatusReclamacao.AguardandoConclusao;
        await context.SaveChangesAsync();
        TempData["Success"] = "Conclusão registrada. A reclamação está pronta para encerramento pela GQ.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SolicitarLaboratorio(SolicitarLaboratorioExternoViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Details), new { id = model.Id });
        var item = await context.ReclamacoesClientes.FindAsync(model.Id); if (item is null) return NotFound();
        if (item.Status != StatusReclamacao.EmInvestigacao) return RedirectToAction(nameof(Details), new { id = model.Id });
        item.LaboratorioExterno = model.Laboratorio.Trim(); item.DataEnvioAmostraLaboratorio = model.DataEnvioAmostra; item.LaudoLaboratorioAnexoId = null; item.Status = StatusReclamacao.AguardandoLaboratorioExterno;
        await context.SaveChangesAsync(); TempData["Success"] = "Laboratório externo solicitado."; return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarResultadoLaboratorio(ResultadoLaboratorioExternoViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Details), new { id = model.Id });
        var item = await context.ReclamacoesClientes.FindAsync(model.Id); if (item is null) return NotFound();
        var laudoValido = item.LaudoLaboratorioAnexoId.HasValue && await context.Anexos.AnyAsync(a => a.Id == item.LaudoLaboratorioAnexoId && a.ReclamacaoClienteId == item.Id && a.Critico && a.Ativo);
        if (item.Status != StatusReclamacao.AguardandoLaboratorioExterno || !laudoValido) { TempData["Error"] = "Envie o laudo como anexo crítico antes de registrar o resultado."; return RedirectToAction(nameof(Details), new { id = model.Id }); }
        item.DataRecebimentoResultadoLaboratorio = model.DataRecebimento; item.IdentificacaoLaudoLaboratorio = model.IdentificacaoLaudo.Trim(); item.ResultadoLaboratorio = model.Resultado.Trim(); item.Status = StatusReclamacao.EmInvestigacao;
        await context.SaveChangesAsync(); TempData["Success"] = "Resultado laboratorial registrado. A reclamação voltou para investigação."; return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Roles.GarantiaQualidade + "," + Roles.ResponsavelTecnico)]
    public async Task<IActionResult> Prorrogar(ProrrogacaoPrazoViewModel model)
    {
        if (!ModelState.IsValid || !model.NovaData.HasValue) return RedirectToAction(nameof(Details), new { id = model.Id });
        var item = await context.ReclamacoesClientes.FindAsync(model.Id); if (item is null) return NotFound();
        if (!item.DataAlvo.HasValue || model.NovaData <= item.DataAlvo) { TempData["Error"] = "Informe uma nova data posterior ao prazo atual."; return RedirectToAction(nameof(Details), new { id = model.Id }); }
        context.ProrrogacoesPrazo.Add(new ProrrogacaoPrazo { ReclamacaoClienteId = item.Id, DataAnterior = item.DataAlvo.Value, NovaData = model.NovaData.Value, Motivo = model.Motivo.Trim(), ClienteComunicado = model.ClienteComunicado, RegistroComunicacaoCliente = model.RegistroComunicacaoCliente?.Trim(), Usuario = User.Identity?.Name ?? "Usuário autenticado", RegistradaEm = DateTimeOffset.UtcNow });
        item.DataAlvo = model.NovaData; await context.SaveChangesAsync(); TempData["Success"] = "Prazo prorrogado e registrado no histórico."; return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Encerrar(int id)
    {
        var reclamacao = await context.ReclamacoesClientes.FindAsync(id);
        if (reclamacao is null) return NotFound();
        var laudoValido = string.IsNullOrWhiteSpace(reclamacao.LaboratorioExterno) || (reclamacao.LaudoLaboratorioAnexoId.HasValue && await context.Anexos.AnyAsync(a => a.Id == reclamacao.LaudoLaboratorioAnexoId && a.ReclamacaoClienteId == id && a.Critico && a.Ativo));
        if (reclamacao.Status != StatusReclamacao.AguardandoConclusao || reclamacao.Resultado is null || string.IsNullOrWhiteSpace(reclamacao.RespostaCliente) || !laudoValido)
        {
            TempData["Error"] = "Registre a conclusão e a resposta ao cliente antes do encerramento.";
            return RedirectToAction(nameof(Details), new { id });
        }
        reclamacao.Status = StatusReclamacao.Encerrada;
        reclamacao.UsuarioEncerramento = User.Identity?.Name ?? "Usuário autenticado";
        reclamacao.EncerradaEm = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();
        TempData["Success"] = "Reclamação encerrada pela GQ.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.GestaoQualidade)]
    public async Task<IActionResult> Reabrir(ReclamacaoReaberturaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Informe uma justificativa de reabertura com pelo menos 10 caracteres.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        var reclamacao = await context.ReclamacoesClientes.FindAsync(model.Id);
        if (reclamacao is null) return NotFound();
        if (reclamacao.Status != StatusReclamacao.Encerrada)
        {
            TempData["Error"] = "Somente reclamação encerrada pode ser reaberta.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        reclamacao.StatusAnteriorReabertura = reclamacao.Status;
        reclamacao.JustificativaReabertura = model.Justificativa.Trim();
        reclamacao.UsuarioReabertura = User.Identity?.Name ?? "Usuário autenticado";
        reclamacao.ReabertaEm = DateTimeOffset.UtcNow;
        reclamacao.Status = StatusReclamacao.EmInvestigacao;
        await context.SaveChangesAsync();
        TempData["Success"] = "Reclamação reaberta. Registre uma nova conclusão antes do encerramento pela GQ.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
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
