# ADR 0009 — Vínculos múltiplos de anexos

- Estado: aceito (registro retroativo)
- Data: 2026-10-08

## Contexto

Uma mesma evidência ou laudo pode servir a RC, NC e Recall relacionados sem duplicação física (RF-009).

## Decisão

- O anexo pertence ao processo de origem (`Anexos`); vínculos adicionais ficam em `AnexosProcessosVinculos`, que referencia o mesmo anexo e um único processo de destino.
- Uma restrição `CHECK` garante exatamente um processo por vínculo e índices únicos impedem vínculo duplicado.
- Criar vínculo e anular anexo são ações de GQ e Administrador; o destino é informado pelo código do processo.

## Alternativas

- Copiar o arquivo para cada processo: rejeitado pelo requisito.
- Tabela de relação genérica sem restrições: rejeitada por permitir vínculos inválidos.

## Consequências

- A anulação de um anexo afeta todos os processos vinculados.
- O download respeita o perfil, inclusive para anexos anulados.
