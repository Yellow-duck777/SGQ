# Ambiente local

## Dependências

- SDK .NET 10;
- PostgreSQL 17;
- `dotnet-ef` 10 (somente para criar migrations ou diagnosticar);
- Git;
- Node.js (somente para executar `npx markdownlint-cli2`);
- Docker Desktop: necessário apenas quando existirem testes com PostgreSQL descartável (planejado; ver [backlog](../gestao/backlog.md)).

## Conexão local

Crie o banco `sgq_dev` e configure a conexão com User Secrets:

```powershell
$senha = Read-Host "Senha do PostgreSQL" -AsSecureString
$senhaTexto = [System.Net.NetworkCredential]::new('', $senha).Password
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=sgq_dev;Username=postgres;Password=$senhaTexto" --project .\src\SGQ.Web\SGQ.Web.csproj
Remove-Variable senha, senhaTexto
```

Nunca grave a conexão com senha em `appsettings*.json`.

## Configurações opcionais (User Secrets)

| Chave | Finalidade |
| --- | --- |
| `InitialAdminEmail` | Conta que recebe o perfil Administrador na inicialização, somente se ainda não existir nenhum Administrador |
| `SeedAdmin:Email` e `SeedAdmin:Password` | Administrador local criado apenas em Development |
| `Smtp:Host`, `Port`, `EnableSsl`, `UserName`, `Password`, `From` | Envio de notificações e alertas; sem eles nada é enviado |
| `DevelopmentTestUsers:*` | Contas de teste, descritas em [usuários de teste](../usuarios-teste-development.md) |

Arquivos de anexos ficam em `src/SGQ.Web/App_Data/uploads` e as chaves de Data Protection em `src/SGQ.Web/.data-protection`. Ambos são locais e não devem ser versionados.

## Banco e aplicação

A aplicação aplica as migrations pendentes ao iniciar. Para aplicá-las manualmente:

```powershell
dotnet ef database update --project .\src\SGQ.Web\SGQ.Web.csproj --startup-project .\src\SGQ.Web\SGQ.Web.csproj
dotnet run --project .\src\SGQ.Web\SGQ.Web.csproj --launch-profile http
```

Endereço padrão: `http://localhost:5024`.

## Apresentação local

Para mostrar o SGQ neste computador:

1. Abra o PowerShell na pasta do projeto.
2. Execute `dotnet run --project src/SGQ.Web`.
3. Abra no navegador o endereço mostrado após `Now listening on`. Por padrão, o perfil HTTP usa `http://localhost:5024`.
4. Quando terminar, pressione `Ctrl + C` no PowerShell para encerrar o servidor.

O endereço funciona apenas neste computador enquanto o servidor estiver ligado. Credenciais e configurações do banco continuam locais.

## Diagnóstico

```powershell
dotnet --info
dotnet build SGQ.slnx
dotnet ef migrations list --project .\src\SGQ.Web\SGQ.Web.csproj --startup-project .\src\SGQ.Web\SGQ.Web.csproj
dotnet ef migrations has-pending-model-changes --project .\src\SGQ.Web\SGQ.Web.csproj --startup-project .\src\SGQ.Web\SGQ.Web.csproj
```

Se `dotnet ef` não existir:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.11
```
