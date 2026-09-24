using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public class DiaNaoUtil
{
    public int Id { get; set; }

    [Required]
    public DateOnly Data { get; set; }

    [Required, StringLength(200)]
    public string Descricao { get; set; } = string.Empty;

    public TipoDiaNaoUtil Tipo { get; set; }

    [Range(2000, 9999)]
    public int Ano { get; set; }

    public bool Ativo { get; set; } = true;
}
