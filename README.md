# SGQ — Sistema de Gestão da Qualidade

Sistema web para registrar, tratar e rastrear processos de qualidade.

> O repositório é público. Não inclua dados reais, documentos internos, POPs, credenciais ou informações confidenciais.

## Processos cobertos

- Reclamação de Cliente (RC)
- Não Conformidade (NC)
- Recall / Recolhimento de Produto

## Recursos implementados

- Autenticação com ASP.NET Identity; perfis Administrador, GQ, RT, CQ e Auditor.
- Cadastros de clientes, produtos e lotes.
- Numeração anual automática: `RC-AAAA-000001`, `NC-AAAA-000001` e `REC-AAAA-000001`.
- Fluxo de RC: abertura, validação, classificação, geração automática de NC, investigação, conclusão e encerramento.
- Fluxo de NC: abertura, contenção, investigação, plano de ação, eficácia, aprovações RT/GQ e encerramento.
- Fluxo de Recall: avaliação, aprovações, recolhimento, comunicação, retornos, destinação e encerramento.
- Auditoria automática de criações, alterações e exclusões.
- Anexos e evidências para RC, NC e Recall, com limite de 25 MB, formatos permitidos e anulação lógica.
- Dashboard com indicadores de processos abertos, aprovações pendentes e alertas de prazo.

## Tecnologias

- C# / .NET 10 e ASP.NET Core MVC
- Entity Framework Core + PostgreSQL 17
- ASP.NET Identity
- Bootstrap, HTML, CSS e JavaScript

## Como executar

Configure a conexão PostgreSQL por User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=sgq_dev;Username=postgres;Password=sua_senha" --project src/SGQ.Web
```

Em desenvolvimento, o seeder cria um usuário admin automaticamente. Configure as credenciais:

```powershell
dotnet user-secrets set "SeedAdmin:Email" "demo@sgq.local" --project src/SGQ.Web
dotnet user-secrets set "SeedAdmin:Password" "Admin@2026!" --project src/SGQ.Web
```

Execute a aplicação (as migrações são aplicadas automaticamente na inicialização):

```powershell
dotnet run --project src/SGQ.Web
```

## Desenvolvimento

Para validar a compilação:

```powershell
dotnet build SGQ.slnx --no-restore
```

Para criar ou desativar contas locais de teste com segurança, consulte [a documentação de usuários de teste](docs/usuarios-teste-development.md).

## Próximas evoluções

- Pesquisa, filtros, relatórios e exportação XLSX/PDF.
- Vínculo de uma mesma evidência a múltiplos processos sem duplicação física.
- Testes automatizados e CI.
