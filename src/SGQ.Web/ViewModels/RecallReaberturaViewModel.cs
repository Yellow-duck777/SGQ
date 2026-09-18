using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.ViewModels;

public class RecallReaberturaViewModel
{
    public int Id { get; set; }

    [Required, StringLength(500, MinimumLength = 10)]
    public string Motivo { get; set; } = string.Empty;

    [Required, StringLength(2000, MinimumLength = 10)]
    public string Justificativa { get; set; } = string.Empty;
}
