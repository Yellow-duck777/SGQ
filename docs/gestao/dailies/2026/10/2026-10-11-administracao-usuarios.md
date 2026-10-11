# Daily — Administração de usuários — 2026-10-11

## Identificação

- Demanda: `DEM-2026-119`
- Responsável: desenvolvimento
- Revisor técnico: `@ViniciusLgo`
- Validador da Qualidade: pendente
- Branch: `feat/administracao-usuarios-anulacao-nc`
- Estado: Em revisão

## Objetivo

Permitir que somente Administradores gerenciem contas locais de demonstração e seus perfis, sem exclusão física de usuários.

## Contexto

A tela **Usuários** já lista contas e atualiza perfis. Para testar os fluxos do SGQ, o responsável solicitou que Administradores também possam criar contas, editar perfis, redefinir senhas e bloquear ou desbloquear o acesso. Requisitos relacionados: `RF-003`, `RF-004`, `RF-005`, `RF-006`, `RS-003` e `RS-006`.

## Critérios de aceite

- [x] Somente Administrador acessa as ações de criar usuário, atualizar perfis, redefinir senha e bloquear ou desbloquear uma conta; o backend rejeita os demais perfis.
- [x] Administrador cria conta com e-mail, senha inicial e ao menos um perfil reconhecido.
- [x] Administrador atualiza os perfis, redefine a senha e bloqueia ou desbloqueia contas existentes.
- [x] Nenhuma conta é excluída fisicamente; o bloqueio preserva a conta e sua auditoria.
- [x] O sistema não permite deixar a aplicação sem Administrador.
- [x] Cada operação possui teste automatizado e não registra senha, hash ou token na auditoria.

## Fora do escopo

- exclusão física de usuários;
- convite, confirmação de e-mail, MFA e aprovação formal de contas;
- alteração da política geral de senha;
- anulação de Não Conformidade. Essa regra depende de definição da Qualidade sobre status, reabertura e efeitos no fluxo.

## Plano do dia

- [x] Preparar testes de autorização e operações administrativas.
- [x] Implementar a menor mudança necessária.
- [x] Executar verificações aplicáveis.
- [x] Atualizar documentação afetada.
- [ ] Preparar revisão e commit.

## Verificações esperadas

```powershell
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore -m:1
dotnet test SGQ.slnx --no-build
npx --yes markdownlint-cli2@0.18.1
```

## Evidências

- Comandos e resultados: `dotnet restore SGQ.slnx` e `dotnet build SGQ.slnx --no-restore -m:1` concluídos sem erros; 101 testes Web e 3 de Arquitetura aprovados; 15 testes de Integração ignorados porque `SGQ_TEST_PG` não está configurada; `markdownlint-cli2` com 0 erros.
- Capturas ou gravações sem dados reais: pendente.
- PR: pendente.

## Bloqueios e decisões

- Anulação de NC não será implementada nesta demanda: a Qualidade deve definir o status, a possibilidade de reabertura e os efeitos sobre aprovações e prazos.

## Encerramento

- Resultado: pendente.
- Pendências: anulação de NC.
- Revisão técnica: pendente.
- Validação da Qualidade: pendente.
