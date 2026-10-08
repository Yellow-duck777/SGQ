# Matriz de rastreabilidade

Esta matriz liga requisitos, código, testes e evidências e é a única fonte do estado de cada requisito. O detalhamento por requisito deve ser acrescentado no mesmo PR que o valida ou implementa.

Estados: `Proposto`, `Validado`, `Implementado`, `Verificado` e `Adiado`, conforme [requisitos gerais](requisitos-gerais.md). Nenhum requisito está `Verificado` ainda: a verificação independente e a evidência de CI estão pendentes (DEM-2026-003 no [backlog](../gestao/backlog.md)).

Atualização: 2026-10-08. Itens marcados com **(DEM-2026-008)** foram corrigidos na branch `fix/seguranca-fluxos-e-docs` e dependem do resultado dos testes registrado na [daily](../gestao/dailies/2026/10/2026-10-08-correcoes-seguranca-fluxos-e-documentacao.md). Os nomes de testes citados existem em `tests/SGQ.Web.Tests` e `tests/SGQ.ArchitectureTests`; a lista completa está na [matriz de testes](../qualidade/matriz-de-testes.md).

## Responsáveis

- validação de negócio: responsável da Qualidade;
- validação técnica: `@ViniciusLgo`;
- implementação: autor do PR;
- verificação: pessoa diferente do autor quando houver aprovação regulada.

## Requisitos gerais e segurança

| Requisito | Estado | Implementação atual | Teste/evidência | Lacuna / próxima ação |
| --- | --- | --- | --- | --- |
| RF-001 | Implementado | ASP.NET Identity, login individual, bloqueio após 5 falhas por 15 min | Roteiro AUTH-001 | Aprovação de conta, MFA e política de senha 12+ (DEM-2026-005, DEM-2026-113, DEM-2026-115) |
| RF-002 | Implementado | Usuário autenticado gravado como texto (`UsuarioAbertura` etc.); pareceres RT e GQ gravam o usuário **(DEM-2026-008)** | Roteiro AUTH-002; `NaoConformidadesControllerTests`, `RecallsControllerTests` | Trocar texto por FK auditável (DEM-2026-004) |
| RF-003 | Implementado | Perfis Administrador, GQ, RT, CQ e Auditor; política de fallback nega acesso a contas sem perfil **(DEM-2026-008)** | Roteiro SEC-001 | Matriz papel x ação para escrita, pendente de decisão da Qualidade (DEM-2026-101) |
| RF-004 | Implementado (parcial) | `[Authorize(Roles)]` nas transições críticas; antiforgery global; segregação RT/GQ no backend **(DEM-2026-008)** | `NaoConformidadesControllerTests`, `RecallsControllerTests` | Ações de escrita sem perfil específico (DEM-2026-101) |
| RF-005 | Implementado (parcial) | `HistoricoAuditoria` gravado em `SaveChangesAsync`; consulta por processo | Sem teste dedicado | Imutabilidade, FK de usuário, justificativa e testes (DEM-2026-004) |
| RF-006 | Implementado (parcial) | Usuário, data/hora, ação, entidade, chave e alterações; entidades do Identity deixam de ser auditadas **(DEM-2026-008)** | Sem teste dedicado | Justificativa e valor anterior estruturado |
| RF-007 | Implementado (parcial) | Sem exclusão física pela interface de processos; `Produto→Lote` com `Restrict` | Sem teste | Cadastros ainda permitem exclusão onde a FK não impede |
| RF-008 | Implementado (parcial) | Anulação lógica de anexo com justificativa, usuário e data | `AnexosControllerTests` | Inativação de clientes, produtos e lotes (DEM-2026-010) |
| RF-009 | Implementado (parcial) | Anexos em RC, NC e Recall; 25 MB; extensões permitidas; crítico; anulação lógica; vínculo múltiplo sem cópia; download autenticado (anexo anulado: GQ, Administrador e Auditor) **(DEM-2026-008)** | `AnexosControllerTests` (`Enviar_EvidenciaCriticaComum_NaoExigeFluxoDeLaboratorio`, `Vincular_CriaVinculoParaProcessoDeDestino_EImpedeDuplicidade`) | Limite de 20 por registro, assinatura/MIME, SHA-256 e antimalware (DEM-2026-050); armazenamento disco x `bytea` (DEM-2026-102) |
| RF-010 | Implementado | Código, cliente, produto, lote, período, classificação, status, responsável e área nas listagens | Sem teste | Navegação entre processos relacionados e anexos |
| RF-011 | Implementado (parcial) | Busca por código e relacionamentos em RC, NC e Recall | Sem teste | Pesquisa global; índices e paginação (DEM-2026-103) |
| RF-012 | Implementado | Filtros por status, classificação/decisão, produto, lote, responsável e período | Sem teste | — |
| RF-013 | Implementado (parcial) | Datas em `DateTimeOffset` UTC e `DateOnly` | `PrazoServiceTests` | `TimeProvider` e fuso explícito (DEM-2026-104) |
| RF-014 | Implementado | Eventos e destinatários da matriz do RF-014 e alertas de prazo e de ação de NC vencida | Sem teste dedicado | Deduplicação por evento/outbox (DEM-2026-105); "áreas envolvidas" do Recall aprovado |
| RS-001 | Implementado | Identity | Roteiro AUTH-001 | — |
| RS-002 | Implementado | Hash de senha pelo Identity | Roteiro AUTH-001 | — |
| RS-003 | Implementado (parcial) | Política de fallback exige perfil **(DEM-2026-008)**; transições críticas por perfil | Roteiro SEC-001 | Perfis por ação de escrita (DEM-2026-101) |
| RS-004 | Implementado (parcial) | Auditoria em `SaveChanges` | — | Ver RF-005 |
| RS-005 | Implementado (parcial) | Download exige autenticação e perfil | `AnexosControllerTests` | Antimalware, validação de conteúdo e armazenamento (DEM-2026-050, DEM-2026-102) |
| RS-006 | Implementado | User Secrets; nenhuma credencial no repositório (varredura manual de 2026-10-08) | Revisão manual | Varredura automática no CI (DEM-2026-003) |
| RS-007 | Implementado | Exemplos usam apenas valores fictícios | Revisão manual | CI com busca de segredos |

## Cadastros

| Requisito | Estado | Implementação atual | Teste/evidência | Lacuna / próxima ação |
| --- | --- | --- | --- | --- |
| CAD-001 | Proposto | Cliente com nome e contato | Roteiro CAD-001 | Validar campos e identificadores com a Qualidade |
| CAD-002 | Proposto | Produto com nome | Roteiro CAD-001 | Idem |
| CAD-003 | Proposto | Lote com número e produto | Roteiro CAD-001 | Validar datas e unicidade por produto |
| CAD-004 | Proposto | Não há inativação | — | Validar regras (DEM-2026-010) |
| CAD-005 | Proposto | Qualquer usuário com perfil edita cadastros | — | Definir responsáveis por operação (DEM-2026-101) |

## Reclamação de Cliente

| Requisito | Estado | Implementação atual | Teste/evidência | Lacuna / próxima ação |
| --- | --- | --- | --- | --- |
| RF-RC-001–RF-RC-019 | Implementado | Abertura com cliente, contato, produto, lotes do mesmo produto, datas, quantidades, canal, produto disponível e código anual | Roteiro CAD-001 (inclui lote de outro produto rejeitado) | Estados `Rascunho` e `Informações Pendentes` sem ação própria (DEM-2026-106) |
| RF-RC-020–RF-RC-021 | Implementado | Classificação definida na validação pela GQ; `Classificar` restrito a GQ e Administrador somente com a RC em andamento (nem rascunho nem encerrada); enum validado **(DEM-2026-008)** | `FluxosProcessosControllerTests.Validar_ReclamacaoElegivel_CriaNaoConformidadeVinculadaEIniciaInvestigacao` | — |
| RF-RC-022–RF-RC-025 | Validado | Sem busca de reclamações semelhantes nem avaliação de risco estruturada; filtro por lote existe | — | Implementar recorte validado |
| RF-RC-026–RF-RC-033 | Implementado | Investigação, laboratório externo (solicitação, laudo crítico, resultado), Procedente/Improcedente, tratamento | `ReclamacoesControllerTests` (`RegistrarResultadoLaboratorio_*`) | Parâmetros de análise e amostra de retenção como campos estruturados |
| RF-RC-034–RF-RC-035 | Implementado | Resposta ao cliente com data e texto | Sem teste | Tratamento comercial separado (DEM-2026-111) |
| RF-RC-036 | Implementado (parcial) | Prazo de 15 dias úteis com calendário | `PrazoServiceTests` | O prazo deve iniciar só com informações completas, 24 h depois (RN-010 e RN-011; DEM-2026-106) |
| RF-RC-037–RF-RC-038 | Implementado | NC gerada na validação, vinculada e acessível | `FluxosProcessosControllerTests.Validar_*` | Atomicidade com transação explícita |
| RF-RC-039 | Implementado | Encerramento pela GQ; reabertura com justificativa, retorno a Em Investigação | `ReclamacoesControllerTests.Reabrir_ReclamacaoEncerrada_RetornaParaInvestigacaoEPreservaHistorico` | Requisito diz GQ ou RT; código permite GQ e Administrador (DEM-2026-107) |
| ST-RC-001–ST-RC-007 | Implementado (parcial) | Enum `StatusReclamacao` completo; `AvancarStatus` removido **(DEM-2026-008)** | — | Transições de `Rascunho` e `Informações Pendentes` (DEM-2026-106) |

## Não Conformidade

| Requisito | Estado | Implementação atual | Teste/evidência | Lacuna / próxima ação |
| --- | --- | --- | --- | --- |
| RF-NC-001–RF-NC-012 | Implementado | Abertura manual (GQ e Administrador) ou por RC, origem, área, produto, classificação | `FluxosProcessosControllerTests.Validar_*` | — |
| RF-NC-013–RF-NC-017 | Implementado | Criticidade, contenção e notificação de NC crítica | Sem teste | — |
| RF-NC-018–RF-NC-027 | Implementado (parcial) | Investigação, causa provável e raiz, método, ações com responsável, prazo e evidência, laboratório externo | Sem teste específico | Revisão documental e treinamento (RF-NC-026 e RF-NC-027) sem campos próprios |
| RF-NC-028–RF-NC-030 | Implementado | Eficácia eficaz ou ineficaz; ineficaz volta à investigação | Sem teste específico | — |
| RF-NC-031 | Implementado | Aprovações RT e GQ por contas distintas com usuário registrado **(DEM-2026-008)**; decisão do CQ em divergência; encerramento pela GQ; ações vencidas bloqueiam | `NaoConformidadesControllerTests` (`Reprovar_QuandoGqAprovouErtReprova_EncaminhaParaDecisaoDoCq`, `DecidirDivergencia_*`), `FluxosProcessosControllerTests.Encerrar_NaoConformidadeSemAprovacoes_ConservaStatus` | Segregação do CQ (DEM-2026-108) |
| RF-NC-032 | Implementado (parcial) | Indicadores básicos no dashboard e no relatório | — | Reincidência e tempo médio por área (DEM-2026-060) |
| Reabertura de NC | Implementado | GQ, RT ou Auditor; zera aprovações, eficácia e decisão do CQ **(DEM-2026-008)** | `NaoConformidadesControllerTests.Reabrir_NcEncerrada_ResetaAprovacoesERetornaParaInvestigacao` | — |

## Recall

| Requisito | Estado | Implementação atual | Teste/evidência | Lacuna / próxima ação |
| --- | --- | --- | --- | --- |
| RF-RECALL-001–RF-RECALL-017 | Implementado | Abertura, origem, vínculos, quantidades, risco e decisão Aplicável ou Não Aplicável; Não Aplicável grava encerramento **(DEM-2026-008)** | Sem teste de abertura | — |
| RN-RECALL-001–RN-RECALL-002 | Validado | Não há regra automática; a avaliação é manual | — | Critérios regulatórios só com validação documental |
| RF-RECALL-018–RF-RECALL-022 | Implementado (parcial) | Bloqueio, comunicação a clientes e à autoridade, protocolo | `RecallsControllerTests.RegistrarOperacao_ComRegistrosObrigatorios_AguardaRetornos` | Campos regulatórios detalhados (RF-RECALL-022) |
| RF-RECALL-023–RF-RECALL-033 | Implementado (parcial) | Retornos com cliente, quantidade, data, embalagem, lacre, avarias e documento | Sem teste | Produto e lote por retorno; evidências por anexo |
| RF-RECALL-034–RF-RECALL-038 | Implementado (parcial) | Avaliação de destinação e registro de destinação com evidência textual | Sem teste | Classificação do produto devolvido (RF-RECALL-036) |
| RF-RECALL-039 | Implementado | Encerramento pela GQ após aprovações por contas distintas ou decisão favorável do CQ; exige comunicação à autoridade; deadlock de divergência corrigido **(DEM-2026-008)** | `RecallsControllerTests` (`Aprovar_*`, `DecidirDivergencia_*`), `FluxosProcessosControllerTests.Encerrar_RecallSemComunicacaoRegulatoria_ConservaStatus` | — |
| Reabertura de Recall | Implementado | GQ ou RT; limpa aprovações, decisão do CQ, comunicação e encerramento **(DEM-2026-008)** | `RecallsControllerTests.Reabrir_RecallEncerrado_ResetaAprovacoesERetornaParaAvaliacao` | — |

## Regras de negócio

| Requisito | Estado | Implementação atual | Teste/evidência | Lacuna / próxima ação |
| --- | --- | --- | --- | --- |
| RN-001, RN-006, RN-008 | Implementado | NC automática, classificação pela GQ, vínculo RC–NC | `FluxosProcessosControllerTests.Validar_*` | — |
| RN-002–RN-005 | Implementado | Classificações Crítica, Maior e Menor | — | — |
| RN-007 | Validado | Sem exclusão física de processos; sem rotina de retenção | — | Política de retenção em produção |
| RN-009 | Implementado | 15 dias úteis | `PrazoServiceTests` | Ver RF-RC-036 |
| RN-010–RN-011 | Validado | Não implementadas | — | DEM-2026-106 |
| RN-012 | Implementado | Laboratório externo condiciona a conclusão; laudo crítico obrigatório | `ReclamacoesControllerTests`, `AnexosControllerTests` | — |
| RN-013 | Validado | Não implementada (atraso externo) | — | DEM-2026-109 |
| RN-014–RN-017 | Implementado | Prorrogação por GQ e RT com motivo, data anterior e comunicação ao cliente | Sem teste | — |
| RN-018–RN-021 | Implementado (parcial) | Alerta por e-mail a GQ e RT; 48 horas úteis aproximadas por 2 dias úteis | Sem teste | Granularidade de horas |
| RN-022 | Validado | Prazo cobre o processo inteiro; não há controle por etapa | — | — |
| RN-023–RN-024 | Implementado | Atrasado por data-alvo; não bloqueia o tratamento | — | — |

## Validado, mas ainda não implementado

| Item | Requisito | Demanda |
| --- | --- | --- |
| Controle de amostras internas | Seção "Controle de amostras internas" | DEM-2026-110 |
| Tratamento comercial da RC | Seção "Tratamento comercial" | DEM-2026-111 |
| Atraso externo separado nos indicadores | RN-013 e RF-009 | DEM-2026-109 |
| Dashboard completo do MVP (laboratório, RC e NC por classificação) | Seção 12 | DEM-2026-060 |
| Relatórios completos do MVP (Procedente x Improcedente, atrasos externos, tempo médio, Recall por lote) | Seção 12 | DEM-2026-060 |
| Limite de 20 anexos por registro | RF-009 | DEM-2026-050 |
| Início do prazo da RC com informações completas | RN-010 e RN-011 | DEM-2026-106 |

## Registro de validação

Cada alteração de estado deve acrescentar no PR: requisito, estado anterior, estado novo, nome/função dos validadores, data e link da evidência. Não coloque assinatura, e-mail pessoal ou documento interno neste arquivo público.

## Pendências históricas

A seção 13 do documento consolidado preserva o histórico de perguntas (P-001 a P-040), todas resolvidas pela seção 14 e pelos fluxos de status. As lacunas vigentes estão nesta matriz e no [backlog](../gestao/backlog.md).
