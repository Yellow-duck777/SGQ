# Visão geral da arquitetura

## Estado atual

O SGQ continua sendo um único sistema implantável, agora dividido inicialmente nos projetos `SGQ.Domain`, `SGQ.Application`, `SGQ.Infrastructure` e `SGQ.Web`. Entidades e enums operacionais estão em `Domain`; testes automatizados protegem as dependências entre as camadas.

Esta é uma separação incremental. `ApplicationDbContext`, Identity, migrations, controllers e apresentação ainda permanecem em `Web`. Por isso, `Web` mantém temporariamente uma referência direta a `Domain` e os controllers ainda acessam EF Core. A próxima fatia criará casos de uso em `Application`, moverá persistência para `Infrastructure` e eliminará esse acesso direto.

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

A primeira separação foi executada com testes arquiteturais e sem alteração do esquema físico do PostgreSQL. As próximas mudanças continuarão em PRs próprios. Nenhuma funcionalidade será reescrita apenas para “aproveitar” a reorganização.
