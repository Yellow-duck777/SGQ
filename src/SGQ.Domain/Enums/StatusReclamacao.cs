using System.ComponentModel.DataAnnotations;

namespace SGQ.Domain.Enums;

public enum StatusReclamacao
{
    Rascunho,
    [Display(Name = "Informações pendentes")] InformacoesPendentes,
    [Display(Name = "Aguardando validação da GQ")] AguardandoValidacaoGq,
    [Display(Name = "Em investigação")] EmInvestigacao,
    [Display(Name = "Aguardando laboratório externo")] AguardandoLaboratorioExterno,
    [Display(Name = "Aguardando conclusão")] AguardandoConclusao,
    Encerrada
}
