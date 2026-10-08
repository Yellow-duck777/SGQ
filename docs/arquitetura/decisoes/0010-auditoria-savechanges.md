# ADR 0010 — Auditoria via SaveChanges

- Estado: aceito, com limitações conhecidas (registro retroativo)
- Data: 2026-10-08

## Contexto

Os requisitos RF-005 e RF-006 exigem histórico de criação, alteração e exclusão, com usuário e data.

## Decisão

- `ApplicationDbContext.SaveChangesAsync` captura entidades adicionadas, modificadas e removidas e grava `HistoricosAuditoria` com entidade, chave, ação, usuário (texto), data UTC e lista de alterações.
- Entidades do Identity e a própria auditoria não são auditadas (correção da branch `fix/seguranca-fluxos-e-docs`, que também remove os registros antigos pela migration `LimpaAuditoriaIdentity`, pois continham hash de senha).
- Sem requisição HTTP, o usuário gravado é "Sistema".

## Alternativas

- Triggers no PostgreSQL: independem da aplicação, mas perdem o usuário da requisição.
- Tabelas temporais ou captura de mudanças: mais robustas, mais complexas.
- Evento de domínio explícito por ação: mais preciso, exige mais código.

## Consequências

- A auditoria é gravada em uma segunda chamada de salvamento; uma falha entre as duas perde o registro.
- A tabela é mutável e o usuário é texto, sem FK; imutabilidade e justificativa dependem de DEM-2026-004.
- O campo de alterações pode conter dados de negócio e não deve receber segredos.
