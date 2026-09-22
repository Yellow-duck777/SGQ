# Ambiente local

## Dependências

- SDK .NET 10;
- PostgreSQL 17;
- `dotnet-ef` 10;
- Docker Desktop para testes isolados;
- Git.

## Conexão local

Crie o banco `sgq_dev` e configure a conexão com User Secrets:

```powershell
$senha = Read-Host "Senha do PostgreSQL" -AsSecureString
$senhaTexto = [System.Net.NetworkCredential]::new('', $senha).Password
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=sgq_dev;Username=postgres;Password=$senhaTexto" --project .\src\SGQ.Web\SGQ.Web.csproj
Remove-Variable senha, senhaTexto
```

Nunca grave a conexão com senha em `appsettings*.json`.

## Banco e aplicação

```powershell
dotnet ef database update --project .\src\SGQ.Web\SGQ.Web.csproj --startup-project .\src\SGQ.Web\SGQ.Web.csproj
dotnet run --project .\src\SGQ.Web\SGQ.Web.csproj --launch-profile http
```

Endereço padrão: `http://localhost:5024`.

## Diagnóstico

```powershell
dotnet --info
dotnet build SGQ.slnx
dotnet ef migrations list --project .\src\SGQ.Web\SGQ.Web.csproj --startup-project .\src\SGQ.Web\SGQ.Web.csproj
```

Se `dotnet ef` não existir:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.11
```
