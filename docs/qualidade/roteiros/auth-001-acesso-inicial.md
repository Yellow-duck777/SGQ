# AUTH-001 — Acesso inicial

## Objetivo

Verificar o comportamento de autenticação existente antes do hardening planejado.

## Passos

1. Acesse `http://localhost:5024` sem sessão.
2. Abra uma rota protegida, como `/Clientes`.
3. Confirme o redirecionamento para login.
4. Registre uma conta fictícia.
5. Entre com a conta criada.
6. Saia e tente uma senha incorreta.

## Resultado atual esperado

- a página inicial ainda é pública;
- rotas de negócio exigem autenticação;
- cadastro padrão permite acesso após login;
- senha incorreta apresenta mensagem genérica.

Esses resultados descrevem a baseline, não o comportamento de segurança aprovado para o produto.
