using System.ComponentModel.DataAnnotations;

namespace SGQ.Domain.Enums;

public enum StatusNaoConformidade
{
    [Display(Name = "Em investigação")] EmInvestigacao,
    [Display(Name = "Em tratamento")] EmTratamento,
    [Display(Name = "Aguardando aprovação")] AguardandoAprovacao,
    [Display(Name = "Aguardando decisão do CQ")] AguardandoDecisaoCq,
    [Display(Name = "Aguardando laboratório externo")] AguardandoLaboratorioExterno,
    Encerrada
}
