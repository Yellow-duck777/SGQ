using System.ComponentModel.DataAnnotations;
namespace SGQ.Web.ViewModels;
public class SolicitarLaboratorioExternoViewModel { public int Id { get; set; } [Required, StringLength(200)] public string Laboratorio { get; set; } = string.Empty; [Required] public DateOnly? DataEnvioAmostra { get; set; } }
public class ResultadoLaboratorioExternoViewModel { public int Id { get; set; } [Required] public DateOnly? DataRecebimento { get; set; } [Required, StringLength(200)] public string IdentificacaoLaudo { get; set; } = string.Empty; [Required, StringLength(4000)] public string Resultado { get; set; } = string.Empty; }
