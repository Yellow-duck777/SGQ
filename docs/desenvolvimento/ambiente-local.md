# Ambiente local

## Dependências

- SDK .NET 10;
- PostgreSQL 17;
- `dotnet-ef` 10 (somente para criar migrations ou diagnosticar);
- Git;
- Node.js (somente para executar `npx markdownlint-cli2`);
- Um PostgreSQL acessível para os testes de integração (`SGQ_TEST_PG`); não é necessário Docker localmente.

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

## Primeiro acesso e contas sem perfil

O cadastro é aberto, mas uma conta sem perfil não acessa nenhuma tela de negócio: recebe 403 até que um Administrador atribua um perfil em **Usuários**. Para ter o primeiro Administrador:

1. Defina `InitialAdminEmail` com o e-mail da conta **antes do primeiro start** (a conta é cadastrada pela tela de login e recebe o perfil na inicialização seguinte, somente se ainda não existir nenhum Administrador); ou
2. se a aplicação já rodou, atribua o perfil Administrador diretamente no banco (tabelas `AspNetRoles` e `AspNetUserRoles`) a uma conta existente.

Uma conta existente sem perfil continua bloqueada até que o Administrador atribua um perfil em Usuários. Na inicialização, a aplicação registra um aviso (`LogWarning`) com a quantidade de contas sem perfil. Consulte também o [checklist de publicação](../desenvolvimento/publicacao-checklist.md).

## Testes

```powershell
dotnet test SGQ.slnx --no-build
```

Os testes de integração (`tests/SGQ.IntegrationTests`) exigem um PostgreSQL real, informado pela variável de ambiente `SGQ_TEST_PG` com a conexão do servidor sem nome de banco. Cada execução cria e descarta um banco próprio e nunca usa `sgq_dev`.

```powershell
$env:SGQ_TEST_PG = "Host=127.0.0.1;Port=5432;Username=postgres;Password=<senha-local>"
dotnet test SGQ.slnx --no-build
```

Sem a variável, esses testes aparecem como ignorados. O CI (`.github/workflows/ci.yml`) sempre os executa contra um serviço `postgres:17`.

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
