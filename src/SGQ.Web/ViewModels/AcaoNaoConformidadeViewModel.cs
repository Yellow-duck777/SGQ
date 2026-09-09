using System.ComponentModel.DataAnnotations;
namespace SGQ.Web.ViewModels;
public class AcaoNaoConformidadeViewModel
{
    public int NaoConformidadeId { get; set; }
    [Required, StringLength(2000)] public string Descricao { get; set; } = string.Empty;
    [Required, StringLength(256)] public string Responsavel { get; set; } = string.Empty;
    [Required, DataType(DataType.Date)] public DateOnly Prazo { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(15));
    public bool Obrigatoria { get; set; } = true;
}
