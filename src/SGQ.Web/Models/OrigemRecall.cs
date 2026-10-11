using System.ComponentModel.DataAnnotations;

namespace SGQ.Web.Models;

public enum OrigemRecall
{
    [Display(Name = "Reclamação de cliente")] ReclamacaoCliente,
    [Display(Name = "Resultado analítico fora de especificação")] ResultadoAnaliticoForaEspecificacao,
    [Display(Name = "Desvio de produção")] DesvioProducao,
    [Display(Name = "Erro de rotulagem")] ErroRotulagem,
    [Display(Name = "Falha de embalagem")] FalhaEmbalagem,
    [Display(Name = "Não conformidade")] NaoConformidade,
    [Display(Name = "Determinação da autoridade sanitária")] DeterminacaoAutoridadeSanitaria,
    [Display(Name = "Avaliação técnica")] AvaliacaoTecnica
}
