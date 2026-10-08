# ADR 0005 — Laboratório externo

- Estado: aceito (registro retroativo)
- Data: 2026-10-08

## Contexto

Quando a investigação de RC ou NC depende de laboratório externo, a conclusão fica condicionada ao resultado e o laudo é anexo obrigatório (RF-009, RN-012).

## Decisão

- RC e NC possuem o status `AguardandoLaboratorioExterno`, alcançado pela solicitação (laboratório e data de envio da amostra).
- O laudo é um anexo marcado como crítico e vinculado ao processo; sem laudo válido e ativo, o resultado não é registrado e o encerramento é recusado.
- Registrado o resultado, o processo retorna à investigação.

## Alternativas

- Entidade própria de laboratório e remessa: adiada; os dados ficam nos processos.
- Laudo opcional: rejeitado pelo requisito.

## Consequências

- O atraso externo (RN-013) e o controle de amostras internas ainda não existem (DEM-2026-109 e DEM-2026-110).
- Recall não tem fluxo de laboratório externo; usa o laudo como evidência quando aplicável.
