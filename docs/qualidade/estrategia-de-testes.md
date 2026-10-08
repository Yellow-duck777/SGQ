# Estratégia de testes

## Objetivo

Demonstrar que regras, integrações, autorização e fluxos funcionam no ambiente realista sem usar o banco de desenvolvimento nem dados verdadeiros.

## Situação atual

| Projeto | Existe | Responsabilidade |
| --- | --- | --- |
| `SGQ.ArchitectureTests` | Sim | Dependências entre projetos (xUnit) |
| `SGQ.Web.Tests` | Sim | Testes de controllers e serviços com xUnit e EF Core InMemory |
| `SGQ.UnitTests` | Não (planejado) | Regras de Domain e Application sem infraestrutura |
| `SGQ.IntegrationTests` | Não (planejado) | EF Core, PostgreSQL, migrations, controllers e políticas |
| `SGQ.EndToEndTests` | Não (planejado) | Fluxos no navegador com Playwright |

A lista de testes e a cobertura por requisito estão na [matriz de testes](matriz-de-testes.md). O número de testes não é fixado neste documento.

### Riscos do estado atual

- O provedor InMemory não aplica restrições relacionais, índices únicos, `CHECK`, comportamento de exclusão nem concorrência; um teste verde não prova que o PostgreSQL aceita a operação.
- Não há testes de autorização por perfil via HTTP (política de fallback e `[Authorize(Roles)]` são verificados manualmente pelos roteiros).
- Não há testes das migrations, de calendário, notificações, relatórios e administração de usuários.
- Não existe CI: nada executa automaticamente em PR (DEM-2026-003).

## Plano

Integração usará PostgreSQL descartável via Testcontainers (exige Docker ativo e nunca aponta para `sgq_dev`), `WebApplicationFactory` para autorização e Playwright para fluxos críticos. O CI executará restore, build, testes, `markdownlint-cli2`, busca de segredos e análise de dependências (DEM-2026-003 e DEM-2026-117).

## Cobertura

`SGQ.Domain` e `SGQ.Application` devem manter cobertura de linhas igual ou superior a 80% quando houver regras nessas camadas. Cobertura não substitui cenários relevantes nem será artificialmente ampliada por testes sem comportamento.

## Cenários obrigatórios

- autenticação, aprovação de conta, MFA e bloqueio;
- autorização por perfil e por URL;
- segregação entre GQ e RT;
- transições válidas e inválidas;
- geração concorrente de códigos;
- prazos em dias úteis;
- auditoria sem dados sensíveis;
- anexos maliciosos, tipo falso, limite e quantidade;
- migrations em banco vazio;
- fluxos completos de RC, NC e Recall;
- acessibilidade e responsividade.

## Pull Requests

Toda correção começa com teste que reproduz o defeito quando viável. Toda funcionalidade inclui cenário positivo, falhas esperadas e autorização. Testes instáveis bloqueiam o PR; não devem ser simplesmente repetidos até passar.

## Evidências manuais

Roteiros manuais complementam automação para usabilidade, conteúdo, acessibilidade exploratória e homologação da Qualidade. Capturas usam dados fictícios.
