# Requisitos de Reclamação de Cliente

Fonte completa: seções **7**, **10** e **Status da Reclamação de Cliente** de [requisitos-consolidados.md](requisitos-consolidados.md). O estado de cada requisito está na [matriz de rastreabilidade](matriz-rastreabilidade.md).

## Escopo documentado

- RF-RC-001–RF-RC-019: registro e completude;
- RF-RC-020–RF-RC-025: classificação, recorrência e risco;
- RF-RC-026–RF-RC-033: investigação, conclusão e tratamento;
- RF-RC-034–RF-RC-039: resposta, prazo, NC e encerramento;
- ST-RC-001–ST-RC-007: estados permitidos;
- RN-RC-STATUS-001: reclamação não pode ser cancelada.

## Situação atual

O fluxo principal está implementado: abertura com código anual, vínculo de cliente, produto e lotes, validação e classificação pela GQ, geração atômica da NC, investigação, laboratório externo com laudo crítico, conclusão (Procedente ou Improcedente), resposta ao cliente, prorrogação, encerramento pela GQ e reabertura com justificativa.

Lacunas conhecidas: busca de reclamações semelhantes e avaliação de risco estruturada (RF-RC-022 a RF-RC-025), início do prazo apenas com informações completas e 24 horas depois (RN-010 e RN-011), transições dos estados `Rascunho` e `Informações Pendentes`, tratamento comercial e definição de quem reabre (requisito: GQ ou RT; código: GQ e Administrador). Cada lacuna tem demanda no [backlog](../gestao/backlog.md).

## Critério para alterar o módulo

Todos os requisitos do recorte do PR devem estar `Validado`, `Implementado` ou `Verificado`, possuir critérios de aceite e casos negativos. A criação automática da NC deve permanecer atômica com a validação da GQ.
