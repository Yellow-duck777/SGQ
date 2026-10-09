# RC-001 — Fluxo completo da Reclamação de Cliente

**Objetivo:** percorrer a reclamação do registro ao encerramento e à reabertura, incluindo laboratório externo e prorrogação de prazo.
**Perfis envolvidos:** GQ (`gq@sgq.test`), RT, CQ e Auditor (para conferir o que cada um vê).
**Tempo estimado:** 40 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1`. Este roteiro cria a própria reclamação (anote o código `RC-AAAA-00000N` no passo 5).

## Dados que serão usados

| Item | Valor |
| --- | --- |
| Cliente | Distribuidora Horizonte (demo) |
| Produto / lote | Produto Exemplo Alfa / ALFA-2026-001 |
| Canal | E-mail |
| Descrição | `Teste de roteiro: embalagem amassada na entrega.` |
| Contas | `gq@`, `rt@`, `cq@`, `auditor@` (domínio `sgq.test`) |

## Passos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | `gq@sgq.test` | Abra **Reclamações** e clique em **Nova reclamação**. | Formulário em 4 seções ("Dados do recebimento", cliente e produto, descrição, amostra); campos obrigatórios com `*`. Não há campo de data-alvo (o prazo é calculado). | | |
| 2 | `gq@sgq.test` | Clique em **Registrar reclamação** sem preencher nada. | Mensagens em português junto a cada campo obrigatório (canal, cliente, produto, descrição); nada é salvo. | | |
| 3 | `gq@sgq.test` | Escolha o canal **E-mail** e o cliente **Distribuidora Horizonte (demo)**. | O campo **Contato do cliente** é preenchido sozinho com o contato cadastrado. | | |
| 4 | `gq@sgq.test` | Escolha o produto **Produto Exemplo Alfa**; marque o lote **ALFA-2026-001** (a lista tem busca e contador); escreva a descrição e clique em **Registrar reclamação**. | Aviso verde "Reclamação RC-AAAA-00000N criada e encaminhada para validação da GQ." e abre o detalhe. | | |
| 5 | `gq@sgq.test` | No detalhe, confira o cabeçalho. | Código, selo **Aguardando validação da GQ**, prazo "No prazo · dd/mm", trilho de etapas na etapa 2 (Validação da GQ) e painel "Próxima ação: validar e classificar". **Anote o código.** | | |
| 6 | `rt@`, `cq@`, `auditor@` | Abra a mesma RC com cada conta. | Painel "Aguardando validação da GQ"; **não** existe o botão **Validar e criar NC**. | | |
| 7 | `gq@sgq.test` | Clique em **Validar e criar NC** sem escolher a classificação. | O navegador exige a classificação; nada acontece. | | |
| 8 | `gq@sgq.test` | Escolha **Crítica** e clique em **Validar e criar NC**; confirme o diálogo. | Aviso "Reclamação validada e Não Conformidade criada automaticamente."; situação **Em investigação**; aparece o bloco "Não conformidade vinculada" com o código `NC-…` e botão **Abrir NC**. | | |
| 9 | `gq@sgq.test` | Abra **Precisa de análise externa? Solicitar laboratório**; informe `Lab Teste` e a data; clique em **Solicitar laboratório**. | Aviso "Laboratório externo solicitado."; situação **Aguardando laboratório externo**; o formulário de resultado fica desabilitado até haver laudo. | | |
| 10 | `gq@sgq.test` | No bloco do laboratório, escolha um arquivo qualquer (ex.: um `.txt` pequeno) em **Escolher arquivo do laudo** e clique em **Enviar laudo**. | Aviso "Laudo crítico enviado. Registre o resultado para retomar a investigação." | | |
| 11 | `gq@sgq.test` | Preencha **Identificação do laudo** (`L-1`), **Resultado recebido em** e **Resultado** (`Sem desvios.`) e clique em **Registrar resultado**. | Aviso "Resultado laboratorial registrado. A reclamação voltou para investigação."; situação **Em investigação**. | | Se tentar registrar o resultado antes de enviar o laudo: "Envie o laudo como anexo crítico antes de registrar o resultado." |
| 12 | `gq@sgq.test` | Abra **Prorrogar prazo (GQ ou RT)**; informe uma data posterior ao prazo atual, um motivo e clique em **Registrar prorrogação**. | Aviso "Prazo prorrogado e registrado no histórico."; o selo de prazo mostra a nova data. | | Data anterior ou igual ao prazo: "Informe uma nova data posterior ao prazo atual." |
| 13 | `gq@sgq.test` | Em **Conclusão da investigação**, preencha **O que foi investigado**, **Resultado**, **Tratamento aplicado**, **Data da resposta ao cliente** e **Resposta ao cliente**; clique em **Registrar conclusão**. | Aviso "Conclusão registrada. A reclamação está pronta para encerramento pela GQ."; situação **Aguardando conclusão**. | | |
| 14 | `cq@sgq.test` | Abra a RC. | Painel "Aguardando encerramento pela GQ"; sem botão **Encerrar processo**. | | |
| 15 | `gq@sgq.test` | Clique em **Encerrar processo** e confirme. | Aviso "Reclamação encerrada pela GQ."; situação **Encerrada**; todas as etapas concluídas. | | |
| 16 | `gq@sgq.test` | Abra **Reabrir reclamação**, escreva uma justificativa curta (`curta`) e confirme. | O navegador ou o servidor recusa: a justificativa precisa de ao menos 10 caracteres. | | |
| 17 | `gq@sgq.test` | Reabra com `Cliente trouxe fato novo relevante.` | Aviso "Reclamação reaberta. Registre uma nova conclusão antes do encerramento pela GQ."; situação **Em investigação**. | | |
| 18 | `gq@sgq.test` | No detalhe, role até **Histórico de alterações**. | Linha do tempo legível: "Registro criado", mudanças de **Situação** (ex.: Em investigação → Aguardando conclusão), prazo prorrogado etc. Sem nomes técnicos de código. | | |
| 19 | `gq@sgq.test` | Volte à lista **Reclamações**; clique no atalho **Em investigação**. | A lista filtra e o atalho mostra a contagem; digite `zzzz` na busca: aparece o estado vazio com orientação. | | |

## Defeitos conhecidos / observações

- Concluir a investigação e solicitar laboratório são aceitos pelo servidor para qualquer perfil (decisão D1 em aberto); a tela mostra os formulários a todos.
- Prorrogar uma RC já encerrada é recusado pela tela (formulário oculto), mas uma requisição manipulada ainda é aceita.
