# CAD-001 — Cadastros (clientes, produtos, lotes)

**Objetivo:** conferir cadastro, edição, busca e validação de clientes, produtos e lotes.
**Perfis envolvidos:** `gq@sgq.test` (qualquer perfil com acesso pode cadastrar hoje).
**Tempo estimado:** 20 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1`.

## Dados que serão usados

| Item | Valor |
| --- | --- |
| Cliente de teste | `Cliente Teste Roteiro` / contato `roteiro@cliente.test` |
| Produto de teste | `Produto Teste Roteiro` |
| Lote de teste | `LT-ROTEIRO-001` (do produto acima) |
| Dados existentes | clientes "Distribuidora Horizonte (demo)", "Rede Vida Saudável (demo)", "Farmácia Central (demo)"; produtos "Produto Exemplo Alfa/Beta/Gama"; lotes ALFA-2026-001, ALFA-2026-002, BETA-2026-001, GAMA-2026-001 |

## Passos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | `gq@sgq.test` | Abra **Clientes** no menu. | Lista com os 3 clientes de demonstração, contador, campo de busca e botão **Novo cliente**. | | |
| 2 | `gq@sgq.test` | Digite `Farmácia` na busca. | A lista filtra e mostra só "Farmácia Central (demo)". Apague o texto: voltam os 3. | | |
| 3 | `gq@sgq.test` | Clique em **Novo cliente**, deixe os campos vazios e salve. | Mensagens de validação junto aos campos obrigatórios; nada é salvo. | | |
| 4 | `gq@sgq.test` | Preencha `Cliente Teste Roteiro` e `roteiro@cliente.test` e salve. | Volta à lista com o novo cliente. | | |
| 5 | `gq@sgq.test` | Edite o cliente criado (ação **Editar**), mude o contato para `outro@cliente.test` e salve. | A lista mostra o contato novo. | | |
| 6 | `gq@sgq.test` | Abra **Produtos** e clique em **Novo produto**; cadastre `Produto Teste Roteiro`. | O produto aparece na lista (com os lotes dele, se houver). | | |
| 7 | `gq@sgq.test` | Abra **Lotes** e clique em **Novo lote**; deixe tudo vazio e salve. | Validação pedindo número e produto; nada é salvo. | | |
| 8 | `gq@sgq.test` | Cadastre o lote `LT-ROTEIRO-001`, escolhendo `Produto Teste Roteiro`. | Mensagem "Lote cadastrado com sucesso."; o lote aparece com o produto correto e o selo "Sem processos". | | Antes das correções desta versão nenhum lote podia ser salvo. |
| 9 | `gq@sgq.test` | Edite o lote e troque para outro produto; salve. | Mensagem "Lote atualizado com sucesso."; a lista mostra o produto novo. | | Volte para o produto original para o próximo roteiro. |
| 10 | `gq@sgq.test` | Abra **Produtos**. | `Produto Teste Roteiro` mostra o número do lote `LT-ROTEIRO-001`. | | |
| 11 | `gq@sgq.test` | Abra **Lotes** e observe o lote `GAMA-2026-001`. | Aparece o selo "Em uso" (há processos com esse lote). | | |
| 12 | `gq@sgq.test` | Abra **Reclamações → Nova reclamação**, escolha `Produto Teste Roteiro`. | A lista de lotes mostra somente `LT-ROTEIRO-001` (lotes de outros produtos não aparecem). | | |
| 13 | `auditor@sgq.test` | Abra Clientes, Produtos e Lotes. | As listas abrem; confira se aparecem os botões de novo/editar (hoje o servidor permite a qualquer perfil; ver observação). | | Decisão D1 em aberto. |

## Defeitos conhecidos / observações

- Ainda **não existe** excluir ou inativar cliente, produto ou lote, e o sistema aceita nomes repetidos (só o calendário barra duplicidade).
- Hoje qualquer perfil autenticado consegue cadastrar e editar; a restrição por perfil aguarda decisão da Qualidade.
