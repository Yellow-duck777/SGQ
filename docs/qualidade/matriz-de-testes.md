# Matriz de testes

Esta matriz acompanha a [matriz de rastreabilidade](../produto/matriz-rastreabilidade.md). Estados: `Planejado`, `Automatizado`, `Manual`, `Verificado` ou `Bloqueado`.

## Testes automatizados existentes

Todos os testes de `SGQ.Web.Tests` usam EF Core InMemory (ver riscos na [estratégia](estrategia-de-testes.md)). Os testes novos da branch `fix/seguranca-fluxos-e-docs` (DEM-2026-008) são acrescentados nas mesmas classes e na daily da branch.

| Classe | Testes | Cobre |
| --- | --- | --- |
| `ProjectDependencyTests` (`SGQ.ArchitectureTests`) | `Domain_NaoDeveDependerDeCamadasExternas`, `Application_NaoDeveDependerDeInfrastructureOuWeb`, `Infrastructure_NaoDeveDependerDeWeb` | Fronteiras entre camadas |
| `PrazoServiceTests` | `AdicionarDiasUteisAsync_PulaFimDeSemana`, `AdicionarDiasUteisAsync_PulaDiaNaoUtilAtivo`, `CalcularPrazoNaoConformidadeAsync_NaoDefinePrazoParaClassificacaoCritica` | Dias úteis, calendário, prazo de NC |
| `AnexosControllerTests` | `Enviar_EvidenciaCriticaComum_NaoExigeFluxoDeLaboratorio`, `Vincular_CriaVinculoParaProcessoDeDestino_EImpedeDuplicidade` | Envio e vínculo de anexos |
| `FluxosProcessosControllerTests` | `Validar_ReclamacaoElegivel_CriaNaoConformidadeVinculadaEIniciaInvestigacao`, `Encerrar_NaoConformidadeSemAprovacoes_ConservaStatus`, `Encerrar_RecallSemComunicacaoRegulatoria_ConservaStatus` | Validação da RC, regras de encerramento |
| `ReclamacoesControllerTests` | `RegistrarResultadoLaboratorio_SemLaudoCritico_MantemAguardandoLaboratorio`, `RegistrarResultadoLaboratorio_ComLaudoCritico_ArmazenaResultadoERetornaParaInvestigacao`, `Reabrir_ReclamacaoEncerrada_RetornaParaInvestigacaoEPreservaHistorico` | Laboratório externo e reabertura da RC |
| `NaoConformidadesControllerTests` | `Reprovar_QuandoGqAprovouErtReprova_EncaminhaParaDecisaoDoCq`, `DecidirDivergencia_Favoravel_RegistraDecisaoERetornaParaAprovacao`, `DecidirDivergencia_SemJustificativaSuficiente_MantemAguardandoDecisaoDoCq`, `Reabrir_NcEncerrada_ResetaAprovacoesERetornaParaInvestigacao` | Aprovação, divergência e reabertura da NC |
| `RecallsControllerTests` | `Aprovar_QuandoRtEGqAprovam_EncaminhaParaRecolhimento`, `RegistrarOperacao_ComRegistrosObrigatorios_AguardaRetornos`, `DecidirDivergencia_SemJustificativaSuficiente_MantemAguardandoDecisaoDoCq`, `Reabrir_RecallEncerrado_ResetaAprovacoesERetornaParaAvaliacao` | Aprovação, operação e reabertura do Recall |

## Cobertura por tema

| ID | Área | Nível | Resultado esperado | Estado |
| --- | --- | --- | --- | --- |
| AUTH-001 | Login válido e inválido | Manual | Autentica somente conta apta; bloqueio após cinco falhas | Manual (roteiro) |
| AUTH-002 | Identidade do autor e dos pareceres | Manual e unidade | Grava o usuário autenticado | Manual (roteiro) |
| SEC-001 | Acesso sem perfil e por perfil | Manual | Conta sem perfil recebe 403; ações críticas restritas | Manual (roteiro); automação planejada (DEM-2026-117) |
| SEC-002 | Auditoria | Integração | Evento contém ator e mudança e não contém dados do Identity | Planejado |
| SEC-003 | Segregação RT/GQ | Unidade | Mesma conta e Administrador não emitem os dois pareceres | Automatizado (testes da DEM-2026-008) |
| SEC-004 | Segredos e dependências | CI | Nenhum segredo ou vulnerabilidade conhecida | Manual; CI planejado |
| CAD-001 | Cadastros e abertura de RC | Manual | Cadastros listados; RC com código anual; lote de outro produto rejeitado | Manual (roteiro CAD-001) |
| CAD-004 | Inativação | Integração | Registro referenciado não é apagado | Planejado |
| RC | Fluxo de RC | Unidade e integração | Validação, NC automática, laboratório, reabertura | Automatizado em parte (classes acima); encerramento, conclusão e prorrogação planejados |
| NC | Fluxo de NC | Unidade e integração | Aprovação, divergência, reabertura | Automatizado em parte; ações, eficácia e laboratório planejados |
| REC | Fluxo de Recall | Unidade e integração | Aprovação, operação, reabertura | Automatizado em parte; retornos e destinação planejados |
| ANX | Anexos | Integração e segurança | Valida acesso, formato, malware e limites | Automatizado em parte (envio e vínculo); malware, assinatura e limite de 20 planejados |
| PRZ | Prazos e calendário | Unidade | Dias úteis e prazos por classificação | Automatizado (`PrazoServiceTests`) |
| NOT | Notificações e alertas | Unidade e integração | Destinatários e deduplicação | Planejado |
| REL | Relatórios e exportação | Integração | Totais conciliam com os filtros; CSV sem injeção de fórmula | Planejado |
