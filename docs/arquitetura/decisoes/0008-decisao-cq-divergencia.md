# ADR 0008 — Decisão do CQ em divergência

- Estado: aceito (registro retroativo)
- Data: 2026-10-08

## Contexto

Em NC e Recall, RT e GQ aprovam. Quando os pareceres divergem, o CQ decide (RF-NC-031, RF-RECALL-039).

## Decisão

- A divergência leva o processo ao status `AguardandoDecisaoCq` e notifica o CQ.
- O CQ registra decisão favorável ou desfavorável com justificativa de ao menos 10 caracteres, usuário e data.
- Favorável: NC volta a `AguardandoAprovacao` e pode ser encerrada pela GQ com a decisão favorável; Recall segue para `EmRecolhimento` e o encerramento aceita a decisão favorável. Desfavorável: NC volta a `EmTratamento` e Recall a `EmAvaliacao`.
- Reabertura e novas rodadas de aprovação limpam a decisão anterior.

## Alternativas

- Desempate pela GQ: rejeitado pelo requisito.
- Exigir unanimidade sem árbitro: rejeitado por travar o processo.

## Consequências

- O Administrador também pode decidir como CQ no código; a segregação do CQ está em DEM-2026-108.
- O requisito original não listava `AguardandoDecisaoCq` entre os status; o consolidado foi atualizado.
