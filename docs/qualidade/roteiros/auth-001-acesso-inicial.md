# AUTH-001 — Acesso inicial

## Objetivo

Verificar autenticação, bloqueio e comportamento de contas novas.

## Passos

1. Acesse `http://localhost:5024` sem sessão.
2. Abra uma rota protegida, como `/Clientes`.
3. Confirme o redirecionamento para o login.
4. Cadastre uma conta fictícia (`@sgq.test`).
5. Entre com a conta criada e abra `/Clientes`.
6. Saia e tente uma senha incorreta cinco vezes seguidas.

## Resultado esperado

- sem sessão, a página inicial e as rotas de negócio redirecionam ao login;
- a conta recém-cadastrada autentica, mas recebe 403 nos módulos até que um Administrador atribua um perfil;
- senha incorreta apresenta mensagem genérica;
- após cinco falhas a conta fica bloqueada por 15 minutos.

A confirmação de e-mail, a aprovação administrativa formal e o MFA ainda não existem (DEM-2026-005).
