# Proposta de AGENTS.md unificado

Este documento propõe a estrutura de um único `AGENTS.md`, combinando a versão de governança (daily obrigatória, requisitos `Validado`, commits com confirmação) com a versão didática do `origin/main` (explicar tudo, uma etapa por vez). Esta proposta já foi aplicada no [AGENTS.md](../../AGENTS.md) (PR de integração com PostgreSQL e CI); o documento fica como registro das decisões de unificação. O responsável técnico e o Caio devem revisar o `AGENTS.md` resultante.

## O que cada versão tem de útil

| Versão | Pontos a manter |
| --- | --- |
| Governança | Leitura obrigatória; daily e critério de aceite como pré-condição; requisitos validados; proibição de trabalhar na `main`; sem dados reais nem segredos; autorização no backend; fluxo de commit com confirmação; sem push, merge ou rebase sem autorização; revisão mínima |
| `origin/main` | Comunicação didática; uma etapa por vez com "antes e depois"; confirmação antes de mudanças sensíveis; progresso nunca inventado; fonte de verdade e aviso de conflito entre código e documento; não alterar migrations antigas; tratamento disciplinado de erros; formato de resposta; uma única sessão de servidor local; sem subagentes sem pedido |

## Conflitos a resolver

1. **Fonte dos requisitos**: o `origin/main` aponta `docs/02-requisitos.md`, que não existe mais; usar `docs/produto/requisitos-consolidados.md` e a matriz de rastreabilidade.
2. **Estados de requisito**: usar a regra de `requisitos-gerais.md`: `Validado`, `Implementado` e `Verificado` orientam alteração; `Proposto` e `Adiado` não.
3. **Granularidade**: uma etapa por vez com confirmação apenas em mudanças sensíveis, para não duplicar pedidos de confirmação.
4. **Comandos de validação**: `restore`, `build --no-restore`, `test --no-build` e `markdownlint-cli2`.
5. **Progresso**: percentual só quando calculável a partir da matriz; senão, "Progresso geral ainda não calculado".

## Estrutura proposta (13 seções)

1. **Escopo**: vale para qualquer IA que leia ou altere o repositório; o repositório é público.
2. **Leitura obrigatória**: `README.md`, `CONTRIBUTING.md`, a daily ativa em `docs/gestao/dailies/`, os requisitos citados e a matriz de rastreabilidade; para commits, `tutorial-commit.md`; para segurança, `SECURITY.md` e `docs/arquitetura/seguranca.md`. Sem daily ou critério de aceite para mudança funcional, parar e pedir a definição.
3. **Comunicação e ritmo** (do `origin/main`, seções 1 e 2): linguagem simples, termos técnicos explicados, uma etapa por vez, informar antes (o quê, por quê, arquivos afetados) e depois (feito, funcionou, resultado).
4. **Mudanças que exigem explicação e confirmação prévia** (do `origin/main`, seção 5): banco, migration, autenticação, permissões, regra de negócio, dependências, comandos destrutivos e configuração importante.
5. **Governança** (da versão local): requisitos nos estados permitidos, nunca na `main`, mudanças pequenas e testáveis, não inventar regra de negócio (registrar a dúvida no backlog), preservar alterações do usuário.
6. **Banco de dados** (do `origin/main`, seção 11): não editar migrations antigas, criar nova migration, manter modelo, snapshot, `banco-de-dados.md` e testes coerentes, nunca apagar dados sem autorização.
7. **Segurança e dados**: sem segredos, senhas, tokens ou dados reais em código, testes, seeds e capturas; autorização validada no backend.
8. **Validação**: `dotnet restore SGQ.slnx`, `dotnet build SGQ.slnx --no-restore`, `dotnet test SGQ.slnx --no-build`, `dotnet ef migrations has-pending-model-changes` quando houver modelo e `npx --yes markdownlint-cli2@0.18.1` para documentação; explicar o resultado de forma simples.
9. **Erros** (do `origin/main`, seção 14): ler a mensagem, explicá-la, identificar a causa provável, propor e aplicar uma correção controlada e testar de novo; sem alterações aleatórias.
10. **Commits** (da versão local): confirmar a branch, mostrar `git status` e resumo do diff, executar verificações, propor arquivos e mensagem Conventional Commit, aguardar confirmação explícita e criar um único commit por intenção; sem push, merge, rebase, alteração de proteção de branch ou publicação sem autorização específica.
11. **Formato da resposta e progresso** (do `origin/main`, seções 6, 7, 8 e 15): objetivo, situação atual, próximo passo, resultado e progresso; percentual calculado a partir da matriz de rastreabilidade, nunca inventado; diferenciar progresso da tarefa e do projeto.
12. **Ambiente local** (do `origin/main`, seções 3 e 4): uma única sessão e instância do servidor; não criar nem coordenar subagentes ou agentes paralelos sem pedido explícito.
13. **Revisão mínima antes de entregar** (da versão local): o que mudou, critérios atendidos, comandos executados, resultados e limitações, riscos e decisões pendentes do revisor técnico e da Qualidade.
