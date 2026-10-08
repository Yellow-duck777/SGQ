# Pull Request

## Objetivo

<!-- Explique resumidamente o problema e a solução. -->

Daily: <!-- docs/gestao/dailies/AAAA/MM/AAAA-MM-DD[-slug].md -->
Requisitos: <!-- RF/RN/RS relacionados ou "não se aplica" -->

## Alterações realizadas

-

## Como validar

1.

## Evidências

<!-- Comandos, resultados e capturas sem dados reais. -->

## Impactos

- Banco/migration: não
- Segurança/permissões: não
- Implantação/configuração: não
- Documentação atualizada: não

## Fora do escopo

-

## Checklist

- [ ] O título e os commits seguem o padrão Conventional Commits.
- [ ] Executei `dotnet restore`, `dotnet build --no-restore` e `dotnet test --no-build` sem erros.
- [ ] Executei `npx --yes markdownlint-cli2@0.18.1` quando alterei documentação.
- [ ] Cobri cenários negativos e autorização quando aplicável.
- [ ] Atualizei a matriz de rastreabilidade e a documentação quando necessário.
- [ ] Registrei um ADR quando houve decisão arquitetural.
- [ ] Se há migration, o snapshot está coerente (`dotnet ef migrations has-pending-model-changes`) e `banco-de-dados.md` foi atualizado.
- [ ] Solicitei revisão de `@ViniciusLgo`.
- [ ] Não há segredos, senhas ou dados sensíveis no PR.
