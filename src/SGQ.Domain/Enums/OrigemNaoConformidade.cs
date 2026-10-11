using System.ComponentModel.DataAnnotations;

namespace SGQ.Domain.Enums;

public enum OrigemNaoConformidade
{
    [Display(Name = "Reclamação de cliente")] ReclamacaoCliente,
    Auditoria,
    [Display(Name = "Inspeção")] Inspecao,
    [Display(Name = "Monitoramento de processo")] MonitoramentoDeProcesso,
    [Display(Name = "Controle de mudança")] ControleDeMudanca,
    [Display(Name = "Outro desvio")] OutroDesvio
}
