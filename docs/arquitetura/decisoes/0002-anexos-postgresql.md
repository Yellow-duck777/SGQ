# ADR 0002 — Armazenamento de anexos

- Estado: aceito com desvio temporário; **a reverificar antes de produção**
- Data: 2026-09-22 (decisão original); revisado em 2026-10-08

## Contexto

O produto permite até 20 anexos de 25 MB por processo, podendo alcançar 500 MB por processo. Foram avaliados armazenamento S3 privado, volume do servidor e PostgreSQL.

## Decisão original

Guardar metadados e conteúdo binário em tabela própria no PostgreSQL (`bytea`), com controle transacional centralizado.

## Situação atual (desvio)

A implementação entregue guarda o conteúdo em disco, em `src/SGQ.Web/App_Data/uploads`, com nome aleatório (GUID), e a tabela `Anexos` guarda somente os metadados. Essa é a **decisão vigente** até nova deliberação. A pasta `App_Data/` não é versionada.

## Alternativas

- PostgreSQL (`bytea`): transação única e backup unificado; cresce o banco e exige medir memória.
- Disco do servidor (atual): simples e barato; exige backup do diretório em conjunto com o banco, consistência entre metadados e arquivo e volume persistente em contêiner.
- Armazenamento de objetos privado (S3 ou equivalente): escalável; adiciona dependência externa e custo.

## Consequências

- Backup e restauração precisam incluir banco e diretório de arquivos; uma restauração parcial deixa anexos órfãos ou ausentes.
- A gravação do arquivo e a do registro não são atômicas.
- Em contêiner ou em mais de uma instância, o diretório precisa ser um volume compartilhado e persistente.
- A validação de conteúdo, o SHA-256, a varredura antimalware e o limite de 20 anexos independem do local de armazenamento e seguem pendentes (DEM-2026-050).

## Lembrete de reverificação

Antes de qualquer publicação em produção, reverificar com o time (incluindo o Caio) a decisão disco x `bytea` x armazenamento de objetos, medindo backup, restauração e crescimento. Acompanhamento: DEM-2026-102 no [backlog](../../gestao/backlog.md). Se a decisão mudar, criar um novo ADR que substitua este.
