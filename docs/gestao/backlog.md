# Backlog

Use identificadores `DEM-AAAA-NNN`. Uma demanda só entra em desenvolvimento quando possuir requisito validado, critérios de aceite e responsável.

## Fundação priorizada

| ID | Demanda | Estado | Dependência |
| --- | --- | --- | --- |
| DEM-2026-001 | Organizar documentação e governança | Em andamento | Nenhuma |
| DEM-2026-002 | Separar solução em camadas | Proposto | DEM-2026-001 |
| DEM-2026-003 | Criar pirâmide de testes e CI | Proposto | DEM-2026-002 |
| DEM-2026-004 | Consolidar modelo e migrations | Proposto | DEM-2026-002 |
| DEM-2026-005 | Endurecer autenticação e autorização | Proposto | DEM-2026-003 e 004 |
| DEM-2026-006 | Criar design system, login e sidebar | Proposto | DEM-2026-005 |

## Módulos

| ID | Demanda | Estado | Dependência |
| --- | --- | --- | --- |
| DEM-2026-010 | Validar e concluir cadastros | Proposto | Fundação segura |
| DEM-2026-020 | Implementar fluxo completo de RC | Proposto | Cadastros e serviços compartilhados |
| DEM-2026-030 | Implementar fluxo completo de NC | Proposto | RC e aprovações |
| DEM-2026-040 | Implementar fluxo completo de Recall | Proposto | NC e aprovações |
| DEM-2026-050 | Implementar anexos protegidos | Proposto | ClamAV e auditoria |
| DEM-2026-060 | Implementar dashboard e relatórios | Proposto | Módulos verificados |

## Entrada de ideias

Ideias novas são registradas com problema, usuário afetado e resultado esperado. Elas não recebem código ou testes antes de virar requisito `Validado`.
