# SEC-001 — Acesso por perfil

## Pré-condição

Banco de teste e contas fictícias: uma sem perfil, uma para cada perfil (Administrador, GQ, RT, CQ e Auditor).

## Passos

1. Entre com a conta sem perfil e abra `/`, `/Clientes`, `/Reclamacoes`, `/Recalls` e `/Usuarios`.
2. Entre como cada perfil e tente: validar uma RC, encerrar uma NC, aprovar um Recall, decidir uma divergência (CQ), reabrir uma NC (Auditor), abrir `/Usuarios` e `/Relatorios`.
3. Tente baixar o anexo anulado com GQ, Auditor e RT.
4. Envie por requisição manipulada um POST de ação restrita com uma conta sem o perfil.

## Resultado esperado

- a conta sem perfil recebe 403 em todas as rotas de negócio;
- cada ação restrita só é aceita pelos perfis documentados e recusada com 403 para os demais, mesmo sem o botão na interface;
- o download de anexo anulado é permitido apenas a GQ, Administrador e Auditor.

Ações de escrita não críticas (cadastros, abertura de RC e Recall) ainda aceitam qualquer perfil; essa lacuna está em DEM-2026-101 e não deve ser reportada como falha deste roteiro.

A parte de acesso por perfil e a ausência de perfil têm cobertura automatizada em `AcessoPorPerfilTests` (autenticação simulada); este roteiro continua necessário para validar o login real por cookie e as ações de escrita por interface.
