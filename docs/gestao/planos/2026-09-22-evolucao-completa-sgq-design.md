# Desenho do programa de evolução do SGQ

> Documento histórico de 2026-09-22. O estado atual das ondas está em [roadmap.md](../roadmap.md) e as demandas em [backlog.md](../backlog.md).

## Propósito

Este documento organiza a evolução do SGQ em entregas pequenas, executáveis e verificáveis. Ele estabelece a sequência entre governança, arquitetura, banco, segurança, módulos funcionais e operação sem transformar o trabalho num único refactor de alto risco.

O desenvolvimento será realizado diretamente pela IA, com revisão do responsável técnico nos pontos de controle. Commits serão locais, separados por intenção. Push, Pull Request, merge, publicação e ações externas continuam dependendo de autorização específica.

## Estado de partida

O repositório possui uma aplicação ASP.NET Core MVC em .NET 10, concentrada em `SGQ.Web`, com Identity, EF Core e PostgreSQL. Clientes, produtos, lotes, abertura de RC e visualização de NC possuem implementação inicial. Controllers acessam o `ApplicationDbContext` diretamente e ainda não existem projetos automatizados de teste.

Há duas linhas de trabalho locais que deverão ser integradas antes da fundação técnica:

- `docs/fundacao-governanca`: reorganização documental ainda não consolidada na `main`;
- `feat/interface-login-dashboard`: commit local `bdaa9f4`, contendo login, sidebar, dashboard e administrador de desenvolvimento.

O banco atual contém somente dados descartáveis de desenvolvimento e pode ser reconstruído. O repositório permanece público e não receberá dados reais, POPs completos, credenciais ou documentos internos.

## Estratégias consideradas

### Evolução incremental por ondas — adotada

Cada onda entrega software funcionando, testes proporcionais, documentação coerente e commits pequenos. Reduz o risco, facilita revisão e cria material didático para quem está aprendendo.

### Reestruturação integral

Separaria todos os projetos, reconstruiria o banco e reescreveria os módulos numa única entrega. Foi rejeitada porque produziria um diff difícil de revisar, aumentaria o tempo sem versão executável e dificultaria localizar regressões.

### Módulos antes da fundação

Priorizaria RC e NC completos sobre a arquitetura atual. Foi rejeitada porque ampliaria débito em autorização, persistência, auditoria e testes, obrigando retrabalho posterior.

## Princípios de execução

1. A aplicação deve continuar compilando e executando ao final de cada entrega.
2. Uma demanda funcional somente começa após seus requisitos estarem em `Validado`.
3. Cada mudança funcional inicia por um teste que descreve o comportamento esperado quando viável.
4. Nenhuma regra de autorização será implementada somente na interface.
5. Cada PR terá uma intenção principal, critérios de aceite e evidências reproduzíveis.
6. Migrations e documentação do banco mudam juntas.
7. Nenhum módulo apresentará indicadores, estados ou resultados fictícios como se fossem reais.
8. Commits locais serão frequentes; publicação no GitHub ocorrerá somente por autorização.
9. Dados usados em testes, seeds e capturas serão sintéticos.
10. Requisitos regulatórios vagos serão devolvidos à Qualidade antes do código correspondente.

## Modelo de ondas

### Onda 0 — Integração segura do trabalho existente

Objetivo: consolidar documentação e interface sem perder histórico ou misturar intenções.

Entregas:

- revisar e registrar a fundação documental;
- integrar a documentação antes das mudanças estruturais;
- atualizar a branch visual sobre a documentação consolidada;
- executar build e roteiro manual do login, dashboard e módulos atuais;
- abrir PRs separados para documentação e interface quando autorizado.

Saída: `main` preparada para a fundação técnica, sem worktrees divergentes nem alterações pendentes.

### Onda 1 — Fronteiras arquiteturais

Objetivo: converter o projeto único num monólito modular sem reescrever as funcionalidades existentes.

Entregas:

- criar `SGQ.Domain`, `SGQ.Application` e `SGQ.Infrastructure`;
- manter `SGQ.Web` como composição, MVC e Identity;
- mover entidades e configurações por etapas;
- criar testes arquiteturais para impedir dependências invertidas;
- tratar warnings como erros no código próprio.

Dependências permitidas:

```text
SGQ.Application    → SGQ.Domain
SGQ.Infrastructure → SGQ.Application
SGQ.Web            → SGQ.Application
SGQ.Web            → SGQ.Infrastructure somente na composição
```

Saída: build verde, comportamento atual preservado e regras arquiteturais automatizadas.

### Onda 2 — Pirâmide de testes e integração contínua

Objetivo: criar uma rede de segurança antes de alterar banco e autenticação.

Entregas:

- criar `SGQ.UnitTests`, `SGQ.IntegrationTests`, `SGQ.EndToEndTests` e `SGQ.ArchitectureTests`;
- configurar xUnit, WebApplicationFactory, Testcontainers PostgreSQL e Playwright;
- cobrir os fluxos atuais como caracterização;
- medir cobertura de `Domain` e `Application` com limite de 80%;
- configurar CI para restore, build, testes, coverage, migrations, CodeQL, dependências vulneráveis e busca de segredos.

Saída: testes locais e CI reproduzíveis, sem uso de `sgq_dev` nos testes.

### Onda 3 — Modelo de dados e migrations

Objetivo: reconstruir a persistência sobre convenções consistentes enquanto os dados ainda são descartáveis.

Entregas:

- documentar diagrama e inventário de tabelas;
- configurar nomes físicos em `snake_case`;
- consolidar as migrations numa base inicial;
- definir UTC, precisão decimal, FKs, índices e restrições;
- adicionar inativação lógica e concorrência otimista;
- substituir `Max + 1` por `AnnualSequence` transacional com retry;
- adicionar `AuditEvent`, `StatusHistory` e o modelo-base de `Attachment`;
- testar criação em banco vazio, concorrência, backup e restauração.

Saída: banco reproduzível e regras estruturais protegidas por integração.

### Onda 4 — Identidade, aprovação e segurança

Objetivo: impedir acesso operacional indevido e estabelecer a trilha de identidade.

Fluxo:

```text
Cadastro público
→ confirmação por SMTP
→ aguardando aprovação
→ administrador atribui perfis
→ MFA para perfil crítico
→ acesso operacional
```

Entregas:

- estender `ApplicationUser` com estado, aprovador e datas;
- criar perfis Administrador, GQ, RT, CQ e Colaborador, permitindo múltiplos perfis;
- aplicar autenticação global e exceções públicas explícitas;
- criar painel de aprovação e gestão de perfis;
- exigir TOTP para Administrador, GQ, RT e CQ;
- implementar autenticação recente para ações críticas;
- aplicar lockout, rate limiting, cookies seguros e headers defensivos;
- invalidar sessões quando aprovação ou perfis mudarem;
- proibir que a mesma conta satisfaça aprovações GQ e RT;
- registrar eventos de identidade sem incluir segredos ou dados pessoais desnecessários.

Saída: todos os caminhos positivos e negativos de acesso cobertos por testes automatizados.

### Onda 5 — Design system e autorização visual

Objetivo: consolidar o protótipo visual já aprovado como sistema acessível e orientado por permissão.

Entregas:

- incorporar login, sidebar e dashboard já construídos;
- extrair componentes e tokens reutilizáveis;
- hospedar IBM Plex Sans e IBM Plex Mono localmente;
- filtrar navegação por permissão sem substituir a autorização do backend;
- revisar teclado, foco, contraste, leitores de tela e redução de movimento;
- testar responsividade e WCAG 2.2 AA.

Saída: interface coerente, acessível e sem links para operações não autorizadas.

### Onda 6 — Cadastros básicos

Objetivo: entregar clientes, produtos e lotes como base confiável para os processos.

Entregas:

- validar campos e unicidades com a Qualidade;
- substituir binding direto de entidades por comandos e ViewModels;
- implementar pesquisa, filtros, paginação e inativação;
- aplicar concorrência otimista e auditoria;
- impedir exclusões ou vínculos inválidos;
- cobrir autorização, validação, overposting e concorrência.

Saída: requisitos de cadastros nos estados `Implementado` e `Verificado`.

### Onda 7 — Reclamação de Cliente

Objetivo: concluir o ciclo da RC com prazos, evidências e transições controladas.

Entregas:

- abertura e complementação de informações;
- validação GQ, classificação, investigação e laboratório;
- conclusão, resposta e encerramento;
- histórico imutável de transições e justificativas;
- geração transacional de código anual;
- abertura automática de NC quando aplicável;
- invalidação de aprovações após alterações relevantes.

Saída: fluxo completo de RC demonstrado por testes unitários, integração e navegador.

### Onda 8 — Não Conformidade

Objetivo: controlar contenção, investigação, causa, ação e eficácia com segregação de funções.

Entregas:

- criação manual ou proveniente de RC;
- contenção e investigação;
- análise de causa e plano de ação;
- implementação e avaliação de eficácia;
- aprovações separadas de GQ e RT;
- encerramento e reabertura auditados.

Saída: transições inválidas bloqueadas no backend e fluxo completo verificado.

### Onda 9 — Recall

Objetivo: executar recolhimentos rastreáveis quando a avaliação técnica indicar necessidade.

Entregas:

- avaliação e aprovação;
- identificação de lotes e destinatários;
- recolhimento, retorno, segregação e destinação;
- comunicações e evidências;
- encerramento e reabertura auditados.

Saída: fluxo de Recall verificado com dados sintéticos e segregação de aprovação.

### Onda 10 — Serviços compartilhados e anexos

Objetivo: completar capacidades usadas pelos três processos.

Entregas:

- calendário e cálculo de dias úteis;
- prorrogações justificadas;
- notificações SMTP;
- anexos em `bytea`, separados das tabelas operacionais;
- limite de 25 MB por arquivo e 20 anexos por registro;
- validação de extensão, MIME e assinatura real;
- SHA-256, ClamAV e download autorizado;
- anulação lógica com justificativa e auditoria;
- testes de crescimento de até 500 MB por processo, memória, backup e restauração.

Saída: serviços integrados sem expor conteúdo malicioso ou dados sem autorização.

### Onda 11 — Informação gerencial

Objetivo: transformar dados verificados em acompanhamento e decisão.

Entregas:

- dashboard baseado somente em consultas reais;
- indicadores validados pela Qualidade;
- pesquisa global;
- relatórios e exportações PDF/XLSX;
- testes de autorização, precisão e volume.

Saída: informações conciliáveis com os registros operacionais.

### Onda 12 — Infraestrutura e produção

Objetivo: publicar com operação segura, observável e recuperável.

Entregas:

- Dockerfiles multi-stage;
- Compose separado para desenvolvimento, testes e produção;
- Linux, PostgreSQL, proxy HTTPS e ClamAV;
- Mailpit somente em desenvolvimento e SMTP configurável em produção;
- ambientes de desenvolvimento, homologação e produção;
- Data Protection persistente em volume protegido;
- health checks sem detalhes sensíveis;
- logs estruturados com correlação e auditoria separada;
- backup diário criptografado com retenção operacional de 30 dias;
- ensaio periódico de restauração;
- checklist de homologação e publicação.

Saída: publicação aprovada tecnicamente e homologada pela responsável da Qualidade.

## Fluxo de dados alvo

Uma requisição autenticada chega a `SGQ.Web`, onde ocorre validação de entrada e autorização de rota. A camada `Application` executa o caso de uso e consulta regras do `Domain`. Contratos de persistência, identidade, notificações e anexos são implementados por `Infrastructure`. A transação persiste o estado operacional, o histórico e a auditoria; somente depois são agendados efeitos externos idempotentes, como e-mail.

```text
Navegador
→ Web: autenticação, autorização e ViewModel
→ Application: caso de uso e política
→ Domain: regra e transição
→ Infrastructure: PostgreSQL e integrações
→ Web: resultado seguro para o usuário
```

## Tratamento de erros

- erros de validação retornam mensagens de campo sem stack trace;
- conflitos de concorrência retornam orientação para recarregar e comparar alterações;
- transições inválidas são recusadas antes da persistência;
- falhas transitórias de banco usam retry somente em operações idempotentes ou protegidas por transação;
- falhas de SMTP não desfazem silenciosamente a operação principal e ficam disponíveis para reprocessamento;
- anexos reprovados não são disponibilizados;
- eventos inesperados recebem identificador de correlação e log sem conteúdo sensível;
- produção usa página de erro genérica e health checks sem detalhes internos.

## Estratégia de testes

Cada regra de negócio nasce em teste unitário. Persistência, Identity, autorização, migrations, sequências e anexos são verificados contra PostgreSQL descartável. Fluxos críticos são exercitados por Playwright. Testes arquiteturais protegem as dependências entre projetos.

O pipeline mínimo executará:

```text
restore
→ build com warnings como erro
→ testes unitários
→ testes arquiteturais
→ testes de integração PostgreSQL
→ coverage de Domain/Application >= 80%
→ migrations em banco vazio
→ testes end-to-end selecionados
→ CodeQL, dependências e segredos
```

Testes manuais ficam reservados a usabilidade, acessibilidade exploratória e homologação da Qualidade. Build bem-sucedido não será tratado como prova de funcionamento no navegador.

## Governança de uma demanda

Cada demanda terá identificador `DEM-AAAA-NNN`, responsável, requisitos relacionados, critérios de aceite e uma daily. O estado percorre `Proposto`, `Validado`, `Implementado`, `Verificado` ou `Adiado`.

Uma entrega somente avança quando:

1. dependências anteriores estão verificadas;
2. requisitos funcionais foram validados pelo revisor técnico e pela Qualidade;
3. testes da mudança passam localmente;
4. documentação e matriz de rastreabilidade estão atualizadas;
5. o diff contém uma única intenção revisável;
6. riscos residuais estão registrados.

## Estratégia de branches e integração

Cada demanda usa uma branch curta criada da `main` atualizada. Mudanças estruturais não compartilham branch com funcionalidades. O título do PR e os commits usam `tipo(escopo): descrição no imperativo` em português.

Ordem inicial de integração:

1. `docs/fundacao-governanca`;
2. `feat/interface-login-dashboard`, atualizada sobre a documentação;
3. `refactor(arquitetura): separar fronteiras do monólito`;
4. `test(testes): criar pirâmide e infraestrutura de testes`;
5. `refactor(banco): consolidar modelo e migrations`;
6. `feat(auth): adicionar aprovação e perfis`.

## Pontos de controle do responsável técnico

O revisor acompanha cada onda em quatro momentos:

- início: confirma escopo e critérios;
- execução: avalia decisões irreversíveis ou de segurança;
- demonstração: percorre o roteiro funcional e as evidências;
- encerramento: autoriza commit final, push, PR ou integração conforme a ação solicitada.

## Critério de conclusão do programa

O programa estará concluído quando RC, NC e Recall operarem de ponta a ponta com autorização, segregação, auditoria, anexos, prazos e relatórios; o ambiente de produção for reproduzível em Linux; backup e restauração tiverem evidência; todos os requisitos do escopo estiverem `Verificado` ou `Adiado` com justificativa; e a responsável da Qualidade tiver homologado a versão candidata.
