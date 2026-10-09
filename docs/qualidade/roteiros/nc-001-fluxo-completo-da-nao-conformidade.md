# NC-001 — Fluxo completo da Não Conformidade

**Objetivo:** percorrer a NC da abertura ao encerramento: investigação, plano de ação, eficácia, aprovações de RT e GQ em contas distintas, divergência decidida pelo CQ, reabertura e prorrogação.
**Perfis envolvidos:** GQ, RT, CQ, Administrador e Auditor.
**Tempo estimado:** 60 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1`. O roteiro cria a própria NC (anote o código no passo 3).

## Dados que serão usados

| Item | Valor |
| --- | --- |
| Origem / área | Inspeção / `Qualidade` |
| Classificação | **Maior** (prazo calculado em 15 dias úteis) e, no passo 5, **Crítica** (exige data-alvo) |
| Descrição | `Teste de roteiro: etiqueta ilegível em lote.` |
| Contas | `gq@`, `rt@`, `cq@`, `admin@`, `auditor@` (domínio `sgq.test`) |

## Passos

### A. Abertura

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | `rt@sgq.test` | Abra **Não conformidades**. | A lista abre, mas **não** há o botão **Nova não conformidade** (só GQ e Administrador abrem). | | |
| 2 | `gq@sgq.test` | Clique em **Nova não conformidade**. Em **Classificação e prazo** leia a explicação dos prazos. | Maior = 15 dias úteis, Menor = 30 dias úteis, Crítica exige data-alvo; o campo **Data-alvo** só aparece para Crítica. | | |
| 3 | `gq@sgq.test` | Escolha origem **Inspeção**, área `Qualidade`, classificação **Maior**, descreva o desvio e salve. | Aviso "Não Conformidade NC-AAAA-00000N criada."; situação **Em investigação**; prazo calculado. **Anote o código.** | | |
| 4 | `gq@sgq.test` | Abra **Nova não conformidade**, escolha **Crítica** e salve sem data-alvo. | Mensagem de validação pedindo a data-alvo; nada é salvo. | | |
| 5 | `gq@sgq.test` | Repita com **Crítica** e uma data-alvo futura. | NC criada com selo **Crítica**; o selo de prazo mostra a data informada. | | Pode ser descartada: a NC dos próximos passos é a do passo 3. |

### B. Investigação e plano de ação

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 6 | `gq@sgq.test` | Na NC do passo 3, confira o painel "Próxima ação" e o trilho de etapas. | Etapa **Investigação** em andamento (etapa 2 de 6). | | |
| 7 | `gq@sgq.test` | Em **Investigação**, deixe campos vazios e clique em **Salvar investigação**. | Erro "Preencha todos os campos da investigação."; a situação não muda. | | |
| 8 | `gq@sgq.test` | Preencha **Contenção**, **Investigação**, **Causa provável**, **Causa raiz** e **Método de análise** (ex.: `5 Porquês`) e salve. | Aviso "Investigação registrada. Inclua e conclua as ações necessárias."; situação **Em tratamento**. | | |
| 9 | `gq@sgq.test` | Em **Plano de ação**, preencha **O que será feito**, **Responsável** e **Prazo** (data futura) e clique em **Adicionar ação**. Repita para uma segunda ação. | As duas ações aparecem na tabela, marcadas como obrigatórias e pendentes, com selo de prazo. | | Uma ação com prazo no passado aparece destacada como em atraso. |
| 10 | `gq@sgq.test` | Com ações pendentes, tente **Eficaz: enviar para aprovação**. | O botão está desabilitado ou o servidor responde "Conclua todas as ações obrigatórias antes de avaliar a eficácia." | | |
| 11 | `gq@sgq.test` | Conclua cada ação com **Concluir**, informando a evidência (ex.: `Lista de presença`). | A ação mostra a data de conclusão e a evidência; ao concluir todas, a avaliação de eficácia é liberada. | | |

### C. Eficácia e aprovação (RT e GQ em contas distintas)

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 12 | `gq@sgq.test` | Clique em **Eficaz: enviar para aprovação** e confirme. | Aviso "Eficácia registrada. A NC aguarda aprovações."; situação **Aguardando aprovação**; dois cartões: **Responsável Técnico (RT)** e **Garantia da Qualidade (GQ)**, ambos **Pendente**. | | |
| 13 | `admin@sgq.test` | Abra a NC. | Não há botões de voto; a tela explica que o Administrador não vota. | | |
| 14 | `auditor@`, `cq@` | Abra a NC. | Também sem botões de voto, com a explicação. | | |
| 15 | `rt@sgq.test` | Clique em **Aprovar como RT** e confirme. | O cartão do RT mostra **Aprovado**, "Emitido por `rt@sgq.test`". A situação continua **Aguardando aprovação**. | | |
| 16 | `rt@sgq.test` | Reabra a NC: os botões do RT ainda aparecem? | Os botões de voto do RT somem (o parecer já foi dado). | | |
| 17 | `gq@sgq.test` | Clique em **Aprovar como GQ** e confirme. | O cartão da GQ mostra **Aprovado**; aparece o botão **Encerrar processo**. | | |
| 18 | `gq@sgq.test` | Clique em **Encerrar processo** e confirme. | Aviso "Não Conformidade encerrada."; situação **Encerrada**; todas as etapas concluídas. | | |

### D. Reprovação e divergência decidida pelo CQ

Crie uma segunda NC (como nos passos 3, 8, 9, 11 e 12) e deixe-a em **Aguardando aprovação**. Anote o código.

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 19 | `gq@sgq.test` | Clique em **Aprovar como GQ**. | Cartão GQ **Aprovado**. | | |
| 20 | `rt@sgq.test` | Clique em **Reprovar como RT** e confirme. | Situação **Aguardando decisão do CQ** (divergência). | | |
| 21 | `cq@sgq.test` | Abra a NC. | Aparecem o resumo da divergência, o campo **Justificativa da decisão** e os botões **Decisão favorável** e **Decisão desfavorável: voltar ao tratamento**. | | |
| 22 | `cq@sgq.test` | Escreva `curta` e tente **Decisão desfavorável**. | Erro "A decisão do CQ requer justificativa de ao menos 10 caracteres." | | |
| 23 | `cq@sgq.test` | Escreva `Decisão desfavorável fundamentada.` e clique em **Decisão desfavorável: voltar ao tratamento**. | A NC volta para **Em tratamento**; a justificativa e o autor ficam registrados. | | |
| 24 | `gq@sgq.test` | Conclua novamente a avaliação (**Eficaz: enviar para aprovação**) e abra a NC. | Nova rodada **limpa**: os dois pareceres aparecem **Pendente** (nada da rodada anterior fica marcado). | | |
| 25 | `gq@sgq.test` e `rt@sgq.test` | Repita a divergência (GQ aprova, RT reprova) e, com o CQ, escolha **Decisão favorável** com justificativa. | Situação volta a **Aguardando aprovação** e aparece **Encerrar processo** para a GQ; ela encerra normalmente. | | |

### E. Mesma conta nos dois pareceres

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 26 | `admin@sgq.test` | Em **Usuários**, dê a `rt@sgq.test` também o perfil **GQ** (mantendo RT). | Perfis atualizados. | | |
| 27 | `rt@sgq.test` | Em uma NC nova aguardando aprovação, clique em **Aprovar como RT**; depois tente votar como GQ. | Depois do primeiro voto os botões da GQ somem e a tela explica que a mesma conta não vota pelos dois perfis. | | |
| 28 | `admin@sgq.test` | **Desfaça** o passo 26 (remova o perfil GQ de `rt@sgq.test`). | `rt@sgq.test` volta a ter só RT. | | |

### F. Reabertura e prorrogação

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 29 | `cq@`, `admin@` | Abra a NC encerrada do passo 18. | **Não** existe o botão de reabrir (só GQ, RT e Auditor). | | |
| 30 | `auditor@sgq.test` | Em **Reabertura**, escreva `curta` e envie. | Erro "Informe uma justificativa de reabertura com pelo menos 10 caracteres." | | |
| 31 | `auditor@sgq.test` | Reabra com `Nova evidência exige revisão da investigação.` | Aviso "Não Conformidade reaberta. Revise a investigação, as ações e a eficácia antes das novas aprovações."; situação **Em investigação**; pareceres e decisão do CQ **zerados**. | | |
| 32 | `gq@sgq.test` | Em uma NC em andamento, abra **Prorrogar prazo**, informe uma data posterior ao prazo, motivo e confirme. | Aviso "Prazo prorrogado e registrado no histórico."; o selo de prazo mostra a nova data. | | |
| 33 | `admin@sgq.test` | Abra a mesma NC. | O formulário de prorrogação **não** é oferecido (só GQ e RT). | | |
| 34 | `gq@sgq.test` | Abra **Histórico de alterações** da NC. | Linha do tempo legível com as mudanças de situação, pareceres e prorrogação, em português. | | |

## Defeitos conhecidos / observações

- Investigação, plano de ação e eficácia são aceitos pelo servidor para qualquer perfil (decisão D1 em aberto); a tela mostra os formulários a todos.
- Uma reprovação de RT ou GQ com o outro parecer pendente devolve a NC direto para **Em tratamento**.
- Depois de votar, o parecer não pode ser trocado pela tela.
