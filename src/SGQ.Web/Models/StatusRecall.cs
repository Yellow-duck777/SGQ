using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public enum StatusRecall
{
    [Display(Name = "Em avaliação")] EmAvaliacao,
    [Display(Name = "Aguardando aprovação")] AguardandoAprovacao,
    [Display(Name = "Aguardando decisão do CQ")] AguardandoDecisaoCq,
    [Display(Name = "Em recolhimento")] EmRecolhimento,
    [Display(Name = "Aguardando retorno")] AguardandoRetorno,
    [Display(Name = "Em avaliação de destinação")] EmAvaliacaoDeDestinacao,
    [Display(Name = "Aguardando encerramento")] AguardandoEncerramento,
    Encerrado
}
