# ADR 0002 — Anexos no PostgreSQL

- Estado: aceito
- Data: 2026-09-22

## Contexto

O produto permite até 20 anexos de 25 MB por processo. Foram avaliados armazenamento S3 privado, volume do servidor e PostgreSQL.

## Decisão

Guardar metadados e conteúdo binário em tabela própria no PostgreSQL.

## Consequências

O controle transacional é centralizado, mas o banco e os backups podem crescer até 500 MB por processo. A implementação deverá medir memória, usar endpoints autorizados, validar conteúdo, fazer varredura antimalware e provar backup/restauração antes da produção.
