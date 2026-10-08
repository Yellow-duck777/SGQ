# SGQ — Sistema de Gestão da Qualidade

Sistema web para registrar, tratar e rastrear processos de qualidade.

> O repositório é público. Não inclua dados reais, documentos internos, POPs, credenciais ou informações confidenciais.

A documentação está indexada em [docs/README.md](docs/README.md). A situação verificável de cada requisito fica na [matriz de rastreabilidade](docs/produto/matriz-rastreabilidade.md).

## Processos cobertos

- Reclamação de Cliente (RC)
- Não Conformidade (NC)
- Recall / Recolhimento de Produto

## Recursos implementados

O estado por requisito, incluindo lacunas, está na matriz de rastreabilidade.

- Autenticação com ASP.NET Identity, bloqueio após cinco falhas e perfis Administrador, GQ, RT, CQ e Auditor. Contas sem perfil não acessam o sistema até que um Administrador atribua um perfil.
- Administração de usuários e perfis (somente Administrador).
- Cadastros de clientes, produtos e lotes.
- Numeração anual automática: `RC-AAAA-000001`, `NC-AAAA-000001` e `REC-AAAA-000001`.
- Fluxo de RC: abertura, validação pela GQ, classificação, geração automática de NC, investigação, laboratório externo, conclusão, resposta ao cliente, encerramento e reabertura.
- Fluxo de NC: abertura, contenção, investigação, laboratório externo, plano de ação, eficácia, aprovações RT e GQ por contas distintas, decisão do CQ em divergência, encerramento e reabertura.
- Fluxo de Recall: avaliação, aprovações RT e GQ, decisão do CQ em divergência, recolhimento, comunicação, retornos, destinação, encerramento e reabertura.
- Calendário de dias não úteis, prazos em dias úteis e prorrogação com motivo.
- Alertas de prazo e notificações de eventos do fluxo por e-mail (requer SMTP configurado).
- Pesquisa e filtros nas listagens de RC, NC e Recall.
- Relatório de processos com exportação CSV, XLSX e PDF.
- Auditoria automática de criações, alterações e exclusões dos dados do SGQ.
- Anexos e evidências para RC, NC e Recall, com limite de 25 MB, extensões permitidas, anulação lógica e vínculo da mesma evidência a vários processos. Os arquivos ficam em disco, em `App_Data/uploads` (ver [ADR 0002](docs/arquitetura/decisoes/0002-anexos-postgresql.md)).
- Dashboard com indicadores de processos abertos, aprovações pendentes, vencidos e próximos do vencimento.

## Tecnologias

- C# / .NET 10 e ASP.NET Core MVC
- Entity Framework Core + PostgreSQL 17
- ASP.NET Identity
- Bootstrap, HTML, CSS e JavaScript

## Como executar

Configure a conexão PostgreSQL por User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=sgq_dev;Username=postgres;Password=<senha-local>" --project src/SGQ.Web
```

Primeiro administrador (qualquer ambiente): cadastre a conta pela tela de login e informe o e-mail dela. Na inicialização, a conta recebe o perfil Administrador somente se ainda não existir nenhum Administrador.

```powershell
dotnet user-secrets set "InitialAdminEmail" "<e-mail-da-conta>" --project src/SGQ.Web
```

Somente em Development, o seeder pode criar um administrador local:

```powershell
dotnet user-secrets set "SeedAdmin:Email" "<e-mail-de-teste>@sgq.test" --project src/SGQ.Web
dotnet user-secrets set "SeedAdmin:Password" "<senha-local-gerada>" --project src/SGQ.Web
```

E-mails por SMTP são opcionais. Sem as chaves abaixo, nenhuma notificação é enviada:

```powershell
dotnet user-secrets set "Smtp:Host" "<servidor>" --project src/SGQ.Web
dotnet user-secrets set "Smtp:Port" "587" --project src/SGQ.Web
dotnet user-secrets set "Smtp:EnableSsl" "true" --project src/SGQ.Web
dotnet user-secrets set "Smtp:UserName" "<usuario>" --project src/SGQ.Web
dotnet user-secrets set "Smtp:Password" "<senha>" --project src/SGQ.Web
dotnet user-secrets set "Smtp:From" "<remetente>" --project src/SGQ.Web
```

Execute a aplicação:

```powershell
dotnet run --project src/SGQ.Web
```

> Atenção: as migrations são aplicadas automaticamente na inicialização, em qualquer ambiente. Isso é aceitável apenas enquanto não houver produção; consulte o item de hardening no [backlog](docs/gestao/backlog.md). Para apresentar o sistema localmente, veja [ambiente local](docs/desenvolvimento/ambiente-local.md).

## Desenvolvimento

Verificações antes de abrir um PR:

```powershell
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore
dotnet test SGQ.slnx --no-build
npx --yes markdownlint-cli2@0.18.1
```

Para criar ou desativar contas locais de teste com segurança, consulte [a documentação de usuários de teste](docs/usuarios-teste-development.md). Regras de contribuição: [CONTRIBUTING.md](CONTRIBUTING.md).

## Próximas evoluções

As demandas pendentes têm identificador no [backlog](docs/gestao/backlog.md). As principais:

- integração contínua (CI) e testes de integração com PostgreSQL real;
- hardening de produção (política de senha, cabeçalhos, MFA, Data Protection, migrations fora da inicialização);
- validação de conteúdo de anexos, SHA-256, antimalware e limite de 20 anexos por registro;
- inativação de cadastros, paginação e concorrência otimista;
- amostras internas, tratamento comercial, atraso externo e relatórios completos do MVP;
- conclusão da separação em camadas (`Application` e `Infrastructure`).
