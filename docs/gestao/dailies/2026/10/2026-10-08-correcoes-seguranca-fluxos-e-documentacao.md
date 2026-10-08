# Daily — Correções de segurança, fluxos e documentação — 2026-10-08

## Identificação

- Demanda: `DEM-2026-008`
- Responsável: desenvolvimento
- Revisor técnico: `@ViniciusLgo`
- Validador da Qualidade: pendente (decisões de regra listadas abaixo)
- Branch: `fix/seguranca-fluxos-e-docs`
- Estado: Em revisão

## Objetivo

Corrigir as falhas de segurança e de integridade dos fluxos identificadas na auditoria de 2026-10-08 sobre a integração dos trabalhos paralelos e atualizar a documentação para refletir o estado real do código.

## Contexto

A integração da linha de governança com a linha funcional deixou a documentação desatualizada (matriz marcando como ausente o que já existe) e revelou falhas: contas sem perfil acessavam os módulos, qualquer conta autenticada podia classificar e encerrar uma RC, RT e GQ podiam ser a mesma conta, a auditoria gravava hash de senha, e havia travas e resíduos de estado em Recall e NC. Requisitos relacionados: RF-003, RF-004, RF-005, RF-006, RF-009, RF-RC-021, RF-RC-037, RF-RC-039, RF-NC-031, RF-RECALL-039, RS-003 e RS-005.

## Critérios de aceite

- [x] Conta autenticada sem perfil recebe 403 em todas as rotas de negócio (política de fallback; sem teste automatizado, pois exige teste de integração com PostgreSQL, ver DEM-2026-117).
- [x] `AvancarStatus` não existe mais; `Classificar` só aceita GQ e Administrador, somente com a reclamação em andamento (nem rascunho nem encerrada), e rejeita valores de enum inválidos; `Validar` valida a classificação.
- [x] A auditoria não grava entidades do Identity; a migration `LimpaAuditoriaIdentity` remove os registros antigos.
- [x] NC e Recall: reabertura e novas rodadas de aprovação zeram decisão do CQ, reprovações e aprovações.
- [x] Recall: o encerramento aceita decisão favorável do CQ em divergência; a reabertura limpa pareceres, decisão do CQ e carimbo de encerramento; Recall Não Aplicável grava data e usuário de encerramento.
- [x] Segregação RT/GQ: contas distintas, Administrador não aprova como RT ou GQ e o usuário de cada parecer é gravado (migration `AddParecerUsuariosRtGq`).
- [x] Download de anexo anulado restrito a GQ, Administrador e Auditor.
- [x] CSV neutraliza fórmulas.
- [x] `InitialAdminEmail` só promove quando ainda não existe Administrador.
- [x] `App_Data/` consta no `.gitignore`.
- [x] Cada correção tem teste automatizado.
- [x] Matriz de rastreabilidade, README, banco, segurança, ADRs e backlog refletem o código; `markdownlint-cli2` e a verificação de links sem erro.

## Escopo

- correções de código e testes dos itens acima;
- reescrita da matriz de rastreabilidade, atualização de requisitos, README, ambiente local, arquitetura, qualidade, roteiros, backlog, roadmap, CONTRIBUTING, SECURITY e template de PR;
- ADRs 0003 a 0011 e revisão do ADR 0002;
- [proposta de AGENTS.md unificado](../../../proposta-agents-unificado.md) para revisão.

## Fora do escopo

- perfis por ação nas ações de escrita (DEM-2026-101);
- armazenamento de anexos disco x `bytea` (DEM-2026-102);
- índices e paginação, `TimeProvider`, outbox de notificações, concorrência otimista (DEM-2026-103, 104, 105, 112);
- prazo da RC com informações completas, quem reabre a RC, segregação do CQ, atraso externo, amostras e tratamento comercial (DEM-2026-106 a 111);
- hardening de produção, MFA, CI/CD, testes com PostgreSQL real, consolidação de migrations e conclusão da separação em camadas (DEM-2026-113 a 117, 004 e 007);
- edição do `AGENTS.md` (aplicada pelo revisor a partir da proposta).

## Decisões

1. Segregação RT/GQ: contas distintas; o Administrador não aprova como RT ou GQ; o mesmo usuário não emite os dois pareceres; o usuário de cada parecer é gravado.
2. Anexos: a decisão vigente é armazenar em disco (`App_Data/uploads`); o [ADR 0002](../../../../arquitetura/decisoes/0002-anexos-postgresql.md) fica como "aceito com desvio temporário, a reverificar", e o lembrete está no backlog (DEM-2026-102). `App_Data/` entra no `.gitignore`.
3. Uma única branch resolve segurança e documentação.

## Riscos

- A migration `LimpaAuditoriaIdentity` apaga registros de auditoria de forma irreversível.
- A migration `AddParecerUsuariosRtGq` não preenche pareceres já registrados; processos em andamento sem usuário de parecer precisam de nova rodada de aprovação.
- A política de fallback bloqueia contas existentes sem perfil; um Administrador precisa atribuir perfis antes do uso.
- O snapshot do EF citava `SGQ.Web.Models.*` em entidades já movidas para `SGQ.Domain.Entities`; foi regenerado com as migrations desta demanda e `dotnet ef migrations has-pending-model-changes` não acusa pendências.
- Os testes usam EF InMemory e não provam constraints do PostgreSQL.
- Decisão da Qualidade pendente: matriz papel x ação, quem reabre a RC e perfis adicionais.

## Plano

- [x] Auditar documentação e código
- [x] Atualizar a documentação
- [x] Implementar correções de código com testes
- [x] Gerar as migrations e conferir o snapshot
- [x] Executar verificações
- [x] Preparar revisão e commit

## Verificações esperadas

```powershell
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore
dotnet test SGQ.slnx --no-build
dotnet ef migrations has-pending-model-changes --project src/SGQ.Web/SGQ.Web.csproj --startup-project src/SGQ.Web/SGQ.Web.csproj
npx --yes markdownlint-cli2@0.18.1
```

## Evidências

- `dotnet build SGQ.slnx`: 0 avisos, 0 erros.
- `dotnet test SGQ.slnx`: 51 aprovados (3 arquitetura, 48 Web, dos quais 26 novos desta demanda).
- `dotnet ef migrations has-pending-model-changes`: sem pendências.
- `npx markdownlint-cli2@0.18.1`: 0 erros em 54 arquivos.
- Limitação: não havia PostgreSQL local; o SQL das migrations foi gerado e inspecionado (`dotnet ef migrations script`), mas não executado contra um banco.
- Capturas ou gravações sem dados reais: _a preencher_
- PR: _a preencher_

## Bloqueios e decisões

- Matriz papel x ação, perfis adicionais e quem reabre a RC aguardam a Qualidade; não foram inventados.

## Encerramento

- Resultado: correções aplicadas e testadas; pendente revisão técnica e validação da Qualidade.
- Pendências: itens do backlog DEM-2026-101 a 117
- Revisão técnica: _a preencher_
- Validação da Qualidade: _a preencher_
