# Visão geral da arquitetura

## Estado atual

O SGQ é um único sistema implantável, dividido nos projetos `SGQ.Domain`, `SGQ.Application`, `SGQ.Infrastructure` e `SGQ.Web`. Testes arquiteturais (`SGQ.ArchitectureTests`) protegem as dependências entre as camadas.

A separação é incremental e ainda parcial:

| Projeto | O que contém hoje |
| --- | --- |
| `SGQ.Domain` | Entidades `Cliente`, `Produto`, `Lote`, `ReclamacaoCliente`, `ReclamacaoClienteLote`, `NaoConformidade` e `AcaoNaoConformidade`; enums de classificação, origem, resultado e status de RC e NC, e `StatusDecisaoCq` |
| `SGQ.Application` | Somente `ApplicationAssembly` (sem casos de uso) |
| `SGQ.Infrastructure` | Somente `InfrastructureAssembly` (sem persistência ou integrações) |
| `SGQ.Web` | MVC, Identity, `ApplicationDbContext`, migrations, serviços (prazos, e-mail, notificações, alertas), 10 controllers que acessam o `DbContext` diretamente, e as entidades `Recall`, `RetornoRecall`, `Anexo`, `AnexoProcessoVinculo`, `DiaNaoUtil`, `HistoricoAuditoria`, `ProrrogacaoPrazo`, `AlertaEnviado`, `ApplicationUser` e os enums de Recall (`StatusRecall`, `OrigemRecall`, `DecisaoRecall`, `TipoDiaNaoUtil`) |

Por isso `Web` referencia `Domain`, `Application` e `Infrastructure` diretamente. A conclusão da refatoração (casos de uso em `Application`, persistência em `Infrastructure`, entidades restantes em `Domain`) é a demanda DEM-2026-007 do backlog.

## Arquitetura alvo

O sistema continuará como monólito implantável, separado em projetos:

```text
SGQ.Domain          <- regras e entidades sem infraestrutura
SGQ.Application     <- casos de uso, contratos e autorização
SGQ.Infrastructure  <- EF Core, PostgreSQL, SMTP e anexos
SGQ.Web             <- MVC, Identity, UI e composição
```

Dependências permitidas:

```text
SGQ.Application    → SGQ.Domain
SGQ.Infrastructure → SGQ.Application
SGQ.Web            → SGQ.Application
SGQ.Web            → SGQ.Infrastructure somente na composição
```

`Domain` não referencia outros projetos. `Application` não conhece MVC, EF Core ou PostgreSQL. `Web` usa `Infrastructure` apenas no ponto de composição; controllers e páginas dependem de casos de uso da `Application`.

## Módulos

Cadastros, RC, NC e Recall permanecem módulos do mesmo sistema. Os serviços compartilhados (identidade, auditoria, calendário, anexos, notificações, numeração e relatórios) existem hoje dentro de `SGQ.Web`; as [decisões arquiteturais](decisoes/README.md) registram o racional de cada um.

## Evolução

A primeira separação foi executada com testes arquiteturais e sem alteração do esquema físico. As próximas mudanças continuarão em PRs próprios. Nenhuma funcionalidade será reescrita apenas para "aproveitar" a reorganização.
