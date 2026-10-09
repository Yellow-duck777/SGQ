using System.Globalization;
using System.Text.RegularExpressions;
using SGQ.Web.Models;

namespace SGQ.Web.Presentation;

/// <summary>Uma alteração de campo já em linguagem de usuário.</summary>
public sealed record AlteracaoLegivel(string Campo, string? De, string? Para);

/// <summary>
/// Transforma o texto gravado em <see cref="HistoricoAuditoria.Alteracoes"/> ("Campo: antes → depois; Campo2: ...")
/// em uma lista legível: esconde campos técnicos, ignora o que não mudou e traduz nomes e valores.
/// </summary>
public static partial class AuditoriaFormatador
{
    private static readonly HashSet<string> CamposTecnicos = new(StringComparer.Ordinal)
    {
        "Id", "Ano", "SequenciaAnual", "CriadaEm", "ConcurrencyStamp", "SecurityStamp", "PasswordHash", "StatusAnteriorReabertura"
    };

    private static readonly Dictionary<string, string> NomesDeCampos = new(StringComparer.Ordinal)
    {
        ["Codigo"] = "Código", ["Status"] = "Situação", ["Classificacao"] = "Classificação", ["DataAlvo"] = "Prazo",
        ["DataAbertura"] = "Data de abertura", ["DataRecebimento"] = "Data de recebimento", ["Descricao"] = "Descrição",
        ["Investigacao"] = "Investigação", ["Contencao"] = "Contenção", ["CausaProvavel"] = "Causa provável", ["CausaRaiz"] = "Causa raiz",
        ["MetodoAnalise"] = "Método de análise", ["Eficaz"] = "Eficácia comprovada", ["Origem"] = "Origem", ["Area"] = "Área",
        ["Resultado"] = "Resultado", ["TratamentoAplicado"] = "Tratamento aplicado", ["RespostaCliente"] = "Resposta ao cliente",
        ["DataRespostaCliente"] = "Data da resposta", ["CanalRecebimento"] = "Canal de recebimento", ["ContatoCliente"] = "Contato do cliente",
        ["AprovadaRt"] = "Aprovação do RT", ["AprovadaGq"] = "Aprovação da GQ", ["ReprovadaRt"] = "Reprovação do RT", ["ReprovadaGq"] = "Reprovação da GQ",
        ["UsuarioParecerRt"] = "Parecer do RT por", ["UsuarioParecerGq"] = "Parecer da GQ por", ["DecisaoCq"] = "Decisão do CQ",
        ["JustificativaDecisaoCq"] = "Justificativa do CQ", ["UsuarioDecisaoCq"] = "Decisão do CQ por", ["DecididaPeloCqEm"] = "Decisão do CQ em",
        ["UsuarioValidacao"] = "Validada por", ["ValidadaEm"] = "Validada em", ["UsuarioEncerramento"] = "Encerrado por", ["EncerradaEm"] = "Encerrado em",
        ["UsuarioAbertura"] = "Aberto por", ["UsuarioReabertura"] = "Reaberto por", ["ReabertaEm"] = "Reaberto em", ["JustificativaReabertura"] = "Justificativa da reabertura",
        ["MotivoReabertura"] = "Motivo da reabertura", ["LaboratorioExterno"] = "Laboratório externo", ["DataEnvioAmostraLaboratorio"] = "Envio da amostra",
        ["DataRecebimentoResultadoLaboratorio"] = "Recebimento do resultado", ["IdentificacaoLaudoLaboratorio"] = "Identificação do laudo",
        ["ResultadoLaboratorio"] = "Resultado do laboratório", ["DataFabricacao"] = "Fabricação", ["DataValidade"] = "Validade",
        ["QuantidadeEnvolvida"] = "Quantidade envolvida", ["QuantidadeDisponivel"] = "Quantidade disponível", ["ProdutoDisponivel"] = "Produto disponível",
        ["VolumeDisponivel"] = "Volume disponível", ["LocalAquisicao"] = "Local de aquisição", ["QuantidadeProduzida"] = "Quantidade produzida",
        ["QuantidadeEstoque"] = "Quantidade em estoque", ["QuantidadeDistribuida"] = "Quantidade distribuída", ["ClientesEnvolvidos"] = "Clientes envolvidos",
        ["NaturezaOcorrencia"] = "Natureza da ocorrência", ["RiscoPotencial"] = "Risco potencial", ["Decisao"] = "Decisão", ["JustificativaDecisao"] = "Justificativa da decisão",
        ["BloqueioRegistrado"] = "Bloqueio registrado", ["ComunicacaoClientes"] = "Comunicação aos clientes", ["AutoridadeSanitaria"] = "Autoridade sanitária",
        ["ProtocoloAutoridade"] = "Protocolo da autoridade", ["ComunicadaAutoridadeEm"] = "Autoridade comunicada em", ["Destinacao"] = "Destinação",
        ["EvidenciaDestinacao"] = "Evidência da destinação", ["Nome"] = "Nome", ["Contato"] = "Contato", ["Numero"] = "Número", ["Ativo"] = "Ativo",
        ["Evidencia"] = "Evidência", ["DataConclusao"] = "Concluída em", ["Prazo"] = "Prazo", ["Responsavel"] = "Responsável", ["Obrigatoria"] = "Obrigatória",
        ["NomeOriginal"] = "Arquivo", ["Critico"] = "Crítico", ["JustificativaAnulacao"] = "Justificativa da anulação", ["UsuarioAnulacao"] = "Anulado por",
        ["Email"] = "E-mail", ["UserName"] = "Usuário", ["LockoutEnd"] = "Bloqueado até", ["AccessFailedCount"] = "Tentativas de acesso falhas",
        ["Motivo"] = "Motivo", ["NovaData"] = "Nova data", ["DataAnterior"] = "Data anterior", ["Tipo"] = "Tipo", ["Data"] = "Data"
    };

    public static string NomeDoCampo(string campo) =>
        NomesDeCampos.TryGetValue(campo, out var nome) ? nome : SepararPalavras(campo);

    /// <summary>Lista as alterações relevantes. Em criações devolve apenas os campos preenchidos que importam ao usuário.</summary>
    public static IReadOnlyList<AlteracaoLegivel> Interpretar(string? alteracoes, bool criacao = false)
    {
        if (string.IsNullOrWhiteSpace(alteracoes)) return [];
        var lista = new List<AlteracaoLegivel>();
        foreach (var trecho in SeparadorDeCampos().Split(alteracoes))
        {
            var doisPontos = trecho.IndexOf(": ", StringComparison.Ordinal);
            if (doisPontos <= 0) continue;
            var campo = trecho[..doisPontos].Trim();
            if (CamposTecnicos.Contains(campo) || (campo.EndsWith("Id", StringComparison.Ordinal) && campo.Length > 2 && campo != "Id")) continue;

            var valores = trecho[(doisPontos + 2)..];
            var seta = valores.IndexOf(" → ", StringComparison.Ordinal);
            var antes = seta >= 0 ? valores[..seta].Trim() : string.Empty;
            var depois = seta >= 0 ? valores[(seta + 3)..].Trim() : valores.Trim();

            if (!criacao && string.Equals(antes, depois, StringComparison.Ordinal)) continue;
            if (string.IsNullOrEmpty(antes) && string.IsNullOrEmpty(depois)) continue;
            if (criacao && string.IsNullOrEmpty(depois)) continue;
            if (criacao && depois is "False" or "0") continue;

            lista.Add(new AlteracaoLegivel(NomeDoCampo(campo), criacao ? null : Formatar(antes), Formatar(depois)));
        }
        return lista;
    }

    /// <summary>Resumo de uma frase para o tipo de ação gravado ("Added", "Modified", "Deleted").</summary>
    public static string DescreverAcao(string acao) => acao switch
    {
        "Added" => "Registro criado",
        "Modified" => "Registro atualizado",
        "Deleted" => "Registro removido",
        _ => acao
    };

    private static string? Formatar(string valor)
    {
        if (string.IsNullOrEmpty(valor)) return null;
        if (valor is "True") return "Sim";
        if (valor is "False") return "Não";
        if (DateTimeOffset.TryParse(valor, CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None, out var data) && valor.Contains(':') && valor.Contains('/'))
            return data.ToLocalTime().ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"));
        return EnumRotulos.RotuloPorNome(valor);
    }

    private static string SepararPalavras(string texto) =>
        char.ToUpperInvariant(texto[0]) + SeparadorDeMaiusculas().Replace(texto[1..], " $1").ToLowerInvariant();

    // Começa um novo campo apenas quando um identificador PascalCase seguido de ": " aparece depois de "; ".
    [GeneratedRegex("; (?=[A-Z][A-Za-z0-9]*: )")]
    private static partial Regex SeparadorDeCampos();

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex SeparadorDeMaiusculas();
}
