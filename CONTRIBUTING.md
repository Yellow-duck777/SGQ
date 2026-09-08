# Contribuindo para o SGQ

## Fluxo de trabalho

1. Atualize sua `main` local e crie uma branch a partir dela.
2. Faça alterações pequenas, focadas e testáveis.
3. Abra um Pull Request (PR) para a `main`.
4. Solicite a revisão de `@ViniciusLgo`.
5. Faça o merge somente após a aprovação e os checks obrigatórios concluídos.

Branches sugeridas:

- `feat/nome-da-funcionalidade`
- `fix/descricao-do-ajuste`
- `docs/assunto`
- `chore/descricao`

## Padrão de commits

Use o formato Conventional Commits:

```text
tipo: resumo curto no imperativo
```

Tipos aceitos:

| Tipo | Quando usar |
| --- | --- |
| `feat` | Nova funcionalidade |
| `fix` | Correção de defeito |
| `docs` | Documentação |
| `style` | Formatação sem mudança de comportamento |
| `refactor` | Reorganização de código sem correção ou funcionalidade nova |
| `test` | Inclusão ou ajuste de testes |
| `chore` | Configuração, dependências ou manutenção |

Exemplos:

```text
feat: adiciona cadastro de clientes
fix: corrige validação de lote
docs: descreve fluxo de revisão
chore: atualiza configuração do banco
```

Evite mensagens genéricas como `ajustes`, `teste` ou `mudanças`.

## Pull Requests

Cada PR deve:

- ter um objetivo claro e escopo reduzido;
- seguir o modelo de PR do repositório;
- conter testes ou explicar por que eles não se aplicam;
- receber aprovação do responsável técnico antes do merge.
