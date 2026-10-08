# ADR 0011 — Autorização por perfis, política de fallback e segregação RT/GQ

- Estado: aceito (DEM-2026-008)
- Data: 2026-10-08

## Contexto

O cadastro público existe, e a avaliação de segurança mostrou que qualquer conta autenticada, mesmo sem perfil, acessava os módulos. As aprovações de RT e GQ podiam ser dadas pela mesma conta ou pelo Administrador.

## Decisão

- Cinco perfis fixos, definidos em `Roles`: Administrador, GQ, RT, CQ e Auditor, atribuídos pelo Administrador.
- Uma política de fallback exige usuário autenticado com perfil; conta sem perfil recebe 403 em tudo, exceto telas públicas explícitas (login e cadastro).
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
