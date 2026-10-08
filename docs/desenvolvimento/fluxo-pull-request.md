# Fluxo de Pull Request

## Preparação

1. Parta da `main` atualizada.
2. Crie uma branch curta.
3. Implemente somente a daily selecionada.
4. Faça commits separados por intenção.
5. Atualize requisitos, matriz de rastreabilidade, arquitetura (incluindo ADR e banco de dados) e testes afetados.

## Conteúdo do PR

- daily e requisitos relacionados;
- problema e comportamento entregue;
- alterações principais;
- instruções exatas de validação;
- evidências de testes;
- migration e impacto em dados;
- riscos de segurança;
- itens deliberadamente fora do escopo;
- resultado de `dotnet test` e, para documentação, de `markdownlint-cli2`.

## Revisão

O autor não aprova o próprio PR. `@ViniciusLgo` realiza revisão técnica e a responsável da Qualidade valida alterações de negócio. Toda conversa deve ser resolvida e toda alteração após revisão exige nova passagem pelos checks.

## Merge

Use squash merge quando o PR representa uma única intenção. O título final deve seguir Conventional Commits. Após o merge, exclua a branch e atualize a daily e a matriz de rastreabilidade com a evidência final.
