# Estratégia de testes

## Objetivo

Demonstrar que regras, integrações, autorização e fluxos funcionam no ambiente realista sem usar o banco de desenvolvimento nem dados verdadeiros.

## Pirâmide

| Projeto | Responsabilidade |
| --- | --- |
| `SGQ.UnitTests` | Regras de Domain/Application sem infraestrutura |
| `SGQ.IntegrationTests` | EF Core, PostgreSQL, migrations, controllers e políticas |
| `SGQ.EndToEndTests` | Fluxos no navegador com Playwright |
| `SGQ.ArchitectureTests` | Dependências entre projetos e convenções críticas |

Integração usará PostgreSQL descartável via Testcontainers. A execução exige Docker Desktop ativo e nunca aponta para `sgq_dev`.

## Cobertura

`SGQ.Domain` e `SGQ.Application` devem manter cobertura de linhas igual ou superior a 80%. Cobertura não substitui cenários relevantes nem será artificialmente ampliada por testes sem comportamento.

## Cenários obrigatórios

- autenticação, aprovação de conta, MFA e bloqueio;
- autorização por perfil e por URL;
- segregação entre GQ e RT;
- transições válidas e inválidas;
- geração concorrente de códigos;
- prazos em dias úteis;
- auditoria imutável;
- anexos maliciosos, tipo falso, limite e quantidade;
- migrations em banco vazio;
- fluxos completos de RC, NC e Recall;
- acessibilidade e responsividade.

## Pull Requests

Toda correção começa com teste que reproduz o defeito quando viável. Toda funcionalidade inclui cenário positivo, falhas esperadas e autorização. Testes instáveis bloqueiam o PR; não devem ser simplesmente repetidos até passar.

## Evidências manuais

Roteiros manuais complementam automação para usabilidade, conteúdo, acessibilidade exploratória e homologação da Qualidade. Capturas usam dados fictícios.
