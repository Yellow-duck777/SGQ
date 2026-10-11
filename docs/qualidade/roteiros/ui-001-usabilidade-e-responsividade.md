# UI-001 — Usabilidade, teclado e telas pequenas

**Objetivo:** conferir, tela por tela, se o sistema é claro, elegante e fácil de usar em computador e em celular, e se dá para operá-lo só com o teclado.
**Perfis envolvidos:** GQ (`gq@sgq.test`) para quase tudo; Administrador para a tela de usuários.
**Tempo estimado:** 45 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1`.

## Como simular o celular

No navegador (Chrome ou Edge), pressione **F12**, depois **Ctrl + Shift + M** (modo dispositivo) e escolha uma largura de **390 px** (ex.: "iPhone 12"). Ou reduza a janela até ficar bem estreita. Volte para tela larga (1440 px) quando o roteiro pedir.

## Checklist por tela

Para cada tela da tabela, confira os 6 itens e marque ✔ ou ✘ na coluna correspondente.

1. **Largura total:** texto legível, nada cortado, **sem barra de rolagem horizontal** na página.
2. **Celular (390 px):** tabelas viram cartões, menu abre pelo botão de menu, botões fáceis de tocar.
3. **Português:** sem texto em inglês e sem nomes de código (como `AguardandoAprovacao`).
4. **Clareza:** dá para saber em qual situação está o processo e o que fazer a seguir.
5. **Mensagens:** erros aparecem junto ao campo e avisos de sucesso/erro aparecem em destaque.
6. **Teclado:** com a tecla **Tab** o foco passa por todos os botões e links em ordem lógica, com contorno visível.

| Nº | Tela (como chegar) | 1 | 2 | 3 | 4 | 5 | 6 | Observações |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | Login (`/`) sem sessão | | | | | | | |
| 2 | Solicitar acesso | | | | | | | |
| 3 | Esqueci minha senha | | | | | | | |
| 4 | Visão geral (logado como GQ) | | | | | | | "Precisa da sua atenção", cartões de RC/NC/Recall e últimos processos |
| 5 | Lista de Reclamações | | | | | | | atalhos por situação, busca, "Mais filtros" |
| 6 | Nova reclamação | | | | | | | seções, ajuda, seleção de lotes |
| 7 | Detalhe de RC (uma por situação) | | | | | | | trilho de etapas, "Próxima ação" |
| 8 | Lista e detalhe de NC | | | | | | | plano de ação, pareceres lado a lado |
| 9 | Nova NC | | | | | | | |
| 10 | Lista, novo e detalhe de Recall | | | | | | | retornos, encerramento |
| 11 | Clientes, Produtos e Lotes (lista e formulário) | | | | | | | |
| 12 | Calendário e novo dia não útil | | | | | | | |
| 13 | Relatórios | | | | | | | |
| 14 | Usuários (como Administrador) | | | | | | | |
| 15 | Minha conta e Alterar senha | | | | | | | |
| 16 | Página de acesso negado (digite `/Usuarios` como GQ) | | | | | | | |

## Passos específicos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | `gq@sgq.test` | Em um detalhe de RC aguardando validação, clique em **Validar e criar NC**. | Abre uma janela de confirmação com **Cancelar** e **Confirmar**; **Esc** fecha sem enviar; o foco fica dentro da janela. | | |
| 2 | `gq@sgq.test` | Confirme e, logo depois do clique, tente clicar de novo no mesmo botão. | O botão fica desabilitado ("Enviando…"): o formulário não é enviado duas vezes. | | |
| 3 | `gq@sgq.test` | Execute qualquer ação com sucesso (ex.: prorrogar um prazo). | Um aviso verde aparece e some sozinho em cerca de 6 segundos; um aviso de erro (vermelho) **não** some sozinho e tem botão de fechar. | | |
| 4 | `gq@sgq.test` | Na lista de RC, clique na **linha** (não só no código). | Abre o detalhe. Ctrl + clique abre numa nova aba. | | |
| 5 | `gq@sgq.test` | Em 390 px, abra o menu pelo botão no topo. | O menu lateral abre sobre a tela com fundo escurecido; **Esc** ou clique fora fecha; o foco volta ao botão de menu. | | |
| 6 | `gq@sgq.test` | Abra o menu do usuário (canto superior direito). | Mostra nome, perfil e links **Minha conta**, **Alterar senha** e **Sair**; **Esc** fecha. | | |
| 7 | `gq@sgq.test` | No celular, abra o detalhe de um Recall em recolhimento. | O trilho de etapas vira uma lista vertical; o formulário "Início do recolhimento" cabe na tela. | | |
| 8 | `gq@sgq.test` | Na tela de cadastros, use a busca digitando devagar. | A lista filtra em tempo real e o contador atualiza. | | |
| 9 | Qualquer | Aproxime o zoom do navegador para 200 % (**Ctrl +**). | O conteúdo continua utilizável (reorganiza, sem perder botões). | | |

## Defeitos conhecidos / observações

- As mensagens nativas de validação do navegador (por exemplo "Please select an item") aparecem no idioma do navegador; use o navegador em português para vê-las traduzidas.
- Foi feita uma verificação automatizada em Chromium; navegadores diferentes (Safari, Firefox) e leitores de tela não foram testados.
