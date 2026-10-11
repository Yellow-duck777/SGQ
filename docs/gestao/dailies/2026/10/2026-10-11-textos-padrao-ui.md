# Daily — Textos padrão da interface — 2026-10-11

## Identificação

- Demanda: manutenção de interface; requisito funcional: não se aplica.
- Responsável: desenvolvimento.
- Revisor técnico: `@ViniciusLgo`.
- Branch: `fix/ui-textos-padrao-ptbr`.
- Estado: Em revisão.

## Objetivo

Traduzir para português as páginas padrão de privacidade e de erro do sistema.

## Critérios de aceite

- [x] A página de privacidade exibe título e texto em português.
- [x] A página de erro exibe mensagens operacionais em português.
- [x] Nenhuma regra de negócio, migration, permissão ou configuração foi alterada.

## Verificações

- `dotnet restore SGQ.slnx`: concluído.
- `dotnet build SGQ.slnx --no-restore`: 0 avisos e 0 erros.
- `dotnet test SGQ.slnx --no-build`: 51 testes aprovados; 12 de integração ignorados sem `SGQ_TEST_PG` local.

## Riscos e pendências

- Nenhum risco funcional identificado.
- Aguardando revisão técnica antes do merge.
