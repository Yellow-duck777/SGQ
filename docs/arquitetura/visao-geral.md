# Visão geral da arquitetura

## Estado atual

O SGQ é uma aplicação ASP.NET Core MVC única. Controllers acessam diretamente `ApplicationDbContext`; modelos de domínio, persistência, autenticação e apresentação ficam no projeto `SGQ.Web`.

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

`Domain` não referencia outros projetos. `Application` não conhece MVC, EF Core ou PostgreSQL. `Web` usa `Infrastructure` apenas no ponto de composição para registrar as implementações dos contratos; controllers e páginas dependem de casos de uso da `Application`.

## Módulos

Cadastros, RC, NC e Recall permanecem módulos do mesmo sistema. Cada módulo expõe casos de uso pela camada Application e mantém regras no Domain. Serviços compartilhados incluem identidade, auditoria, calendário, anexos, notificações, numeração e relatórios.

## Evolução

A separação acontecerá em PR próprio, acompanhada de testes arquiteturais. Nenhuma funcionalidade deve ser reescrita apenas para “aproveitar” a reorganização.
