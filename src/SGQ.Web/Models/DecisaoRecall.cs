using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public enum DecisaoRecall
{
    [Display(Name = "Aplicável")] Aplicavel,
    [Display(Name = "Não aplicável")] NaoAplicavel
}
