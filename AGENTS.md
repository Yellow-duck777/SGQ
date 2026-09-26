# Instruções para agentes de IA

Estas regras valem para qualquer IA que leia ou altere este repositório.

## Leitura obrigatória

Antes de agir:

1. leia `README.md` e `CONTRIBUTING.md`;
2. localize a daily ativa em `docs/gestao/dailies/`;
3. leia os requisitos citados pela daily;
4. para commits, leia `docs/desenvolvimento/tutorial-commit.md`;
5. para segurança, leia `SECURITY.md` e `docs/arquitetura/seguranca.md`.

Se não existir daily ou critério de aceite para uma mudança funcional, pare e solicite a definição da demanda. Requisitos em estado diferente de `Validado` não autorizam implementação funcional.

## Execução

- Nunca desenvolva diretamente na `main`.
- Preserve alterações do usuário e não misture objetivos.
- Prefira mudanças pequenas, testáveis e explicáveis.
- Não invente regras de negócio; registre a dúvida no backlog.
- Valide autorização no backend, não apenas na interface.
- Não use dados reais em testes, screenshots ou seeds.
- Não versione segredos, documentos internos ou informações confidenciais.
- Migrations, modelos, documentação de banco e testes devem permanecer coerentes.

## Commits

Quando alguém pedir “faça o commit”:

1. confirme a branch atual;
2. mostre `git status` e resuma o diff;
3. execute as verificações aplicáveis;
4. proponha os arquivos e a mensagem Conventional Commit;
5. aguarde confirmação explícita;
6. crie um único commit por intenção.

Não faça push, merge, rebase, alteração de proteção de branch ou publicação sem autorização específica.

## Revisão mínima

Antes de entregar, informe:

- o que mudou;
- quais critérios foram atendidos;
- quais comandos foram executados;
- resultados e limitações;
- riscos ou decisões que ainda dependem do revisor técnico e da Qualidade.
