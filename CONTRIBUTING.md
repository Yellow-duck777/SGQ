# Contribuindo para o SGQ

Este repositório usa entregas pequenas, rastreáveis e revisadas. Código gerado por IA segue exatamente as mesmas regras.

## Antes de começar

1. Leia [AGENTS.md](AGENTS.md).
2. Localize a demanda em `docs/gestao/dailies/AAAA/MM/AAAA-MM-DD[-slug].md`. O sufixo `-slug` é opcional e identifica a demanda quando há mais de uma daily no dia.
3. Confirme na [matriz de rastreabilidade](docs/produto/matriz-rastreabilidade.md) que os requisitos relacionados estão `Validado`, `Implementado` ou `Verificado`. Requisitos `Proposto` ou `Adiado` não autorizam código funcional.
4. Atualize a `main` e crie uma branch.

```powershell
git switch main
git pull --ff-only
git switch -c feat/descricao-curta
```

## Branches

Use nomes em minúsculas, sem acentos e separados por hífen:

| Prefixo | Uso |
| --- | --- |
| `feat/` | Funcionalidade |
| `fix/` | Correção |
| `docs/` | Documentação |
| `test/` | Testes |
| `refactor/` | Refatoração sem mudança funcional |
| `chore/` | Manutenção e configuração |

Nunca faça desenvolvimento direto na `main`.

## Commits

Formato obrigatório:

```text
tipo(escopo): descrição curta no imperativo
```

Tipos: `feat`, `fix`, `docs`, `test`, `refactor`, `style`, `perf`, `build`, `ci`, `chore` e `revert`.

Escopos: `auth`, `usuarios`, `cadastros`, `rc`, `nc`, `recall`, `anexos`, `prazos`, `relatorios`, `banco`, `ui`, `testes`, `docs` e `infra`.

Exemplos:

```text
feat(rc): adiciona abertura de reclamação
fix(auth): ativa bloqueio após tentativas inválidas
docs(docs): atualiza instalação local
test(nc): cobre transições inválidas
```

Antes de pedir um commit à IA, use o fluxo de [tutorial-commit.md](docs/desenvolvimento/tutorial-commit.md). A IA deve mostrar o diff, verificações, arquivos e mensagem e aguardar confirmação.

## Qualidade obrigatória

Execute as verificações aplicáveis à mudança:

```powershell
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore
dotnet test SGQ.slnx --no-build
npx --yes markdownlint-cli2@0.18.1
```

Ainda não há integração contínua (DEM-2026-003): quem abre o PR é responsável por executar e registrar esses comandos. Regras de negócio em `Domain` e `Application` terão cobertura mínima de 80% quando o CI existir. Alterações de banco exigem migration, snapshot coerente (`dotnet ef migrations has-pending-model-changes`), atualização de [banco-de-dados.md](docs/arquitetura/banco-de-dados.md) e teste contra PostgreSQL quando houver infraestrutura. Decisões arquiteturais novas exigem um [ADR](docs/arquitetura/decisoes/README.md).

## Pull Requests

Cada PR deve:

- resolver um único objetivo;
- referenciar a daily e os requisitos;
- explicar comportamento anterior e novo;
- apresentar como validar e as evidências;
- incluir testes ou uma justificativa objetiva;
- atualizar a matriz de rastreabilidade quando mudar o estado de um requisito;
- informar migrations, riscos de segurança e impactos de implantação;
- não conter segredos nem dados reais;
- receber aprovação do revisor técnico (`@ViniciusLgo`) antes do merge.

Use o template do repositório. Conversas devem estar resolvidas e os checks obrigatórios concluídos.

## Segurança

Não abra issue pública para uma vulnerabilidade explorável. Siga [SECURITY.md](SECURITY.md). Nunca cole credenciais, tokens, dumps, dados pessoais, POPs ou documentos internos em prompts, commits, issues ou PRs.
