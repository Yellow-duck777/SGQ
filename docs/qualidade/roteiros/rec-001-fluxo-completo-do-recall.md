# REC-001 — Fluxo completo do Recall

**Objetivo:** percorrer o Recall da abertura ao encerramento: decisão aplicável e não aplicável, aprovações RT e GQ, divergência decidida pelo CQ, recolhimento, retornos de produto (inclusive números com vírgula e com ponto), destinação, encerramento e reabertura.
**Perfis envolvidos:** GQ, RT, CQ, Administrador e Auditor.
**Tempo estimado:** 60 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1`. O roteiro cria os próprios recalls (anote os códigos nos passos 3 e 5).

## Dados que serão usados

| Item | Valor |
| --- | --- |
| Produto / lote | Produto Exemplo Alfa / ALFA-2026-001 |
| Quantidades | produzida `1000`, em estoque `300`, distribuída `600` |
| Clientes envolvidos | `Cliente fictício A` |
| Contas | `gq@`, `rt@`, `cq@`, `admin@`, `auditor@` (domínio `sgq.test`) |

## Passos

### A. Abertura

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | `gq@sgq.test` | Abra **Recalls** e clique em **Novo recall**; sem preencher nada, clique em **Registrar recall**. | Mensagens de validação em português; nada é salvo. | | |
| 2 | `gq@sgq.test` | Escolha o produto **Produto Exemplo Alfa** e abra a lista **Lote**. | Só aparecem lotes desse produto. | | Escolher o lote primeiro seleciona o produto dele. |
| 3 | `gq@sgq.test` | Preencha lote `ALFA-2026-001`, quantidades `1000`, `300` e `600`, **Clientes envolvidos**, **O que aconteceu**, **Risco potencial para o consumidor**, deixe a decisão **Aplicável**, escreva a **Justificativa da decisão** e salve. | Aviso "Recall REC-AAAA-00000N registrado."; situação **Em avaliação**; **anote o código**. | | O formulário confere se estoque + distribuído fecham com o produzido e avisa. |
| 4 | `gq@sgq.test` | Repita a criação, mas escolha a decisão **Não aplicável**. | Aparece o aviso "com a decisão 'Não aplicável' o recall é encerrado assim que for registrado…"; o botão passa a **Registrar e encerrar**. | | |
| 5 | `gq@sgq.test` | Salve o recall não aplicável. | Abre já com situação **Encerrado**, sem trilho de etapas ("Encerrado sem recolhimento"); responsável e data de encerramento aparecem. **Anote o código.** | | |

### B. Aprovações

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 6 | `rt@`, `cq@`, `auditor@` | Abra o recall aplicável (passo 3). | **Não** há o botão **Solicitar aprovação**; o painel explica quem faz. | | |
| 7 | `gq@sgq.test` | Clique em **Solicitar aprovação** e confirme. | Situação **Aguardando aprovação**; cartões do RT e da GQ **Pendente**. | | |
| 8 | `admin@sgq.test` | Abra o recall. | Sem botões **Aprovar/Reprovar**; explicação de que o Administrador não vota. | | |
| 9 | `rt@sgq.test` | Clique em **Aprovar como RT** e confirme. | Parecer do RT **Aprovado**, "Emitido por `rt@sgq.test`"; situação continua **Aguardando aprovação**. | | |
| 10 | `gq@sgq.test` | Clique em **Aprovar como GQ** e confirme. | Situação **Em recolhimento**. | | |

### C. Recolhimento, retornos e destinação

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 11 | `gq@sgq.test` | Em **Início do recolhimento**, tente **Iniciar retornos** com os campos vazios. | Erro "Registre bloqueio e as comunicações obrigatórias antes de iniciar os retornos." (ou o navegador exige os campos). | | |
| 12 | `gq@sgq.test` | Marque **Bloqueio do lote registrado**, preencha **Comunicação aos clientes** (`Carta enviada`), **Autoridade sanitária comunicada** (`Anvisa (fict.)`), **Protocolo da comunicação** (`PROTO-001`) e clique em **Iniciar retornos**. | Situação **Aguardando retorno**. | | |
| 13 | `gq@sgq.test` | Observe o botão **Concluir retornos e avançar**. | Está desabilitado enquanto não houver retornos. | | |
| 14 | `gq@sgq.test` | Em **Retornos de produto**, lance `Cliente A`, quantidade `200`, **Condição da embalagem** `Íntegra`; clique em **Registrar retorno**. | O retorno entra na tabela e o resumo soma 200 de 600 distribuídos. | | |
| 15 | `gq@sgq.test` | Lance `Cliente B` com quantidade `150,5` (vírgula). | Entra com 150,5; o total passa a 350,5. | | Se o campo não aceitar vírgula, teste `150.5` (ponto): os dois devem resultar em 150,5. Antes da correção o ponto virava 1505. |
| 16 | `gq@sgq.test` | Clique em **Concluir retornos e avançar** e confirme. | Situação **Em avaliação de destinação**. | | |
| 17 | `gq@sgq.test` | Preencha **Destinação** (`Descarte controlado`) e **Evidência** (`Termo 123`); clique em **Registrar destinação**. | Situação **Aguardando encerramento**; a lista de pré-requisitos mostra aprovações, comunicação regulatória e destinação cumpridas. | | |
| 18 | `rt@sgq.test` | Abra o recall. | **Não** há botão **Encerrar processo** (só GQ e Administrador). | | |
| 19 | `gq@sgq.test` | Clique em **Encerrar processo** e confirme. | Situação **Encerrado**; responsável e data aparecem. | | |

### D. Reabertura e nova rodada

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 20 | `auditor@sgq.test`, `cq@sgq.test` | Abra o recall encerrado. | Não há **Reabrir recall** (só GQ e RT). | | |
| 21 | `gq@sgq.test` | Abra **Reabrir recall**, escreva motivo/justificativa curtos e clique em **Confirmar reabertura**. | Erro "Informe o motivo e a justificativa da reabertura (mínimo de 10 caracteres cada)." | | |
| 22 | `gq@sgq.test` | Reabra com motivo `Nova informação relevante` e justificativa `Laudo novo recebido do laboratório`. | Aviso "Recall reaberto. A avaliação e as aprovações técnicas devem ser realizadas novamente."; situação **Em avaliação**. | | |
| 23 | `gq@sgq.test` | Solicite aprovação de novo e abra o recall. | Os pareceres de RT e GQ aparecem **Pendente** (nenhum parecer da rodada anterior). | | |

### E. Divergência decidida pelo CQ

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 24 | `gq@sgq.test` | Clique em **Aprovar como GQ**. | Parecer GQ **Aprovado**. | | |
| 25 | `rt@sgq.test` | Clique em **Reprovar como RT** e confirme. | Situação **Aguardando decisão do CQ**. | | |
| 26 | `cq@sgq.test` | Escreva `curta` na justificativa e clique em **Decisão desfavorável: voltar para avaliação**. | Erro "A decisão do CQ requer justificativa de ao menos 10 caracteres." | | |
| 27 | `cq@sgq.test` | Com justificativa válida, clique em **Decisão desfavorável: voltar para avaliação**. | Situação **Em avaliação**; decisão e autor registrados. | | |
| 28 | `gq@sgq.test` e `rt@sgq.test` | Solicite aprovação, GQ aprova e RT reprova de novo. | **Aguardando decisão do CQ**. | | |
| 29 | `cq@sgq.test` | Com justificativa válida, clique em **Decisão favorável: seguir para o recolhimento**. | Situação **Em recolhimento**. | | |
| 30 | `gq@sgq.test` | Percorra de novo recolhimento, retornos e destinação até **Aguardando encerramento** e clique em **Encerrar processo**. | O recall encerra normalmente, mesmo com as aprovações divergentes (vale a decisão favorável do CQ). | | Antes da correção ficava travado. |

### F. Prorrogação e anexos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 31 | `gq@sgq.test` | Em um recall em andamento, informe **Nova data**, **Motivo** e clique em **Registrar prorrogação**. | Aviso "Prazo prorrogado e registrado no histórico."; o selo de prazo mostra a nova data. | | |
| 32 | `admin@sgq.test` | Abra o mesmo recall. | O formulário de prorrogação não é oferecido. | | |
| 33 | `gq@sgq.test` | Em **Anexos**, escolha um arquivo pequeno e clique em **Enviar anexo**. | Aviso "Anexo enviado."; o arquivo aparece na lista com tamanho, autor e **Baixar**. | | |

## Defeitos conhecidos / observações

- Registrar operação, retornos e destinação são aceitos pelo servidor para qualquer perfil (decisão D1 em aberto).
- Ao reabrir um recall, os retornos e a destinação da rodada anterior permanecem registrados e entram no total da nova rodada.
- Reprovar por apenas um perfil devolve o recall à avaliação imediatamente, sem esperar o outro parecer.
