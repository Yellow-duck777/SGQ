# SGQ — Sistema de Gestão da Qualidade

Sistema web para registrar, tratar e rastrear processos de qualidade.

## Processos cobertos

- Reclamação de Cliente (RC)
- Não Conformidade (NC)
- Recall / Recolhimento de Produto

## Recursos implementados

- Autenticação com ASP.NET Identity.
- Cadastros de clientes, produtos e lotes.
- Numeração anual automática: `RC-AAAA-000001`, `NC-AAAA-000001` e `REC-AAAA-000001`.
- Fluxo de RC: abertura, validação, classificação, geração automática de NC, investigação, conclusão e encerramento.
- Fluxo de NC: abertura, contenção, investigação, plano de ação, eficácia, aprovações RT/GQ e encerramento.
- Fluxo de Recall: avaliação, aprovações, recolhimento, comunicação, retornos, destinação e encerramento.
- Perfis: Administrador, GQ, RT, CQ e Auditor; administração de usuários e perfis.
- Auditoria automática de criações, alterações e exclusões.
- Anexos e evidências para RC, NC e Recall, com limite de 25 MB, formatos permitidos e anulação lógica.
- Dashboard com indicadores de processos abertos e aprovações pendentes.

## Tecnologias

- C# / .NET 10
- ASP.NET Core MVC
- Entity Framework Core
- PostgreSQL
- ASP.NET Identity
- Bootstrap

## Como executar

1. Configure a conexão PostgreSQL por User Secrets:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=sgq;Username=postgres;Password=sua_senha" --project src/SGQ.Web
   ```

2. Defina o primeiro administrador (opcional, mas recomendado):

   ```powershell
   dotnet user-secrets set "InitialAdminEmail" "admin@empresa.com" --project src/SGQ.Web
   ```

   Crie ou registre esse usuário antes de iniciar a aplicação. Na inicialização, ele receberá o papel `Administrador`.

3. Aplique as migrações:

   ```powershell
   dotnet ef database update --project src/SGQ.Web --startup-project src/SGQ.Web
   ```

4. Execute a aplicação:

   ```powershell
   dotnet run --project src/SGQ.Web
   ```

## Apresentação local

Para mostrar o SGQ neste computador:

1. Abra o PowerShell na pasta do projeto.
2. Execute `dotnet run --project src/SGQ.Web`.
3. Abra no navegador o endereço mostrado após `Now listening on`. Por padrão, o perfil HTTP usa `http://localhost:5024`.
4. Quando terminar, pressione `Ctrl + C` no PowerShell para encerrar o servidor.

O endereço funciona apenas neste computador enquanto o servidor estiver ligado. As credenciais e as configurações do banco continuam locais e não são exibidas neste guia.

## Desenvolvimento

Para validar a compilação:

```powershell
dotnet build SGQ.slnx --no-restore
```

Para criar ou desativar contas locais de teste com segurança, consulte [a documentação de usuários de teste](docs/usuarios-teste-development.md).

Os requisitos e decisões de negócio estão em [docs/02-requisitos.md](docs/02-requisitos.md).
O roteiro detalhado, estado atual e backlog estão em [docs/03-planejamento-desenvolvimento.md](docs/03-planejamento-desenvolvimento.md).

## Próximas evoluções

- Calendário de dias úteis, prazos e notificações por e-mail.
- Laboratório externo, reabertura, prorrogação e decisão de divergência pelo CQ.
- Pesquisa, filtros, relatórios e exportação XLSX/PDF.
- Vínculo de uma mesma evidência a múltiplos processos sem duplicação física.
