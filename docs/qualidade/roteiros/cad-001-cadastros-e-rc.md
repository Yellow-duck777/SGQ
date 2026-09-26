# CAD-001 — Cadastros e abertura de RC

## Pré-condição

Usuário fictício autenticado e banco de desenvolvimento sem dados reais.

## Passos

1. Cadastre `Cliente Teste`.
2. Cadastre `Produto Teste`.
3. Cadastre o lote `LT-001` vinculado ao produto.
4. Abra uma reclamação usando cliente, produto e lote.
5. Consulte a listagem e os detalhes.

## Resultado esperado

- cadastros aparecem nas listagens;
- lote mostra o produto correto;
- RC recebe código no formato `RC-AAAA-000001`;
- status inicial é `Rascunho`;
- cliente, produto e lote aparecem nos detalhes.

## Cenário negativo

Tente enviar um lote que pertença a outro produto. O backend deve rejeitar a combinação, mesmo que a requisição seja manipulada.
