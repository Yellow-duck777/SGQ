using System.ComponentModel.DataAnnotations;

namespace SGQ.Domain.Enums;

public enum ClassificacaoOcorrencia
{
    [Display(Name = "Crítica")] Critica,
    Maior,
    Menor
}
