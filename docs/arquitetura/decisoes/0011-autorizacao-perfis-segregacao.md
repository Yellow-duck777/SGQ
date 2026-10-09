# ADR 0011 — Autorização por perfis, política padrão e de fallback e segregação RT/GQ

- Estado: aceito (DEM-2026-008)
- Data: 2026-10-08

## Contexto

O cadastro público existe, e a avaliação de segurança mostrou que qualquer conta autenticada, mesmo sem perfil, acessava os módulos. As aprovações de RT e GQ podiam ser dadas pela mesma conta ou pelo Administrador.

## Decisão

- Cinco perfis fixos, definidos em `Roles`: Administrador, GQ, RT, CQ e Auditor, atribuídos pelo Administrador.
- Uma única política (`RequireAuthenticatedUser` e `RequireRole(Roles.Todos)`) é definida como política **padrão** (aplicada a `[Authorize]` simples, usado pelos controllers) e como política de **fallback** (rotas sem atributo). Conta sem perfil recebe 403 em tudo, exceto telas públicas marcadas com `[AllowAnonymous]` (login, cadastro, estilos e erro).
- Transições críticas usam `[Authorize(Roles = ...)]`.
- Segregação RT/GQ: o parecer RT exige o perfil RT e o parecer GQ exige o perfil GQ; o Administrador não aprova como RT ou GQ; o mesmo usuário não emite os dois pareceres. O usuário de cada parecer é gravado em `UsuarioParecerRt` e `UsuarioParecerGq` (NC e Recall).

## Alternativas

- Autorização por política e permissão (claims por ação): mais flexível, depende da matriz papel x ação que a Qualidade ainda não validou (DEM-2026-101).
- Permitir o Administrador como aprovador: rejeitado pela exigência de contas distintas.
- Confiar apenas no filtro de interface: rejeitado (RF-004).

## Consequências

- Contas novas ficam inativas funcionalmente até a atribuição de perfil.
- Ações de escrita não críticas (cadastros, RC, Recall, investigação) ainda aceitam qualquer perfil (DEM-2026-101).
- Contas existentes com os dois perfis (RT e GQ) deixam de poder fechar sozinhas uma aprovação dupla.
- O CQ e o Administrador ainda podem decidir divergências (DEM-2026-108).

## Errata (2026-10-08)

A primeira versão desta decisão (PR #3) definia apenas a política de fallback. Um `[Authorize]` simples, usado por todos os controllers, não usa o fallback: ele usa a política padrão. Portanto contas sem perfil ainda acessavam os módulos. O defeito foi encontrado pelo teste de integração `AutenticadoSemPerfil_RecebeAcessoNegado` e corrigido definindo a mesma política como padrão e como fallback em `Program.cs`. O documento agora reflete a implementação corrigida.

## Verificação

Os testes de `SGQ.IntegrationTests` exercitam a política contra o pipeline real (anônimo, sem perfil, cada perfil, `/Usuarios` só Administrador). A autenticação nesses testes é simulada por um esquema de teste com cabeçalhos; o fluxo de login por cookie não é exercitado.
