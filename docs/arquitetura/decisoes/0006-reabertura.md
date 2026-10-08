# ADR 0006 — Reabertura de processos encerrados

- Estado: aceito (registro retroativo); quem reabre a RC está pendente de decisão
- Data: 2026-10-08

## Contexto

Os requisitos permitem reabrir RC, NC e Recall encerrados, com justificativa, auditoria e preservação do número.

## Decisão

- A reabertura mantém o código, registra usuário, data, justificativa (mínimo de 10 caracteres) e status anterior.
- RC volta a `Em Investigação`; NC volta a `Em Investigação` e zera aprovações, eficácia e decisão do CQ; Recall volta a `Em Avaliação` e zera aprovações, pareceres, decisão do CQ e carimbo de encerramento; a comunicação regulatória é registrada de novo, pois o fluxo volta a passar por `RegistrarOperacao`.
- Perfis no código: NC por GQ, RT e Auditor; Recall por GQ e RT; RC por GQ e Administrador.

## Alternativas

- Criar um novo processo vinculado: rejeitado pelos requisitos (número permanece).
- Proibir a reabertura: rejeitado.

## Consequências

- Para RC, o requisito prevê GQ ou RT, e o código permite GQ e Administrador; a divergência está em DEM-2026-107 e precisa de decisão da Qualidade.
- Reabrir exige nova rodada completa de aprovações.
