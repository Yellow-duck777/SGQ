# AUTH-001 — Acesso inicial (entrar, sair, cadastro e senha)

**Objetivo:** conferir login, mensagens de erro, bloqueio por tentativas, solicitação de acesso e troca de senha.
**Perfis envolvidos:** nenhum (visitante) e `gq@sgq.test`.
**Tempo estimado:** 15 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1` (veja o [índice](README.md)); endereço `http://localhost:5024`. Use um navegador em janela normal e feche as sessões abertas antes de começar.

## Dados que serão usados

| Conta | Perfil | Uso neste roteiro |
| --- | --- | --- |
| `gq@sgq.test` | Garantia da Qualidade | login válido e troca de senha |
| `novo.usuario@sgq.test` | (será criada no passo 9) | solicitar acesso, sem perfil |
| `rt@sgq.test` | Responsável Técnico | tentativas inválidas (passos 5 a 7) |

A senha das contas de demonstração é a exibida pelo script ao final da preparação.

## Passos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | Visitante | Abra `http://localhost:5024/` sem estar logado. | A tela de login aparece ("Acesse sua conta"), com campos **E-mail corporativo** e **Senha**, botão **Entrar no SGQ** e links **Esqueci minha senha** e **Solicitar cadastro**/**Solicitar acesso**. | | |
| 2 | Visitante | Digite na barra de endereço `http://localhost:5024/Reclamacoes`. | Você é levado ao login (nenhuma lista é exibida). | | |
| 3 | Visitante | Clique em **Mostrar** ao lado da senha, digite qualquer texto e clique em **Ocultar**. | O texto da senha aparece e depois volta a ficar oculto. | | |
| 4 | Visitante | Clique em **Entrar no SGQ** com os dois campos vazios. | A tela mostra mensagens pedindo o e-mail e a senha; você continua no login. | | |
| 5 | `rt@sgq.test` | Informe o e-mail e uma senha **errada**; clique em **Entrar no SGQ**. | Mensagem genérica: "Não foi possível entrar. Verifique seu e-mail e senha." (não diz se o e-mail existe). | | |
| 6 | `rt@sgq.test` | Repita o passo 5 até a **5ª tentativa** com senha errada. | Na 5ª falha aparece a página "Conta temporariamente bloqueada por tentativas inválidas". | | A conta fica bloqueada por 15 minutos; **não** espere: use outra conta nos próximos passos. |
| 7 | `rt@sgq.test` | Tente entrar com a senha **correta** logo em seguida. | Continua bloqueada (mesma página de bloqueio). | | Para liberar antes do prazo, rode `scripts\executar-demo.ps1` de novo (redefine as senhas) ou use `-Recriar`. |
| 8 | `gq@sgq.test` | Entre com e-mail e senha corretos. | Abre a **Visão geral** com "Olá, Gq" e o selo do perfil; o menu lateral mostra Reclamações, Não conformidades, Recalls, Relatórios, Clientes, Produtos, Lotes e Calendário (sem "Usuários"). | | |
| 9 | Visitante | Saia (menu do usuário, canto superior direito, **Sair**; confirme). Na tela de login clique em **Solicitar acesso** e cadastre `novo.usuario@sgq.test` com uma senha forte (mínimo 6 caracteres, com maiúscula, minúscula, número e símbolo). | A conta é criada e você já entra, mas vê "Sua conta ainda não tem perfil de acesso" (sem menu lateral). | | A tela de cadastro avisa que só um Administrador libera o perfil. |
| 10 | `novo.usuario@sgq.test` | Digite `http://localhost:5024/Reclamacoes`. | Continua na mensagem de conta sem perfil; nenhuma lista aparece. | | |
| 11 | `novo.usuario@sgq.test` | Use **Sair e entrar de novo** (ou saia pelo menu). | Volta ao login. | | |
| 12 | Visitante | Em **Solicitar acesso**, tente cadastrar uma senha fraca (ex.: `abc`). | Mensagens de validação em português explicam os requisitos da senha. | | |
| 13 | Visitante | Clique em **Esqueci minha senha**, informe qualquer e-mail (existente ou não) e envie. | A mesma mensagem neutra aparece nos dois casos ("Pedido registrado"); nada revela se o e-mail existe. | | Sem servidor de e-mail configurado, nenhuma mensagem é entregue; isso é esperado hoje (veja o roteiro NOT-001 para testar e-mails). |
| 14 | `gq@sgq.test` | Entre, abra o menu do usuário e clique em **Minha conta**. | Aparece a página da conta com o e-mail e o perfil. | | |
| 15 | `gq@sgq.test` | Clique em **Alterar senha**, informe a senha atual, uma nova senha forte e a confirmação. | Aviso de sucesso; ao sair e entrar com a **nova** senha o acesso funciona. | | Anote a nova senha: ela passa a valer para esta conta de demonstração. |
| 16 | `gq@sgq.test` | Tente alterar a senha informando a senha atual errada. | Mensagem de erro em português; a senha não muda. | | |

## Defeitos conhecidos / observações

- A recuperação de senha por e-mail depende de envio de mensagens; sem SMTP e sem confirmação de e-mail ela não entrega nada (ver [decisões pendentes](../../gestao/decisoes-pendentes.md), D8).
- Cadastrar um e-mail já usado informa que ele já existe (comportamento do cadastro aberto).
