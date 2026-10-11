# USR-001 — Usuários e perfis

**Objetivo:** conferir a atribuição de perfis pelo Administrador, o destaque de contas sem perfil e a proteção contra ficar sem Administrador.
**Perfis envolvidos:** Administrador (`admin@sgq.test`) e uma conta nova.
**Tempo estimado:** 15 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1`.

## Dados que serão usados

| Conta | Perfil inicial |
| --- | --- |
| `admin@sgq.test` | Administrador (único Administrador do banco de demonstração) |
| `conta.nova@sgq.test` | (criada no passo 1) sem perfil |

## Passos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | Visitante | Cadastre `conta.nova@sgq.test` em **Solicitar acesso** e saia. | A conta é criada e fica sem acesso aos módulos. | | |
| 2 | `admin@sgq.test` | Entre e abra **Usuários** no menu lateral. | A lista mostra os usuários, a situação (Ativo/Bloqueado) e os perfis em português. No topo há o destaque **Aguardando atribuição de perfil** com `conta.nova@sgq.test`. | | |
| 3 | `admin@sgq.test` | Abra **O que cada perfil pode fazer**. | Cada perfil tem uma explicação; o Administrador "não emite parecer como RT nem como GQ" e "não prorroga prazos". | | |
| 4 | `admin@sgq.test` | Em `conta.nova@sgq.test`, clique em **Editar perfis**, marque **Auditor** e salve. | Aviso de sucesso "Perfis de `conta.nova@sgq.test` atualizados."; a conta sai do destaque e mostra o selo Auditor. | | |
| 5 | `conta.nova@sgq.test` | Entre com a conta. | Agora acessa o sistema como Auditor (menu com Relatórios, sem Usuários). | | Se a sessão antiga continuar aberta, saia e entre de novo. |
| 6 | `admin@sgq.test` | Edite a mesma conta e marque também **RT**; salve. | A conta passa a ter dois perfis (Auditor e RT). | | |
| 7 | `admin@sgq.test` | Edite a conta e desmarque todos os perfis; salve (confirme o aviso, se houver). | Aviso informando que a conta ficará bloqueada até receber um perfil; ela volta ao destaque do topo. | | |
| 8 | `conta.nova@sgq.test` | Tente entrar e abrir `/Reclamacoes`. | Mensagem de conta sem perfil; nada é exibido. | | |
| 9 | `admin@sgq.test` | Em **Editar perfis** da **sua própria conta** (`admin@sgq.test`), desmarque **Administrador** e salve. | Erro: "Não é possível remover o perfil Administrador de `admin@sgq.test`: é o único Administrador do sistema…"; o perfil é mantido. | | Proteção para ninguém ficar sem poder atribuir perfis. |
| 10 | `admin@sgq.test` | Atribua o perfil **Administrador** a `gq@sgq.test` (mantendo GQ), salve. Depois remova o perfil Administrador de `admin@sgq.test`. | Agora existe outro Administrador, então a remoção é aceita. | | Para continuar os demais roteiros, devolva o perfil Administrador a `admin@sgq.test` usando a conta `gq@sgq.test` (agora Administrador), ou rode o script com `-Recriar`. |

## Defeitos conhecidos / observações

- Não existe tela para o Administrador redefinir a senha de outro usuário nem para bloquear/desbloquear contas.
- O sistema não guarda o nome da pessoa: a saudação é derivada do e-mail (`maria.silva@...` vira "Maria").
