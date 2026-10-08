# ADR 0007 — Prorrogação de prazo

- Estado: aceito (registro retroativo)
- Data: 2026-10-08

## Contexto

Somente GQ e RT autorizam prorrogações, sempre com motivo; o cliente deve ser comunicado quando aplicável (RN-014 a RN-017).

## Decisão

- A tabela `ProrrogacoesPrazo` guarda processo (RC, NC ou Recall), data anterior, nova data, motivo, responsável, data e comunicação ao cliente.
- A ação é restrita aos perfis GQ e RT e dispara notificação a GQ e RT.

## Alternativas

- Sobrescrever a data-alvo sem histórico: rejeitado por perder rastreabilidade.
- Aprovação em duas etapas: não exigida pelos requisitos.

## Consequências

- Há histórico completo de datas por processo.
- A prorrogação não recalcula prazos de ações da NC.
