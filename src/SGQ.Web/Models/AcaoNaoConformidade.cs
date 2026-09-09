using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class AcaoNaoConformidade
{
    public int Id { get; set; }
    public int NaoConformidadeId { get; set; }
    public NaoConformidade NaoConformidade { get; set; } = null!;
    [Required, StringLength(2000)] public string Descricao { get; set; } = string.Empty;
    [Required, StringLength(256)] public string Responsavel { get; set; } = string.Empty;
    public DateOnly Prazo { get; set; }
    public bool Obrigatoria { get; set; } = true;
    public DateOnly? DataConclusao { get; set; }
    [StringLength(2000)] public string? Evidencia { get; set; }
}
