# ADR 0004 — Calendário de dias úteis e prazos

- Estado: aceito (registro retroativo)
- Data: 2026-10-08

## Contexto

Os prazos de RC (15 dias úteis), NC Maior (15), NC Menor (30) e os alertas dependem de um calendário configurável com feriados nacionais, estaduais, municipais e dias internos.

## Decisão

- A tabela `DiasNaoUteis` guarda data, descrição, tipo e situação ativa; só consultas e alterações por Administrador e GQ, com auditoria.
- `PrazoService` soma dias úteis ignorando sábados, domingos e datas ativas do calendário.
- NC Crítica não recebe prazo automático; a data-alvo é definida pela GQ.
- Datas civis usam `DateOnly`.

## Alternativas

- Feriados por biblioteca ou API externa: rejeitado pela exigência de feriados municipais e internos.
- Calendário fixo no código: rejeitado por exigir novo deploy a cada alteração.

## Consequências

- Alterar uma data não recalcula prazos já gravados.
- O prazo da RC parte da data informada; a regra de iniciar apenas com informações completas, 24 horas depois (RN-010 e RN-011), não foi implementada (DEM-2026-106).
- O fuso da "data de hoje" usa o relógio do servidor; `TimeProvider` e fuso explícito estão em DEM-2026-104.
