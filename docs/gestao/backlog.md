# Backlog

Use identificadores `DEM-AAAA-NNN`. Uma demanda só entra em desenvolvimento quando possuir requisito validado, critérios de aceite e responsável. Estados: `Proposto`, `Em andamento`, `Parcial`, `Implementado`, `Verificado` e `Adiado`. Atualização: 2026-10-08.

Numeração: 001–009 fundação e correções; 010–060 módulos; 101 em diante pendências identificadas na auditoria de 2026-10-08.

## Lembrete: reverificar armazenamento de anexos

Antes de qualquer publicação em produção, reverificar com o Caio e o time a decisão entre disco (`App_Data/uploads`, atual), `bytea` no PostgreSQL (intenção original) e armazenamento de objetos. Medir backup, restauração e crescimento. Ver DEM-2026-102 e o [ADR 0002](../arquitetura/decisoes/0002-anexos-postgresql.md).

## Fundação

| ID | Demanda | Estado | Dependência |
| --- | --- | --- | --- |
| DEM-2026-001 | Organizar documentação e governança | Implementado | Nenhuma |
| DEM-2026-002 | Separar solução em camadas (primeira fatia) | Implementado | DEM-2026-001 |
| DEM-2026-003 | Criar pirâmide de testes e CI | Parcial (testes de fluxo e arquitetura existem; sem CI, sem PostgreSQL real) | DEM-2026-002 |
| DEM-2026-004 | Consolidar modelo e migrations (FK de usuário, auditoria imutável, `snake_case`) | Proposto | DEM-2026-002 |
| DEM-2026-005 | Endurecer autenticação e autorização | Parcial (perfis, lockout, fallback e segregação feitos; faltam aprovação de conta e confirmação de e-mail) | DEM-2026-003 e 004 |
| DEM-2026-006 | Criar design system, login e sidebar | Implementado | DEM-2026-005 |
| DEM-2026-007 | Concluir a separação em camadas (casos de uso e persistência; hoje `Application` e `Infrastructure` estão vazios e 10 controllers usam o `DbContext`) | Proposto | DEM-2026-002 e 003 |
| DEM-2026-008 | Correções de segurança e fluxos e atualização da documentação (esta branch) | Em andamento | Nenhuma |

## Módulos

| ID | Demanda | Estado | Dependência |
| --- | --- | --- | --- |
| DEM-2026-010 | Validar e concluir cadastros (CAD-001 a CAD-005, inativação) | Proposto | Fundação segura |
| DEM-2026-011 | Calendário de dias úteis, prazos e prorrogação (retroativa) | Implementado | — |
| DEM-2026-012 | Notificações e alertas de prazo por e-mail (retroativa) | Implementado | — |
| DEM-2026-013 | Pesquisa, filtros, relatório e exportações básicas (retroativa) | Parcial | — |
| DEM-2026-014 | Usuários e perfis: administração e auditoria básica (retroativa) | Implementado | — |
| DEM-2026-015 | Anexos: envio, anulação lógica e vínculos múltiplos (retroativa) | Implementado | — |
| DEM-2026-020 | Fluxo de RC (lacunas em DEM-2026-106, 107 e 111) | Parcial | Cadastros |
| DEM-2026-030 | Fluxo de NC (lacuna em DEM-2026-108) | Implementado | RC |
| DEM-2026-040 | Fluxo de Recall | Implementado | NC |
| DEM-2026-050 | Anexos protegidos: assinatura/MIME, SHA-256, antimalware e limite de 20 por registro | Proposto | Auditoria |
| DEM-2026-060 | Dashboard e relatórios completos do MVP | Parcial | Módulos |

## Pendências identificadas em 2026-10-08 (não corrigidas na DEM-2026-008)

| ID | Demanda | Estado |
| --- | --- | --- |
| DEM-2026-101 | Perfis por ação nas ações de escrita; matriz papel x ação depende de decisão da Qualidade (inclui "Colaborador autorizado" e demais áreas) | Proposto |
| DEM-2026-102 | Reverificar armazenamento de anexos (disco x `bytea` x objetos) com o time antes de produção | Proposto (lembrete) |
| DEM-2026-103 | Índices e paginação nas listagens | Proposto |
| DEM-2026-104 | `TimeProvider` e fuso explícito para "hoje" e prazos | Proposto |
| DEM-2026-105 | Deduplicação de notificações por evento e reprocessamento (outbox) | Proposto |
| DEM-2026-106 | Prazo da RC iniciando só com informações completas, 24 h depois (RN-010 e RN-011); ações para `Rascunho` e `Informações Pendentes` | Proposto |
| DEM-2026-107 | Quem reabre a RC: requisito diz GQ ou RT; código permite GQ e Administrador | Proposto (decisão da Qualidade) |
| DEM-2026-108 | Segregação do CQ e do Administrador na decisão de divergência | Proposto |
| DEM-2026-109 | Atraso externo de laboratório nos indicadores (RN-013) | Proposto |
| DEM-2026-110 | Controle de amostras internas | Proposto |
| DEM-2026-111 | Tratamento comercial da RC | Proposto |
| DEM-2026-112 | Concorrência otimista (`xmin`) nas edições | Proposto |
| DEM-2026-113 | Hardening de produção: senha 12 a 128, cabeçalhos, rate limiting, sessão, Data Protection persistente, migrations fora da inicialização | Proposto |
| DEM-2026-114 | Consolidação de migrations antes da produção, preservando `Ativo = true` dos anexos (risco de `AddAnexoCriticalidade`) | Proposto |
| DEM-2026-115 | MFA (TOTP) e autenticação recente para ações críticas | Proposto |
| DEM-2026-116 | CD e publicação (Docker, proxy HTTPS, homologação) | Proposto |
| DEM-2026-117 | Testes com PostgreSQL real (Testcontainers), autorização via HTTP e E2E | Proposto |

## Entrada de ideias

Ideias novas são registradas com problema, usuário afetado e resultado esperado. Elas não recebem código ou testes antes de virar requisito `Validado`.
