# Instruções para agentes de IA

Estas regras valem para qualquer IA que leia ou altere este repositório. O repositório é público: nada de dados reais, documentos internos, credenciais ou informações confidenciais.

Este arquivo unifica a governança do projeto (daily, requisitos, commits) com a comunicação didática combinada com quem está aprendendo desenvolvimento. A origem e as decisões da unificação estão em [a proposta](docs/gestao/proposta-agents-unificado.md).

## 1. Leitura obrigatória

Antes de agir:

1. leia `README.md` e `CONTRIBUTING.md`;
2. localize a daily ativa em `docs/gestao/dailies/`;
3. leia os requisitos citados pela daily e a [matriz de rastreabilidade](docs/produto/matriz-rastreabilidade.md);
4. para commits, leia `docs/desenvolvimento/tutorial-commit.md`;
5. para segurança, leia `SECURITY.md` e `docs/arquitetura/seguranca.md`.

Fonte de verdade das regras de negócio: `docs/produto/requisitos-consolidados.md`. Não invente requisitos; registre a dúvida no backlog (veja também `docs/gestao/decisoes-pendentes.md`). Se o código e a documentação divergirem, avise antes de decidir qual está certo.

Se não existir daily ou critério de aceite para uma mudança funcional, pare e solicite a definição da demanda.

Estados de requisito (conforme `docs/produto/requisitos-gerais.md`): `Validado`, `Implementado` e `Verificado` orientam alterações; `Proposto` e `Adiado` não autorizam implementação funcional.

## 2. Comunicação e ritmo

O responsável pelo projeto está aprendendo desenvolvimento enquanto constrói o sistema. Por isso:

- explique de forma simples e direta; defina brevemente termos técnicos importantes;
- não presuma que a pessoa sabe executar comandos, localizar arquivos ou interpretar erros;
- trabalhe uma etapa por vez quando a tarefa puder ser dividida: antes, diga o que fará, por quê e quais arquivos podem ser afetados; depois, diga o que foi feito, se funcionou e o que o resultado significa;
- não torne respostas simples desnecessariamente longas.

Priorize: entender, explicar, executar, validar, registrar e continuar. Não priorize velocidade em detrimento da compreensão.

## 3. Mudanças que exigem explicação e confirmação prévia

Antes de qualquer uma das ações abaixo, explique o que pretende fazer, por quê, o impacto e o risco, e aguarde a decisão quando houver risco relevante ou mais de uma alternativa válida:

- alterar banco de dados ou criar migration;
- excluir código;
- modificar autenticação ou permissões;
- alterar regras de negócio;
- instalar dependências;
- executar comandos potencialmente destrutivos;
- modificar configurações importantes.

## 4. Execução e governança

- Nunca desenvolva diretamente na `main`.
- Preserve alterações do usuário e não misture objetivos.
- Prefira mudanças pequenas, testáveis e explicáveis.
- Valide autorização no backend, não apenas na interface.
- Cada correção ou regra nova precisa de teste; regras de acesso e banco são verificadas pelos testes de integração.

## 5. Banco de dados

- Não altere migrations já criadas; crie uma nova.
- Ao mudar o modelo: explique a alteração, crie a migration, confira com `dotnet ef migrations has-pending-model-changes` e atualize `docs/arquitetura/banco-de-dados.md` e os testes.
- Nunca apague dados sem autorização explícita. Migrations que removem ou mascaram dados exigem backup e aviso de irreversibilidade no checklist de publicação.

## 6. Segurança e dados

- Nunca exponha ou versione senhas, tokens, segredos ou connection strings com credenciais.
- Não use dados reais em testes, screenshots ou seeds.
- Não coloque credenciais no código; use User Secrets ou variáveis de ambiente.

## 7. Validação

Depois de alterações relevantes, execute o que se aplicar e explique o resultado de forma simples:

```powershell
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore
dotnet test SGQ.slnx --no-build
npx --yes markdownlint-cli2@0.18.1
```

Os testes de integração usam PostgreSQL real e leem a conexão de `SGQ_TEST_PG`; sem a variável eles aparecem como ignorados (veja `docs/desenvolvimento/ambiente-local.md`). Informe quando algo não puder ser executado.

## 8. Erros

Quando ocorrer um erro: não faça várias alterações aleatórias; leia a mensagem; explique o que ela significa; identifique a causa provável; proponha e aplique uma correção controlada; teste novamente.

## 9. Commits

Quando alguém pedir "faça o commit":

1. confirme a branch atual;
2. mostre `git status` e resuma o diff;
3. execute as verificações aplicáveis;
4. proponha os arquivos e a mensagem Conventional Commit;
5. aguarde confirmação explícita;
6. crie um único commit por intenção.

Não faça push, merge, rebase, alteração de proteção de branch ou publicação sem autorização específica.

## 10. Formato das respostas e progresso

Em tarefas de desenvolvimento, prefira: objetivo, situação atual, próximo passo, resultado e progresso.

A porcentagem de progresso nunca é inventada: calcule-a a partir da matriz de rastreabilidade ou das etapas identificadas. Diferencie o progresso da tarefa atual do progresso geral do projeto. Sem base para calcular, informe "Progresso geral ainda não calculado."

## 11. Ambiente local e agentes

- Use uma única sessão de terminal para o servidor local do SGQ e não inicie mais de uma instância.
- Não crie, delegue para ou coordene subagentes ou agentes paralelos sem pedido explícito do usuário.

## 12. Revisão mínima

Antes de entregar, informe:

- o que mudou;
- quais critérios foram atendidos;
- quais comandos foram executados;
- resultados e limitações;
- riscos ou decisões que ainda dependem do revisor técnico e da Qualidade.
