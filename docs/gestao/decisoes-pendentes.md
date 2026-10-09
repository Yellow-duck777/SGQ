# Decisões pendentes

Documento para a responsável da Qualidade, o revisor técnico e o time (incluindo o Caio). Cada item descreve o contexto verificado no código, as opções, uma **RECOMENDAÇÃO** (proposta do autor da auditoria, não é decisão), quem decide e onde registrar. Nenhuma regra de negócio foi alterada com base nas recomendações. Demandas relacionadas estão no [backlog](backlog.md). Data: 2026-10-08.

## Resumo

| ID | Decisão | Decide | Demanda |
| --- | --- | --- | --- |
| D1 | Matriz papel x ação nas ações de escrita | Qualidade e revisor técnico | DEM-2026-101 |
| D2 | Quem reabre RC, NC e Recall | Qualidade | DEM-2026-107 |
| D3 | Segregação do CQ e papel do Administrador na divergência | Qualidade | DEM-2026-108 |
| D4 | Armazenamento de anexos: disco x `bytea` | Time e revisor técnico | DEM-2026-102 |
| D5 | Início do prazo da RC e estados sem ação | Qualidade | DEM-2026-106 |
| D6 | Deduplicação de notificações e outbox | Revisor técnico | DEM-2026-105 |
| D7 | Fuso horário de negócio e `TimeProvider` | Revisor técnico e Qualidade | DEM-2026-104 |
| D8 | Senha, MFA, bloqueio e cadastro aberto x por convite | Revisor técnico e Qualidade | DEM-2026-113, DEM-2026-115, DEM-2026-005 |
| D9 | Classificação alterada após a validação | Qualidade | DEM-2026-118 |
| D10 | Atraso externo, amostras internas e tratamento comercial | Qualidade | DEM-2026-109, 110 e 111 |

## D1. Matriz papel x ação para as ações de escrita

**Contexto.** Os perfis são Administrador, GQ, RT, CQ e Auditor. Somente as transições críticas têm `[Authorize(Roles)]` (validar, encerrar, aprovar, decidir divergência, prorrogar, reabrir, vincular e anular anexos, administração). As ações abaixo aceitam qualquer conta com perfil, inclusive o Auditor, que por definição deveria apenas consultar: RC `Create`, `Concluir`, `SolicitarLaboratorio` e `RegistrarResultadoLaboratorio`; NC `RegistrarInvestigacao`, `SolicitarLaboratorio`, `RegistrarResultadoLaboratorio`, `AdicionarAcao`, `ConcluirAcao` e `AvaliarEficacia`; Recall `Create`, `RegistrarOperacao`, `AdicionarRetorno`, `AvancarDestinacao` e `RegistrarDestinacao`; Anexos `Enviar`; cadastros (clientes, produtos, lotes). O requisito 4.2 diz que criam registros "gestores, auditores e colaboradores autorizados", sem mapear para os perfis implementados; o perfil "Colaborador autorizado" não existe.

**Opções.**

1. Manter como está: simples, mas o Auditor escreve e perfis sem relação com a etapa alteram dados regulados.
2. Matriz por perfil (abaixo): reduz o risco, exige validação e pode bloquear usuários atuais.
3. Matriz por perfil mais perfil adicional "Colaborador" (criação limitada): atende o requisito 4.2 literalmente; exige definir o perfil e suas áreas.

**RECOMENDAÇÃO.** Opção 2, com o Auditor somente leitura e uma revisão da Qualidade sobre se haverá perfil "Colaborador". Matriz sugerida para validação:

| Ação | Administrador | GQ | RT | CQ | Auditor |
| --- | --- | --- | --- | --- | --- |
| RC: abrir | Sim | Sim | Sim | Sim | Não |
| RC: laboratório e conclusão | Não | Sim | Sim | Sim | Não |
| NC: investigação, laboratório | Não | Sim | Sim | Sim | Não |
| NC: adicionar ação | Não | Sim | Sim | Não | Não |
| NC: concluir ação | Não | Sim | Sim | Responsável da ação | Não |
| NC: avaliar eficácia | Não | Sim | Sim | Não | Não |
| Recall: abrir e avaliar | Sim | Sim | Sim | Sim | Não |
| Recall: operação, retornos e destinação | Não | Sim | Sim | Não | Não |
| Anexos: enviar | Sim | Sim | Sim | Sim | Não |
| Cadastros: criar e editar | Sim | Sim | Não | Não | Não |

**Decide.** Responsável da Qualidade (conteúdo da matriz) e revisor técnico (implementação).
**Registrar.** Requisito CAD-005 e nova seção "Permissões" no consolidado; ADR que substitua o [ADR 0011](../arquitetura/decisoes/0011-autorizacao-perfis-segregacao.md); daily da demanda.

## D2. Quem reabre RC, NC e Recall

**Contexto.** Requisito (RF-RC-039): RC reaberta por GQ ou RT. Código: RC por GQ e Administrador; NC por GQ, RT e Auditor (alinhado ao requisito, que cita "Auditor autorizado"); Recall por GQ e RT. O encerramento é de GQ e Administrador nos três processos, embora o requisito diga "pela GQ". Ver [ADR 0006](../arquitetura/decisoes/0006-reabertura.md).

**Opções.**

1. Seguir o requisito para a RC (GQ e RT) e manter NC e Recall.
2. Uniformizar os três em GQ e RT (Auditor apenas solicita, sem executar).
3. Manter o código atual (RC com Administrador).

**RECOMENDAÇÃO.** Opção 2: GQ e RT reabrem os três processos; o Auditor apenas solicita por registro; o Administrador não reabre (é perfil técnico). Decidir também se o Administrador continua podendo encerrar.

**Decide.** Responsável da Qualidade.
**Registrar.** Requisito RF-RC-039 e seção "Reabertura" do consolidado; ADR 0006; matriz de rastreabilidade.

## D3. Segregação do CQ e papel do Administrador na divergência

**Contexto.** `DecidirDivergencia` aceita CQ e Administrador, em NC e Recall. O código não impede que a conta do CQ seja a mesma que emitiu parecer RT ou GQ no processo (esse caso só é possível se a conta tiver mais de um perfil). Ver [ADR 0008](../arquitetura/decisoes/0008-decisao-cq-divergencia.md).

**Opções.**

1. Manter: simples; o árbitro pode ser parte da divergência.
2. Impedir que quem emitiu parecer RT ou GQ decida como CQ no mesmo processo.
3. Opção 2 e retirar o Administrador da decisão.

**RECOMENDAÇÃO.** Opção 3, coerente com a regra já adotada de que o Administrador não aprova como RT ou GQ.

**Decide.** Responsável da Qualidade.
**Registrar.** Seção 4.4 do consolidado; ADR 0008 e ADR 0011; teste de integração.

## D4. Armazenamento de anexos: disco x `bytea`

**Contexto.** O ADR 0002 original escolheu `bytea` no PostgreSQL; a implementação grava em disco (`App_Data/uploads`). Limites do produto: 25 MB por arquivo, 20 por registro (o limite de 20 ainda não é aplicado), até 500 MB por processo. **LEMBRETE: reverificar com o Caio e o time antes de produção (DEM-2026-102).**

**Opções e custos.**

| Opção | Backup e restauração | Réplica e escala | Limites e riscos |
| --- | --- | --- | --- |
| Disco (atual) | Backup separado do banco; exige instante comum e volume persistente | Volume compartilhado se houver mais de uma instância | Gravação do arquivo e do registro não é atômica; anexos órfãos |
| `bytea` | Um único backup consistente | Réplica copia anexos (replicação e disco maiores) | Banco cresce até 500 MB por processo; leitura inteira em memória exige streaming; `pg_dump` lento |
| Armazenamento de objetos privado | Política própria de backup e versionamento | Escala bem | Nova dependência e custo; credenciais |

**RECOMENDAÇÃO.** Manter o disco até produção, mas medir backup e restauração com volume realista (por exemplo, 500 MB por processo) e decidir antes da publicação; avaliar objetos se houver mais de uma instância. Implementar de qualquer forma validação de conteúdo, SHA-256, antimalware e limite de 20 (DEM-2026-050).

**Decide.** Time e revisor técnico.
**Registrar.** Novo ADR que substitua o ADR 0002; atualizar `banco-de-dados.md` e o [checklist de publicação](../desenvolvimento/publicacao-checklist.md).

## D5. Prazo da RC e estados sem ação

**Contexto.** RN-010 e RN-011: o prazo só começa com todas as informações completas, 24 horas depois. O código calcula 15 dias úteis a partir de uma data, sem a condição de completude; os estados `Rascunho` e `InformacoesPendentes` existem no enum, mas sem ação de transição (a ação `AvancarStatus` foi removida por permitir saltar etapas).

**Opções.**

1. Manter o cálculo atual e documentar o desvio.
2. Introduzir a ação "Enviar para a GQ" (Rascunho para Informações Pendentes ou Aguardando Validação) e a ação "Informações completas", que grava a data de início do prazo (completude mais 24 h) e calcula o prazo a partir dela.

**RECOMENDAÇÃO.** Opção 2, com a data de completude registrada e auditada, e prazo exibido só depois dela.

**Decide.** Responsável da Qualidade (definição de "informações completas").
**Registrar.** RN-010 e RN-011; [ADR 0004](../arquitetura/decisoes/0004-calendario-dias-uteis.md); daily.

## D6. Deduplicação de notificações por evento

**Contexto.** `FluxoNotificacaoService` deduplica por tipo, referência e dia (`AlertasEnviados`); duas ocorrências legítimas do mesmo tipo no mesmo dia para o mesmo processo (por exemplo, duas divergências) geram apenas um e-mail. O envio ocorre na requisição, sem fila. Ver [ADR 0003](../arquitetura/decisoes/0003-notificacoes-email.md).

**Opções.**

1. Manter, aceitando perda de e-mails repetidos.
2. Incluir o identificador do evento (rodada de aprovação, versão) na chave de deduplicação.
3. Outbox: gravar a notificação na mesma transação e enviar em segundo plano com reprocessamento.

**RECOMENDAÇÃO.** Opção 3 para produção; a opção 2 é o passo intermediário barato.

**Decide.** Revisor técnico.
**Registrar.** ADR 0003 (novo ADR de substituição); matriz de testes (NOT).

## D7. Fuso horário de negócio e `TimeProvider`

**Contexto.** Datas civis usam `DateTime.Today` do servidor e instantes usam UTC. Em servidor em UTC, "hoje" pode divergir do dia útil do negócio perto da meia-noite, afetando prazos, vencidos e alertas.

**Opções.**

1. Manter o relógio do servidor.
2. Fuso de negócio configurável (por exemplo, `America/Sao_Paulo`) e `TimeProvider` injetado para testar.

**RECOMENDAÇÃO.** Opção 2, com fuso configurável e valor padrão `America/Sao_Paulo` (confirmar com a Qualidade que todos os prazos seguem esse fuso).

**Decide.** Revisor técnico (implementação) e Qualidade (fuso).
**Registrar.** ADR 0004; `padroes-codigo.md`.

## D8. Senha, MFA, bloqueio e cadastro

**Contexto.** O documento de [segurança](../arquitetura/seguranca.md) prevê senha de 12 a 128 caracteres, MFA para Administrador, GQ, RT e CQ, aprovação de contas e bloqueio de 15 minutos após cinco falhas (este já existe). Hoje qualquer pessoa cria conta pela tela de login, mas a conta não acessa nada até receber perfil; a política de senha é a padrão do Identity e não há confirmação de e-mail.

**Opções para o cadastro.**

1. Cadastro aberto com perfil atribuído depois (atual): superfície pública para criação de contas e envio de e-mail.
2. Somente por convite do Administrador: reduz a superfície; exige fluxo de convite e SMTP.

**RECOMENDAÇÃO.** Convite pelo Administrador para produção (opção 2), senha de 12 caracteres ou mais, TOTP obrigatório para os perfis críticos e rate limiting no login; manter o bloqueio atual.

**Decide.** Revisor técnico e Qualidade (perfis que exigem MFA).
**Registrar.** `seguranca.md`; ADR de autenticação; requisitos RS.

## D9. Classificação alterada depois da validação

**Contexto.** `Classificar` (GQ e Administrador) altera a classificação da RC em qualquer estado, exceto `Rascunho` e `Encerrada`, sem justificativa e sem refletir na NC já criada (que guarda a própria classificação e o prazo calculado com base nela). RN-006 e a seção "Alterações relevantes" dizem que mudar a classificação é alteração relevante, que invalida aprovações e exige justificativa e auditoria. Não localizei ação para alterar a classificação da NC.

**Opções.**

1. Manter a classificação bloqueada após a validação (só antes).
2. Permitir a alteração com justificativa obrigatória, propagando à NC vinculada, recalculando o prazo da NC e reiniciando aprovações em andamento.

**RECOMENDAÇÃO.** Opção 2, com justificativa mínima, auditoria e notificação a GQ e RT.

**Decide.** Responsável da Qualidade.
**Registrar.** Seção "Alterações relevantes após aprovação" do consolidado; matriz; daily.

## D10. Itens validados e ainda não implementados

**Contexto.** Decididos nos requisitos e sem código: atraso externo separado do interno nos indicadores (RN-013, DEM-2026-109), controle de amostras internas (DEM-2026-110) e tratamento comercial da RC (DEM-2026-111). Um dashboard e relatórios completos do MVP também faltam (DEM-2026-060).

**Opções.** Implementar na ordem de risco regulatório, ou adiar formalmente (estado `Adiado` com justificativa) o que não for necessário no primeiro uso.

**RECOMENDAÇÃO.** Ordem: atraso externo (impacta indicadores de prazo), tratamento comercial (impacta o cliente), amostras internas. Marcar como `Adiado` o que a Qualidade não exigir para a homologação.

**Decide.** Responsável da Qualidade.
**Registrar.** Matriz de rastreabilidade (estado `Adiado` ou demanda priorizada); backlog.
