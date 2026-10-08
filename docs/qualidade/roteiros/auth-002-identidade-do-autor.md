# AUTH-002 — Identidade do autor e dos pareceres

## Pré-condição

Banco de teste, contas fictícias com perfis GQ, RT e Administrador, uma NC pronta para aprovação.

## Passos

1. Entre como usuário comum com perfil e abra uma RC; confirme que o autor exibido é a conta usada.
2. Entre como RT e aprove a NC pelo perfil RT.
3. Entre como GQ e aprove a mesma NC pelo perfil GQ.
4. Consulte o histórico de auditoria do processo.

## Resultado esperado

- cada registro mostra a conta que realizou a ação, nunca um nome digitado;
- o parecer RT e o parecer GQ mostram contas distintas;
- o histórico de auditoria não contém dados de usuários do Identity.

## Cenário negativo

Com uma conta que tenha os perfis RT e GQ, tente emitir os dois pareceres: o segundo deve ser recusado. Com o Administrador, tente aprovar como RT ou GQ: deve ser recusado.
