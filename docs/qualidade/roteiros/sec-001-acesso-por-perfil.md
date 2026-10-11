# SEC-001 — Acesso por perfil

**Objetivo:** confirmar que cada perfil enxerga e consegue fazer só o que lhe cabe, e que conta sem perfil não acessa nada, mesmo digitando endereços à mão.
**Perfis envolvidos:** Administrador, GQ, RT, CQ, Auditor e uma conta sem perfil.
**Tempo estimado:** 30 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1`. A conta sem perfil é criada no passo 1 (ou reaproveite a do roteiro AUTH-001).

## Dados que serão usados

| Conta | Perfil |
| --- | --- |
| `admin@sgq.test` | Administrador |
| `gq@sgq.test` | Garantia da Qualidade (GQ) |
| `rt@sgq.test` | Responsável Técnico (RT) |
| `cq@sgq.test` | Controle de Qualidade (CQ) |
| `auditor@sgq.test` | Auditor |
| `semperfil@sgq.test` | (nenhum; criada no passo 1) |

Processos de demonstração usados: uma RC aguardando validação (a primeira da lista de Reclamações), uma NC aguardando decisão do CQ e um Recall aguardando aprovação.

## Passos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | Visitante | Em **Solicitar acesso**, cadastre `semperfil@sgq.test`. | Entra e vê "Sua conta ainda não tem perfil de acesso". | | |
| 2 | `semperfil@sgq.test` | Digite, um por vez, `/`, `/Reclamacoes`, `/NaoConformidades`, `/Recalls`, `/Clientes`, `/Relatorios`, `/Usuarios`, `/Home/Privacy`. | Em todos aparece a mensagem de falta de perfil/permissão; nenhuma lista ou dado é exibido. | | |
| 3 | `semperfil@sgq.test` | Tente baixar um anexo digitando `/Anexos/Baixar/1`. | Acesso negado; nenhum arquivo é baixado. | | |
| 4 | `auditor@sgq.test` | Observe o menu lateral. | Visão geral, Reclamações, Não conformidades, Recalls, **Relatórios**, Clientes, Produtos, Lotes, Calendário. **Sem** "Usuários". | | |
| 5 | `auditor@sgq.test` | Abra a lista de Reclamações. | Não há botão **Nova reclamação**. | | O servidor ainda aceitaria a ação por requisição manipulada (decisão D1 em aberto); aqui você testa o que a **tela** oferece. |
| 6 | `auditor@sgq.test` | Abra o detalhe de uma NC encerrada. | Aparece **Reabrir não conformidade** (Auditor reabre NC). | | |
| 7 | `cq@sgq.test` | Observe o menu lateral. | **Sem** "Relatórios" e **sem** "Usuários". | | |
| 8 | `cq@sgq.test` | Digite `/Relatorios`. | Acesso negado ("Você não tem permissão para esta ação"), com botão para voltar. | | |
| 9 | `cq@sgq.test` | Abra a NC que aguarda a decisão do CQ. | Há dois botões: **Decisão favorável** e **Decisão desfavorável: voltar ao tratamento**, e o campo de justificativa. | | |
| 10 | `rt@sgq.test` | Abra a mesma NC. | Não há botões de decisão do CQ; a tela explica quem decide. | | |
| 11 | `rt@sgq.test` | Digite `/Relatorios` e `/Usuarios`. | Ambos negados. | | RT não vê relatórios nem usuários. |
| 12 | `gq@sgq.test` | Abra a RC aguardando validação. | Aparece o painel "Próxima ação: validar e classificar" com o botão **Validar e criar NC**. | | |
| 13 | `rt@sgq.test`, `cq@sgq.test`, `auditor@sgq.test` | Abra a mesma RC com cada conta. | O painel diz "Aguardando validação da GQ" e **não** há botão de validar. | | |
| 14 | `admin@sgq.test` | Abra o Recall aguardando aprovação. | Não há botões **Aprovar/Reprovar**; a tela explica que o voto é de RT e GQ em contas distintas e que o Administrador não vota. | | |
| 15 | `rt@sgq.test` | No mesmo Recall, observe os botões. | Há **Aprovar como RT** e **Reprovar como RT** (não aparecem os "como GQ"). | | |
| 16 | `admin@sgq.test` | Abra `/Usuarios`. | A lista de usuários e perfis aparece, com a seção "O que cada perfil pode fazer". | | |
| 17 | `admin@sgq.test` | Abra o calendário e as listas de Clientes, Produtos e Lotes. | Todas abrem normalmente. | | |
| 18 | `gq@sgq.test` | Digite `/Usuarios`. | Acesso negado (apenas Administrador). | | |
| 19 | Qualquer | Tente abrir a rota removida `http://localhost:5024/Reclamacoes/AvancarStatus/1` digitando no navegador. | Não existe mais (página não encontrada/ação inexistente); nenhuma RC muda de situação. | | Antes da correção qualquer usuário avançava uma RC até "Encerrada" por essa rota. |

## Defeitos conhecidos / observações

- Várias ações de escrita (por exemplo concluir a investigação de uma RC, registrar retornos de um Recall, cadastros) **ainda são aceitas pelo servidor para qualquer perfil**, mesmo quando a tela esconde o botão. A matriz de quem pode fazer o quê está em validação ([decisões pendentes](../../gestao/decisoes-pendentes.md), D1). Não registre isso como falha deste roteiro.
- O acesso por perfil (401/403/200) também tem cobertura automatizada (`AcessoPorPerfilTests`).
