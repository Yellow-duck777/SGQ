# REL-001 — Relatórios e exportações

**Objetivo:** conferir filtros, indicadores e exportação (CSV, Excel e PDF) do relatório de processos, e quem tem acesso.
**Perfis envolvidos:** GQ, Administrador e Auditor (veem relatórios); RT e CQ (não veem).
**Tempo estimado:** 20 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1` (o banco de demonstração traz 4 RC, 5 NC e 4 Recalls em vários estados). Para abrir os arquivos baixados use Excel ou o Bloco de Notas.

## Dados que serão usados

| Item | Valor |
| --- | --- |
| Total esperado no banco de demonstração | 13 processos (4 RC + 5 NC + 4 Recalls) |
| Produtos | Produto Exemplo Alfa, Beta e Gama |

## Passos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | `rt@sgq.test` | Observe o menu lateral e digite `/Relatorios`. | Não há "Relatórios" no menu; o endereço digitado é negado. | | |
| 2 | `cq@sgq.test` | Repita. | Mesmo resultado. | | |
| 3 | `gq@sgq.test` | Abra **Relatórios**. | Título "Relatório de processos", filtros (Processo, Produto, Aberto a partir de, Aberto até), 4 indicadores e a tabela. | | |
| 4 | `gq@sgq.test` | Confira os indicadores com o banco de demonstração recém-criado. | **Processos no período**: 13; **Em aberto**: 10; **Em atraso**: pelo menos 2 (destacado em vermelho); **Tempo médio até encerrar**: um número de dias. | | Os números de atraso mudam com o passar dos dias (a demonstração usa datas relativas). |
| 5 | `gq@sgq.test` | Confira a tabela: tipo, código (link para o processo), abertura, produto/lote, classificação, situação e prazo. | Nenhum texto técnico aparece: situações como "Aguardando decisão do CQ", classificações como "Crítica", decisões como "Aplicável"/"Não aplicável". Prazos vencidos aparecem em vermelho ("Vencido há N dias"). | | |
| 6 | `gq@sgq.test` | Em **Processo**, escolha só **RC** e aplique. | A tabela e os indicadores mostram apenas as 4 reclamações. | | |
| 7 | `gq@sgq.test` | Volte a **Todos**, escolha **Produto Exemplo Alfa** e aplique. | Só processos desse produto. | | |
| 8 | `gq@sgq.test` | Informe um período em que não haja processos (ex.: datas de 2020) e aplique. | Estado vazio com orientação; indicadores zerados. | | |
| 9 | `gq@sgq.test` | Clique em **Limpar**. | Voltam todos os processos. | | |
| 10 | `gq@sgq.test` | Em **Exportar com os filtros atuais**, clique em **CSV**. | O navegador baixa um arquivo `.csv`; ao abrir, as colunas têm os mesmos textos legíveis da tela (sem nomes de código como `AguardandoAprovacao`). | | |
| 11 | `gq@sgq.test` | Aplique o filtro **RC** e exporte **Excel**. | Planilha `.xlsx` abre no Excel com somente as RC e cabeçalhos legíveis. | | |
| 12 | `gq@sgq.test` | Exporte **PDF** (com ou sem filtro). | O PDF abre com o mesmo conteúdo da tela. | | |
| 13 | `auditor@sgq.test` | Abra **Relatórios** e exporte um CSV. | Funciona (Auditor vê e exporta). | | |
| 14 | `admin@sgq.test` | Abra **Relatórios**. | Funciona. | | |
| 15 | `gq@sgq.test` | No celular ou com a janela estreita (~390 px), abra **Relatórios**. | Os filtros empilham; a tabela vira cartões; não há rolagem horizontal da página. | | |

## Defeitos conhecidos / observações

- O relatório e o histórico não têm paginação; com milhares de processos a tela pode ficar lenta (item no backlog).
- O CSV neutraliza células que começam com `=`, `+`, `-` ou `@` (proteção contra fórmulas) acrescentando uma aspa simples no início.
