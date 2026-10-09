using System.ComponentModel.DataAnnotations;

namespace SGQ.Domain.Enums;

public enum StatusDecisaoCq
{
    Pendente,
    [Display(Name = "Favorável")] Favoravel,
    [Display(Name = "Desfavorável")] Desfavoravel
}
