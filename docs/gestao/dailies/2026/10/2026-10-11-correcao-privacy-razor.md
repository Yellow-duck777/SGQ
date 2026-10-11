# Daily — Correção da página de privacidade — 2026-10-11

## Identificação

- Demanda: correção de integração; requisito funcional: não se aplica.
- Responsável: desenvolvimento.
- Revisor técnico: `@ViniciusLgo`.
- Branch: `fix/privacy-razor-merge`.
- Estado: Em revisão.

## Objetivo

Corrigir o bloco Razor duplicado na página de privacidade, introduzido pela integração entre as traduções padrão e a revisão de interface.

## Critérios de aceite

- [x] A página possui um único bloco Razor de título, corretamente fechado.
- [x] A página preserva o conteúdo completo da revisão de interface.
- [x] A solução volta a compilar sem erro Razor.
- [x] Não há alteração de regra de negócio, banco, permissão ou configuração.

## Verificações

- `dotnet build SGQ.slnx --no-restore`: 0 avisos e 0 erros.
- `dotnet test SGQ.slnx --no-build`: 104 testes aprovados; 15 de integração ignorados sem `SGQ_TEST_PG` local.
- `npx.cmd --yes markdownlint-cli2@0.18.1`: 0 erros em 70 arquivos.
- Modelo e migrations: não alterados; a checagem automatizada não foi executada porque `dotnet-ef` não está instalado no computador.
