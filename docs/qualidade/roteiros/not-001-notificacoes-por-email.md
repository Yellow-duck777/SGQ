# NOT-001 — Notificações por e-mail

**Objetivo:** conferir que cada evento importante do fluxo envia o e-mail certo para os perfis certos, sem enviar nada para fora da sua máquina.
**Perfis envolvidos:** GQ, RT, CQ (destinatários) e Administrador (apenas para consultar).
**Tempo estimado:** 30 minutos.
**Pré-condição:** sistema iniciado **com e-mail de teste**: `powershell -ExecutionPolicy Bypass -File scripts\executar-demo.ps1 -ComEmail`. O script inicia o **Mailpit** (caixa de entrada de teste do Laragon) e aponta o SMTP do sistema para ele. A caixa de entrada abre em `http://localhost:8025`. Se o Mailpit não for encontrado, o script avisa e o roteiro não pode ser executado.

## Como ler a caixa de entrada

- Cada e-mail tem o assunto `SGQ: <evento> — <código do processo>` e o corpo com o código e a descrição.
- Os destinatários reais ficam em cópia oculta; no Mailpit eles aparecem nos detalhes do e-mail (campo "Bcc" ou "To", conforme a versão). O perfil que recebe depende do evento (tabela abaixo).
- O sistema envia **no máximo um e-mail por tipo de evento, por processo e por dia** (deduplicação): repetir a mesma ação no mesmo dia não gera outro e-mail.

## Eventos e destinatários

| Evento (o que você faz) | Assunto do e-mail | Quem recebe |
| --- | --- | --- |
| Registrar uma RC | `SGQ: RC aguardando validação — RC-…` | GQ |
| GQ valida a RC (cria a NC) | `SGQ: RC validada e NC criada — RC-…` | GQ e RT |
| GQ valida uma RC como **Crítica** (ou abre NC Crítica) | `SGQ: NC crítica — NC-…` | GQ, RT e CQ |
| Solicitar laboratório externo (RC/NC) | `SGQ: laboratório externo solicitado — …` | GQ e RT |
| Registrar o resultado do laboratório | `SGQ: resultado de laboratório recebido — …` | GQ e RT |
| NC com eficácia comprovada | `SGQ: NC aguardando aprovação — NC-…` | RT e GQ |
| Divergência RT x GQ (NC ou Recall) | `SGQ: divergência requer decisão do CQ — …` | CQ |
| Decisão do CQ registrada | `SGQ: decisão do CQ registrada — …` | RT e GQ |
| Abrir um Recall aplicável | `SGQ: Recall iniciado — REC-…` | GQ, RT e CQ |
| Recall aprovado (ou decisão favorável do CQ) | `SGQ: Recall aprovado — REC-…` | GQ, RT e CQ |
| Prorrogar um prazo | `SGQ: prazo prorrogado — …` | GQ e RT |
| Encerrar um processo | `SGQ: processo encerrado — …` | RC: GQ; NC: GQ e RT; Recall: GQ, RT e CQ (e quem abriu/decidiu) |

## Passos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | — | Abra `http://localhost:8025`. | A caixa de entrada do Mailpit abre vazia (ou com e-mails de uma execução anterior; use "Delete all" para limpar). | | |
| 2 | `gq@sgq.test` | Registre uma RC nova (roteiro RC-001, passos 1 a 4). | Chega **1** e-mail "RC aguardando validação" com o código da RC; destinatário GQ. | | |
| 3 | `gq@sgq.test` | Valide a RC como **Crítica**. | Chegam "RC validada e NC criada" (GQ, RT) e "NC crítica" (GQ, RT, CQ). | | |
| 4 | `gq@sgq.test` | Solicite laboratório externo na RC. | Chega "laboratório externo solicitado" (GQ, RT). | | |
| 5 | `gq@sgq.test` | Envie o laudo e registre o resultado. | Chega "resultado de laboratório recebido" (GQ, RT). | | |
| 6 | `gq@sgq.test` | Prorrogue o prazo da RC. | Chega "prazo prorrogado" (GQ, RT). | | |
| 7 | `gq@sgq.test` | Prorrogue o prazo **de novo no mesmo dia**. | **Não** chega um segundo e-mail (um por tipo, processo e dia). | | Limitação conhecida: a segunda prorrogação não é notificada. |
| 8 | `gq@sgq.test` | Conclua e encerre a RC. | Chega "processo encerrado" (GQ). | | |
| 9 | `gq@sgq.test`, `rt@sgq.test` | Em uma NC, deixe a eficácia comprovada (roteiro NC-001, passos 3 a 12). | Chega "NC aguardando aprovação" (RT, GQ). | | |
| 10 | `gq@sgq.test`, `rt@sgq.test` | GQ aprova e RT reprova. | Chega "divergência requer decisão do CQ" (CQ). | | |
| 11 | `cq@sgq.test` | Registre uma decisão do CQ com justificativa. | Chega "decisão do CQ registrada" (RT, GQ). | | |
| 12 | `gq@sgq.test` | Abra um Recall **aplicável** (roteiro REC-001, passo 3). | Chega "Recall iniciado" (GQ, RT, CQ). | | |
| 13 | `rt@sgq.test`, `gq@sgq.test` | Aprove o recall como RT e como GQ. | Chega "Recall aprovado" (GQ, RT, CQ). | | |
| 14 | `gq@sgq.test` | Abra um Recall **não aplicável**. | **Nenhum** e-mail "Recall iniciado" (ele nasce encerrado). | | |
| 15 | — | Rode o sistema **sem** `-ComEmail` (reinicie o script) e repita o passo 2. | O sistema funciona normalmente e nenhum e-mail é gerado; nada falha na tela. | | O envio é opcional. |

## Defeitos conhecidos / observações

- Eventos repetidos do mesmo tipo no mesmo dia para o mesmo processo (segunda prorrogação, nova divergência, reabrir e encerrar de novo) **não** geram novo e-mail (backlog: deduplicar por evento).
- O envio é feito durante a ação do usuário; se o servidor de e-mail estiver lento, a tela demora.
- Falha no envio é registrada no log e não impede a ação.
