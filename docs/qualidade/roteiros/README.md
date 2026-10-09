# Roteiros de teste manual

Estes roteiros permitem testar o SGQ **à mão**, tela por tela, sem conhecimento técnico. Cada passo diz quem executa (qual conta), o que fazer e o que deve acontecer. Use **somente dados fictícios**.

## 1. Preparar o ambiente (uma vez)

1. Tenha o PostgreSQL 17 e o SDK .NET 10 instalados. O PostgreSQL **não precisa estar ligado** nem ter senha conhecida: se ele não responder, o script cria um PostgreSQL próprio da demonstração (veja abaixo).
2. Abra o PowerShell na pasta do projeto e execute:

   ```powershell
   powershell -ExecutionPolicy Bypass -File scripts\executar-demo.ps1
   ```

   O script cria o banco de demonstração `sgq_demo_test`, aplica as migrations, carrega dados fictícios, cria **uma conta para cada perfil** e inicia o sistema em `http://localhost:5024`. Se o PostgreSQL instalado estiver parado e você não for Administrador, o script usa um PostgreSQL próprio (porta 54329, sem senha, acessível só neste computador), guardado em `%LOCALAPPDATA%\SGQ\postgres-demo`; ele desliga junto com o sistema.
3. No final o script mostra as contas e **a senha** (gerada na hora; vale para todas as contas de demonstração):

   | Conta | Perfil |
   | --- | --- |
   | `admin@sgq.test` | Administrador |
   | `gq@sgq.test` | Garantia da Qualidade (GQ) |
   | `rt@sgq.test` | Responsável Técnico (RT) |
   | `cq@sgq.test` | Controle de Qualidade (CQ) |
   | `auditor@sgq.test` | Auditor |

4. Para testar com várias pessoas ao mesmo tempo, use um navegador diferente (ou janela anônima) para cada conta, ou saia e entre de novo a cada troca de perfil.

Opções do script:

| Opção | Para que serve |
| --- | --- |
| `-Recriar` | Apaga o banco de demonstração e recomeça do zero (pede confirmação digitando `SIM`). Use antes de refazer um roteiro do início. |
| `-ComEmail` | Liga o Mailpit (caixa de entrada de teste em `http://localhost:8025`) para o roteiro de notificações. |
| `-SomentePreparar` | Prepara tudo mas não inicia o sistema. |
| `-Servidor`, `-Porta`, `-Usuario` | Se o PostgreSQL não estiver em `localhost:5432` com o usuário `postgres`. |
| `-PostgresPortatil`, `-PortaPortatil` | Força o uso do PostgreSQL próprio da demonstração (e muda a sua porta, padrão 54329). |

O script usa variáveis de ambiente só desta janela do PowerShell: ele **não altera** seus User Secrets nem o seu banco de desenvolvimento (`sgq_dev`). Para encerrar o sistema, pressione `Ctrl + C` na janela.

## 2. Ordem recomendada

| Ordem | Roteiro | Tempo | O que cobre |
| --- | --- | --- | --- |
| 1 | [AUTH-001 — Acesso inicial](auth-001-acesso-inicial.md) | 15 min | login, bloqueio, solicitar acesso, senha |
| 2 | [SEC-001 — Acesso por perfil](sec-001-acesso-por-perfil.md) | 30 min | o que cada perfil vê e consegue fazer |
| 3 | [USR-001 — Usuários e perfis](usr-001-usuarios-e-perfis.md) | 15 min | atribuir perfis, proteção do último Administrador |
| 4 | [CAD-001 — Cadastros](cad-001-cadastros.md) | 20 min | clientes, produtos e lotes |
| 5 | [CAL-001 — Calendário e prazos](cal-001-calendario-e-prazos.md) | 25 min | dias não úteis e cálculo de prazos |
| 6 | [RC-001 — Fluxo da Reclamação](rc-001-fluxo-completo-da-reclamacao.md) | 40 min | do registro ao encerramento e reabertura |
| 7 | [NC-001 — Fluxo da Não Conformidade](nc-001-fluxo-completo-da-nao-conformidade.md) | 60 min | investigação, ações, aprovações, CQ |
| 8 | [REC-001 — Fluxo do Recall](rec-001-fluxo-completo-do-recall.md) | 60 min | recolhimento, retornos, destinação |
| 9 | [ANX-001 — Anexos](anx-001-anexos.md) | 25 min | envio, vínculo, anulação, download |
| 10 | [REL-001 — Relatórios](rel-001-relatorios-e-exportacoes.md) | 20 min | filtros e exportações |
| 11 | [NOT-001 — Notificações por e-mail](not-001-notificacoes-por-email.md) | 30 min | e-mails de cada evento (usa `-ComEmail`) |
| 12 | [UI-001 — Usabilidade e telas pequenas](ui-001-usabilidade-e-responsividade.md) | 45 min | celular, teclado, mensagens |

Tempo total aproximado: **6 horas e 25 minutos**. Se tiver pouco tempo, faça 1, 2, 6, 7 e 8.

Entre um roteiro e outro, se algo tiver ficado em estado estranho, rode o script com `-Recriar`.

## 3. Como registrar o resultado

- Em cada tabela marque **✔** (passou) ou **✘** (falhou) na coluna "Resultado". Em caso de **✘**, escreva em "Observações" o que apareceu de diferente.
- Estados de cada roteiro: **Não executado**, **Aprovado**, **Reprovado** ou **Bloqueado** (não deu para executar).
- Preencha o [modelo de registro de execução](modelo-de-registro-de-execucao.md) ao final.

### Como relatar um defeito

Anote, de forma curta: **onde** (tela e endereço), **quem** (conta/perfil), **passos** para repetir, **resultado esperado**, **resultado obtido** e uma captura de tela (sem dados reais). Defeitos de segurança **não** devem ir para issue pública: veja [SECURITY.md](../../../SECURITY.md).

## 4. O que não é falha

Alguns comportamentos conhecidos aguardam decisão da Qualidade e estão descritos em [decisões pendentes](../../gestao/decisoes-pendentes.md) e em cada roteiro, em "Defeitos conhecidos". Por exemplo: várias ações de escrita ainda são aceitas pelo servidor para qualquer perfil, mesmo quando a tela esconde o botão. Não os registre como defeito novo.

## 5. Sobre estes roteiros

Os passos foram escritos a partir das telas e mensagens reais do sistema e seus principais fluxos são conferidos por scripts automatizados de navegador (Playwright) em cada versão. Se um texto de botão ou mensagem estiver diferente do roteiro, o roteiro está desatualizado: registre e corrija o roteiro.
