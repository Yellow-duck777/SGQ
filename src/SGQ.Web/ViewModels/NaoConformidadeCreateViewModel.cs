using System.ComponentModel.DataAnnotations;
using SGQ.Domain.Enums;
using SGQ.Web.Models;
namespace SGQ.Web.ViewModels;
public class NaoConformidadeCreateViewModel
{
    [Required, DataType(DataType.Date)] public DateOnly DataAbertura { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [DataType(DataType.Date)] public DateOnly? DataAlvo { get; set; }
    public OrigemNaoConformidade Origem { get; set; }
    [Required, StringLength(150)] public string Area { get; set; } = string.Empty;
    public int? ProdutoId { get; set; }
    [Required, StringLength(4000)] public string Descricao { get; set; } = string.Empty;
    public ClassificacaoOcorrencia Classificacao { get; set; }
}
