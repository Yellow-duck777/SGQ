# Requisitos de Não Conformidade

Fonte completa: seções **8**, **10** e **Status da Não Conformidade** de [requisitos-consolidados.md](requisitos-consolidados.md). O estado de cada requisito está na [matriz de rastreabilidade](matriz-rastreabilidade.md).

## Escopo documentado

- RF-NC-001–RF-NC-012: registro, origem e classificação;
- RF-NC-013–RF-NC-017: criticidade e contenção;
- RF-NC-018–RF-NC-027: investigação e plano de ação;
- RF-NC-028–RF-NC-032: eficácia, encerramento e indicadores.

## Situação atual

Estão implementados: abertura manual (GQ e Administrador) ou originada por RC, contenção, investigação, laboratório externo, plano de ação com responsável e prazo, avaliação de eficácia, aprovações RT e GQ, decisão do CQ em divergência (`AguardandoDecisaoCq`), encerramento pela GQ, prorrogação e reabertura por GQ, RT ou Auditor.

Lacunas conhecidas: campos próprios para revisão documental e treinamento (RF-NC-026 e RF-NC-027), indicadores de reincidência, atraso externo e segregação do CQ.

## Regra de segregação

Quando uma NC exigir aprovações de GQ e RT, contas distintas registram cada parecer, e o sistema grava o usuário de cada um. Um usuário com ambos os perfis não satisfaz as duas aprovações, e o Administrador não aprova como RT ou GQ.
