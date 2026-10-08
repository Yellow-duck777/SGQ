# Estratégia de testes

## Objetivo

Demonstrar que regras, integrações, autorização e fluxos funcionam no ambiente realista sem usar o banco de desenvolvimento nem dados verdadeiros.

## Situação atual

| Projeto | Existe | Responsabilidade |
| --- | --- | --- |
| `SGQ.ArchitectureTests` | Sim | Dependências entre projetos (xUnit) |
| `SGQ.Web.Tests` | Sim | Controllers e serviços com xUnit e EF Core InMemory |
| `SGQ.IntegrationTests` | Sim | Pipeline real com `WebApplicationFactory` e PostgreSQL real: política de acesso por perfil, migrations, restrições e auditoria |
| `SGQ.UnitTests` | Não (planejado) | Regras de Domain e Application sem infraestrutura |
| `SGQ.EndToEndTests` | Não (planejado) | Fluxos no navegador com Playwright |

A lista de testes e a cobertura por requisito estão na [matriz de testes](matriz-de-testes.md). O número de testes não é fixado neste documento.

### Testes de integração com PostgreSQL

`SGQ.IntegrationTests` usa a variável de ambiente `SGQ_TEST_PG`, com a conexão do servidor sem nome de banco (por exemplo `Host=127.0.0.1;Port=5432;Username=postgres;Password=<senha-local>`). Cada execução cria um banco próprio e o descarta ao final; os testes nunca usam `sgq_dev`. Sem a variável, os testes aparecem como ignorados e `dotnet test` continua verde. No CI a variável é sempre definida.

### Integração contínua

O workflow `.github/workflows/ci.yml` tem dois jobs: `build-test` (Ubuntu, serviço `postgres:17`, `dotnet restore`, `build` e `test` com `SGQ_TEST_PG`) e `docs` (`markdownlint-cli2`). Ainda não há varredura de segredos, análise de dependências vulneráveis nem cobertura (DEM-2026-003).

### Limitações e riscos

- A autenticação nos testes de integração é simulada por um esquema de teste que lê cabeçalhos; o fluxo real de login por cookie, o bloqueio por falhas e a confirmação de conta não são exercitados.
- `SGQ.Web.Tests` usa InMemory, que não aplica restrições relacionais; qualquer verificação de restrição deve ir em `SGQ.IntegrationTests`.
- Não há testes de concorrência (numeração anual simultânea), carga nem E2E com navegador (DEM-2026-117).
- Não há testes de calendário via HTTP, notificações, relatórios (exceto CSV) nem administração de usuários além de `/Usuarios` por perfil.
- Um erro de autorização foi encontrado apenas pelo teste de integração (ver [Errata](revisao-seguranca.md)); correções de acesso devem sempre ter teste contra o pipeline real.

## Cobertura

`SGQ.Domain` e `SGQ.Application` devem manter cobertura de linhas igual ou superior a 80% quando houver regras nessas camadas. A cobertura ainda não é medida. Ela não substitui cenários relevantes nem será artificialmente ampliada por testes sem comportamento.

## Cenários obrigatórios

- autenticação, aprovação de conta, MFA e bloqueio;
- autorização por perfil e por URL;
- segregação entre GQ e RT;
- transições válidas e inválidas;
- geração concorrente de códigos;
- prazos em dias úteis;
- auditoria sem dados sensíveis;
- anexos maliciosos, tipo falso, limite e quantidade;
- migrations em banco vazio e em banco existente;
- fluxos completos de RC, NC e Recall;
- acessibilidade e responsividade.

## Pull Requests

Toda correção começa com teste que reproduz o defeito quando viável. Toda funcionalidade inclui cenário positivo, falhas esperadas e autorização. Testes instáveis bloqueiam o PR; não devem ser simplesmente repetidos até passar.

## Evidências manuais

Roteiros manuais complementam automação para usabilidade, conteúdo, acessibilidade exploratória e homologação da Qualidade. Capturas usam dados fictícios.
