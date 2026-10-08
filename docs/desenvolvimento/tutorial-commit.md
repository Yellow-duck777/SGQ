# Tutorial de commit

Use este tutorial sempre que pedir a uma IA para criar um commit.

## 1. Confirme a demanda e a branch

```powershell
git branch --show-current
git status --short
```

A branch não pode ser `main`. O diff deve corresponder a uma única demanda documentada.

## 2. Inspecione as alterações

```powershell
git diff --stat
git diff
```

Arquivos novos também devem ser lidos. Não aceite segredos, dados reais, arquivos temporários ou mudanças sem relação com o objetivo.

## 3. Execute as verificações

Para código .NET:

```powershell
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore
dotnet test SGQ.slnx --no-build
```

Para documentação, valide links, títulos, ortografia, comandos e coerência com o estado real, e execute `npx --yes markdownlint-cli2@0.18.1`. Atualize a matriz de rastreabilidade quando o estado de um requisito mudar. Para migrations, teste aplicação em PostgreSQL limpo.

## 4. Escolha a mensagem

Formato:

```text
tipo(escopo): verbo no imperativo e objetivo
```

Boas mensagens:

```text
docs(docs): organiza governança do projeto
fix(anexos): restringe download de anexo anulado
feat(auth): adiciona aprovação de usuários
fix(rc): impede lote de outro produto
test(nc): cobre reprovação de plano de ação
```

Mensagens proibidas: `ajustes`, `mudanças`, `teste`, `final`, `update` ou descrição de várias intenções.

## 5. Peça confirmação

A IA deve apresentar:

- branch;
- arquivos que entrarão;
- resumo do diff;
- verificações e resultados;
- mensagem proposta.

Somente após confirmação explícita:

```powershell
git add -- caminho/do/arquivo outro/caminho
git commit -m "tipo(escopo): descrição"
```

## 6. Revise o commit

```powershell
git show --stat --oneline HEAD
git status --short
```

Push, PR e merge são ações separadas e exigem autorização própria.
