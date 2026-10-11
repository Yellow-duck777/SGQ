# ANX-001 — Anexos e evidências

**Objetivo:** conferir envio, download, evidência crítica, vínculo com outro processo e anulação de anexos.
**Perfis envolvidos:** GQ (`gq@sgq.test`), RT, CQ e Auditor.
**Tempo estimado:** 25 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1`. Prepare no computador dois arquivos fictícios: `evidencia.txt` (qualquer texto curto) e `programa.exe` (um arquivo qualquer renomeado). **Não use documentos reais.**

## Dados que serão usados

| Item | Valor |
| --- | --- |
| Processo | uma RC em andamento (ex.: a segunda da lista de Reclamações, "Em investigação") |
| Outro processo | uma NC (anote o código `NC-AAAA-00000N`) |

## Passos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | `gq@sgq.test` | Abra o detalhe da RC e role até **Anexos**. | Aparece "Nenhum anexo enviado ainda." e o formulário com **Escolher arquivo**, descrição, **Evidência crítica** e **Enviar anexo**. O texto ao lado do botão diz "Nenhum arquivo · até 25 MB". | | |
| 2 | `gq@sgq.test` | Clique em **Escolher arquivo** e selecione `evidencia.txt`. | O nome do arquivo aparece ao lado do botão. | | |
| 3 | `gq@sgq.test` | Escreva a descrição `Foto da embalagem` e clique em **Enviar anexo**. | Aviso "Anexo enviado."; o arquivo aparece na lista com tamanho, "enviado por `gq@sgq.test` em (data e hora)" e o botão **Baixar**. | | |
| 4 | `gq@sgq.test` | Clique em **Baixar**. | O navegador baixa o arquivo com o nome original. | | |
| 5 | `gq@sgq.test` | Tente enviar `programa.exe`. | Erro "Arquivo inválido. São aceitos os formatos definidos, com até 25 MB."; nada é salvo. | | |
| 6 | `gq@sgq.test` | Envie `evidencia.txt` de novo, desta vez marcando **Evidência crítica**. | O arquivo aparece com o selo **Crítico**. | | |
| 7 | `gq@sgq.test` | Em um dos anexos abra **Gerenciar anexo → Vincular a outro processo**; digite o código da NC anotada e clique em **Vincular**. | Aviso "Evidência vinculada ao processo NC-…". Em "Processos:" do anexo aparecem os dois códigos. | | |
| 8 | `gq@sgq.test` | Abra a NC e veja os **Anexos**. | O mesmo arquivo aparece na NC (sem duplicar o arquivo). | | |
| 9 | `gq@sgq.test` | Tente vincular outra vez ao mesmo processo, ou a um código inexistente (`NC-1999-000001`). | Erro "Não foi possível vincular: processo inexistente, tipo inválido ou vínculo já criado." | | |
| 10 | `gq@sgq.test` | Em **Gerenciar anexo → Anular anexo**, escreva uma justificativa curta (`curta`) e clique em **Anular**. | Erro "Informe a justificativa da anulação (de 10 a 1000 caracteres)."; o anexo continua ativo. | | |
| 11 | `gq@sgq.test` | Anule com `Arquivo enviado no processo errado.` | O anexo fica com o selo **Anulado** e esmaecido; o botão **Baixar** some. | | |
| 12 | `auditor@sgq.test` | Abra a mesma RC e o anexo anulado. | O Auditor consegue ver o histórico do anexo anulado. | | O download de anexo anulado é permitido a GQ, Administrador e Auditor. |
| 13 | `rt@sgq.test` ou `cq@sgq.test` | Tente baixar o anexo anulado digitando `/Anexos/Baixar/<número>` (o número aparece no endereço do botão **Baixar** de um anexo ativo; teste com o id do anulado). | "Página não encontrada": o download não é entregue. | | |
| 14 | `rt@sgq.test` | Abra um anexo **ativo** e baixe. | O download funciona (qualquer perfil com acesso baixa anexos ativos). | | |
| 15 | `rt@sgq.test` | No anexo ativo, procure **Gerenciar anexo**. | Não aparece (só GQ e Administrador vinculam ou anulam). | | |

## Defeitos conhecidos / observações

- O sistema só confere a **extensão** do arquivo e o limite de 25 MB; não verifica o conteúdo nem executa antivírus.
- Os arquivos ficam gravados no disco da máquina (pasta `App_Data/uploads`), e não no banco; essa decisão está marcada para revisão ([decisão D4](../../gestao/decisoes-pendentes.md)).
- O Auditor enxerga o formulário de envio de anexos, e o servidor aceita o envio por qualquer perfil (decisão D1).
