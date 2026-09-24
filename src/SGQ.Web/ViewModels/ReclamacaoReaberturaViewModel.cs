using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.ViewModels;

public class ReclamacaoReaberturaViewModel
{
    public int Id { get; set; }

    [Required, StringLength(2000, MinimumLength = 10)]
    public string Justificativa { get; set; } = string.Empty;
}
