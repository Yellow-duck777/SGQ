using System.Text;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Security;
using SGQ.Web.ViewModels;

namespace SGQ.Web.Controllers;

[Authorize(Roles = Roles.Administrador + "," + Roles.GarantiaQualidade + "," + Roles.Auditor)]
public class RelatoriosController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(DateOnly? inicio, DateOnly? fim, int? produtoId, string? tipo)
    {
        var model = await CriarRelatorioAsync(inicio, fim, produtoId, tipo);
        await PopularProdutosAsync(produtoId);
        return View(model);
    }

    public async Task<IActionResult> ExportarCsv(DateOnly? inicio, DateOnly? fim, int? produtoId, string? tipo)
    {
        var relatorio = await CriarRelatorioAsync(inicio, fim, produtoId, tipo);
        var csv = new StringBuilder();
        csv.AppendLine("Tipo;Código;Data de abertura;Prazo;Produto;Lote;Situação;Classificação/decisão;Área ou cliente;Encerrado em");
        foreach (var item in relatorio.Itens)
        {
            csv.AppendLine(string.Join(';', new[]
            {
                Csv(item.Tipo), Csv(item.Codigo), Csv(item.DataAbertura.ToString("dd/MM/yyyy")),
                Csv(item.DataAlvo?.ToString("dd/MM/yyyy") ?? string.Empty), Csv(item.Produto ?? string.Empty),
                Csv(item.Lote ?? string.Empty), Csv(item.Situacao), Csv(item.Classificacao ?? string.Empty),
                Csv(item.AreaOuCliente ?? string.Empty), Csv(item.EncerradaEm?.ToLocalTime().ToString("dd/MM/yyyy") ?? string.Empty)
            }));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        return File(bytes, "text/csv; charset=utf-8", $"relatorio-processos-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    public async Task<IActionResult> ExportarXlsx(DateOnly? inicio, DateOnly? fim, int? produtoId, string? tipo)
    {
        var relatorio = await CriarRelatorioAsync(inicio, fim, produtoId, tipo);
        using var workbook = new XLWorkbook();
        var planilha = workbook.Worksheets.Add("Processos");
        planilha.Cell("A1").Value = "Relatório de processos";
        planilha.Range("A1:J1").Merge().Style.Font.Bold = true;
        planilha.Range("A1:J1").Style.Font.SetFontSize(16);
        planilha.Cell("A2").Value = $"Período: {Periodo(relatorio.Inicio, relatorio.Fim)}";
        planilha.Range("A2:J2").Merge().Style.Font.Italic = true;
        var cabecalhos = new[] { "Tipo", "Código", "Abertura", "Prazo", "Produto", "Lote", "Situação", "Classificação/decisão", "Área ou cliente", "Encerrado em" };
        for (var coluna = 0; coluna < cabecalhos.Length; coluna++) planilha.Cell(4, coluna + 1).Value = cabecalhos[coluna];
        var cabecalho = planilha.Range(4, 1, 4, cabecalhos.Length);
        cabecalho.Style.Fill.SetBackgroundColor(XLColor.FromHtml("1F4E78"));
        cabecalho.Style.Font.FontColor = XLColor.White;
        cabecalho.Style.Font.Bold = true;

        var linha = 5;
        foreach (var item in relatorio.Itens)
        {
            planilha.Cell(linha, 1).Value = item.Tipo;
            planilha.Cell(linha, 2).Value = item.Codigo;
            planilha.Cell(linha, 3).Value = item.DataAbertura.ToDateTime(TimeOnly.MinValue);
            planilha.Cell(linha, 4).Value = item.DataAlvo?.ToDateTime(TimeOnly.MinValue);
            planilha.Cell(linha, 5).Value = item.Produto;
            planilha.Cell(linha, 6).Value = item.Lote;
            planilha.Cell(linha, 7).Value = item.Situacao;
            planilha.Cell(linha, 8).Value = item.Classificacao;
            planilha.Cell(linha, 9).Value = item.AreaOuCliente;
            planilha.Cell(linha, 10).Value = item.EncerradaEm?.LocalDateTime;
            linha++;
        }

        if (relatorio.Itens.Count > 0)
        {
            var dados = planilha.Range(4, 1, linha - 1, cabecalhos.Length);
            dados.CreateTable("Processos").Theme = XLTableTheme.TableStyleMedium2;
            planilha.Column(3).Style.DateFormat.Format = "dd/mm/yyyy";
            planilha.Column(4).Style.DateFormat.Format = "dd/mm/yyyy";
            planilha.Column(10).Style.DateFormat.Format = "dd/mm/yyyy";
        }
        planilha.Columns().AdjustToContents();
        planilha.SheetView.FreezeRows(4);

        await using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"relatorio-processos-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    public async Task<IActionResult> ExportarPdf(DateOnly? inicio, DateOnly? fim, int? produtoId, string? tipo)
    {
        var relatorio = await CriarRelatorioAsync(inicio, fim, produtoId, tipo);
        var pdf = Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(20);
            page.DefaultTextStyle(style => style.FontSize(8));
            page.Header().Column(column =>
            {
                column.Item().Text("Relatório de processos").FontSize(18).Bold();
                column.Item().Text($"Período: {Periodo(relatorio.Inicio, relatorio.Fim)}").FontColor(Colors.Grey.Darken1);
            });
            page.Content().PaddingVertical(12).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(42); columns.ConstantColumn(78); columns.ConstantColumn(55); columns.ConstantColumn(55);
                    columns.RelativeColumn(1.3f); columns.RelativeColumn(0.8f); columns.RelativeColumn(1.1f); columns.RelativeColumn(1.1f);
                });
                table.Header(header =>
                {
                    foreach (var titulo in new[] { "Tipo", "Código", "Abertura", "Prazo", "Produto", "Lote", "Situação", "Classificação" })
                        header.Cell().Background(Colors.Blue.Darken3).Padding(4).Text(titulo).FontColor(Colors.White).Bold();
                });
                foreach (var item in relatorio.Itens)
                {
                    Celula(item.Tipo); Celula(item.Codigo); Celula(item.DataAbertura.ToString("dd/MM/yyyy")); Celula(item.DataAlvo?.ToString("dd/MM/yyyy") ?? "—");
                    Celula(item.Produto ?? "—"); Celula(item.Lote ?? "—"); Celula(item.Situacao); Celula(item.Classificacao ?? "—");
                }
                void Celula(string texto) => table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(texto);
            });
            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("SGQ - gerado em "); text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                text.Span(" - página "); text.CurrentPageNumber(); text.Span(" de "); text.TotalPages();
            });
        })).GeneratePdf();
        return File(pdf, "application/pdf", $"relatorio-processos-{DateTime.UtcNow:yyyyMMdd}.pdf");
    }

    private async Task<RelatorioProcessosViewModel> CriarRelatorioAsync(DateOnly? inicio, DateOnly? fim, int? produtoId, string? tipo)
    {
        tipo = tipo is "RC" or "NC" or "Recall" ? tipo : null;
        var itens = new List<RelatorioProcessoItemViewModel>();

        if (tipo is null or "RC")
        {
            var query = context.ReclamacoesClientes.Include(item => item.Produto).Include(item => item.Cliente).AsQueryable();
            if (inicio.HasValue) query = query.Where(item => item.DataRecebimento >= inicio);
            if (fim.HasValue) query = query.Where(item => item.DataRecebimento <= fim);
            if (produtoId.HasValue) query = query.Where(item => item.ProdutoId == produtoId);
            itens.AddRange(await query.Select(item => new RelatorioProcessoItemViewModel
            {
                Tipo = "RC", Codigo = item.Codigo, DataAbertura = item.DataRecebimento, DataAlvo = item.DataAlvo,
                Produto = item.Produto.Nome, Situacao = item.Status.ToString(), Classificacao = item.Classificacao.ToString(),
                AreaOuCliente = item.Cliente.Nome, EncerradaEm = item.EncerradaEm
            }).ToListAsync());
        }

        if (tipo is null or "NC")
        {
            var query = context.NaoConformidades.Include(item => item.Produto).AsQueryable();
            if (inicio.HasValue) query = query.Where(item => item.DataAbertura >= inicio);
            if (fim.HasValue) query = query.Where(item => item.DataAbertura <= fim);
            if (produtoId.HasValue) query = query.Where(item => item.ProdutoId == produtoId);
            itens.AddRange(await query.Select(item => new RelatorioProcessoItemViewModel
            {
                Tipo = "NC", Codigo = item.Codigo, DataAbertura = item.DataAbertura, DataAlvo = item.DataAlvo,
                Produto = item.Produto == null ? null : item.Produto.Nome, Situacao = item.Status.ToString(),
                Classificacao = item.Classificacao.ToString(), AreaOuCliente = item.Area, EncerradaEm = item.EncerradaEm
            }).ToListAsync());
        }

        if (tipo is null or "Recall")
        {
            var query = context.Recalls.Include(item => item.Produto).Include(item => item.Lote).AsQueryable();
            if (inicio.HasValue) query = query.Where(item => item.DataAbertura >= inicio);
            if (fim.HasValue) query = query.Where(item => item.DataAbertura <= fim);
            if (produtoId.HasValue) query = query.Where(item => item.ProdutoId == produtoId);
            itens.AddRange(await query.Select(item => new RelatorioProcessoItemViewModel
            {
                Tipo = "Recall", Codigo = item.Codigo, DataAbertura = item.DataAbertura, DataAlvo = item.DataAlvo,
                Produto = item.Produto.Nome, Lote = item.Lote.Numero, Situacao = item.Status.ToString(),
                Classificacao = item.Decisao.ToString(), AreaOuCliente = item.Origem.ToString(), EncerradaEm = item.EncerradaEm
            }).ToListAsync());
        }

        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var encerrados = itens.Where(item => item.EncerradaEm.HasValue).ToList();
        return new RelatorioProcessosViewModel
        {
            Inicio = inicio, Fim = fim, ProdutoId = produtoId, Tipo = tipo,
            Itens = itens.OrderByDescending(item => item.DataAbertura).ThenBy(item => item.Codigo).ToList(),
            ProcessosAbertos = itens.Count(item => !item.EncerradaEm.HasValue),
            ProcessosAtrasados = itens.Count(item => !item.EncerradaEm.HasValue && item.DataAlvo.HasValue && item.DataAlvo < hoje),
            TempoMedioEncerramentoDias = encerrados.Count == 0 ? null : encerrados.Average(item => (item.EncerradaEm!.Value.Date - item.DataAbertura.ToDateTime(TimeOnly.MinValue)).TotalDays)
        };
    }

    private async Task PopularProdutosAsync(int? produtoId) =>
        ViewBag.Produtos = new SelectList(await context.Produtos.OrderBy(item => item.Nome).ToListAsync(), "Id", "Nome", produtoId);

    private static string Csv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";

    private static string Periodo(DateOnly? inicio, DateOnly? fim) =>
        $"{inicio?.ToString("dd/MM/yyyy") ?? "início"} a {fim?.ToString("dd/MM/yyyy") ?? "hoje"}";
}
