# Contribuindo para o SGQ

Este repositório usa entregas pequenas, rastreáveis e revisadas. Código gerado por IA segue exatamente as mesmas regras.

## Antes de começar

1. Leia [AGENTS.md](AGENTS.md).
2. Localize a demanda em `docs/gestao/dailies/AAAA/MM/AAAA-MM-DD.md`.
3. Confirme que os requisitos relacionados estão `Validado` na [matriz de rastreabilidade](docs/produto/matriz-rastreabilidade.md).
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

Escopos iniciais: `auth`, `usuarios`, `cadastros`, `rc`, `nc`, `recall`, `banco`, `ui`, `testes`, `docs` e `infra`.

Exemplos:

```text
feat(rc): adiciona abertura de reclamação
fix(auth): ativa bloqueio após tentativas inválidas
docs(docs): atualiza instalação local
test(nc): cobre transições inválidas
```

Antes de pedir um commit à IA, use o fluxo de [tutorial-commit.md](docs/desenvolvimento/tutorial-commit.md). A IA deve mostrar o diff, verificações, arquivos e mensagem e aguardar confirmação.

## Qualidade obrigatória

Execute as verificações aplicáveis à mudança. Quando a fundação de testes estiver disponível, todo PR deverá passar por:

```powershell
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore
dotnet test SGQ.slnx --no-build
```

Regras de negócio em `Domain` e `Application` terão cobertura mínima de 80%. Alterações de banco exigem migration, atualização do diagrama e teste contra PostgreSQL.

## Pull Requests

Cada PR deve:

- resolver um único objetivo;
- referenciar a daily e os requisitos;
- explicar comportamento anterior e novo;
- apresentar como validar e as evidências;
- incluir testes ou uma justificativa objetiva;
- informar migrations, riscos de segurança e impactos de implantação;
- não conter segredos nem dados reais;
- receber aprovação de `@ViniciusLgo` antes do merge.

Use o template do repositório. Conversas devem estar resolvidas e os checks obrigatórios concluídos.

## Segurança

Não abra issue pública para uma vulnerabilidade explorável. Siga [SECURITY.md](SECURITY.md). Nunca cole credenciais, tokens, dumps, dados pessoais, POPs ou documentos internos em prompts, commits, issues ou PRs.
