# Usuários de teste em Development

O projeto oferece um comando explícito para criar e desativar contas de teste sem guardar senhas no repositório. Ele usa o ASP.NET Identity, portanto os hashes são criados pelo `UserManager` e nunca são inseridos diretamente no banco.

## Proteções

- Só executa quando `ASPNETCORE_ENVIRONMENT=Development`.
- Exige `DevelopmentTestUsers:Enabled=true` nos User Secrets.
- A connection string deve apontar exatamente para `DevelopmentTestUsers:ExpectedDatabase` e esse banco deve terminar em `_test` ou `_tests`.
- Só aceita e-mails no domínio reservado `@sgq.test`.
- Só aceita os perfis já existentes: `Administrador`, `GQ`, `RT`, `CQ` e `Auditor`.
- Não é executado no início normal da aplicação.

Use um banco dedicado, por exemplo `sgq_development_tests`, e aplique nele as migrations antes de criar as contas.

## Configuração local

Todos os valores abaixo devem ser definidos com `dotnet user-secrets set` no projeto `src/SGQ.Web`. As senhas são locais e não devem ser incluídas em arquivos versionados.

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string-do-banco-sgq_development_tests>" --project src/SGQ.Web
dotnet user-secrets set "DevelopmentTestUsers:Enabled" "true" --project src/SGQ.Web
dotnet user-secrets set "DevelopmentTestUsers:ExpectedDatabase" "sgq_development_tests" --project src/SGQ.Web
```

Configure uma conta para cada perfil: `Administrador`, `GQ`, `RT`, `CQ` e `Auditor`. Para cada conta, defina `Email`, `Password` e `Roles:0`; a senha deve ser gerada localmente.

```powershell
dotnet user-secrets set "DevelopmentTestUsers:Users:0:Email" "<usuario>@sgq.test" --project src/SGQ.Web
dotnet user-secrets set "DevelopmentTestUsers:Users:0:Password" "<senha-local-gerada>" --project src/SGQ.Web
dotnet user-secrets set "DevelopmentTestUsers:Users:0:Roles:0" "Administrador" --project src/SGQ.Web
```

## Operações

Criar as contas configuradas:

```powershell
dotnet run --project src/SGQ.Web -- --bootstrap-test-users
```

Desativar apenas as contas `@sgq.test` que estejam configuradas nos User Secrets:

```powershell
dotnet run --project src/SGQ.Web -- --remove-test-users
```

O bootstrap é idempotente: se uma conta já existir com exatamente os mesmos perfis, ela é preservada e sua senha não é alterada. Se os perfis forem diferentes, o comando falha e exige desativação explícita antes de recriar a conta.

O comando de limpeza bloqueia a conta indefinidamente e invalida as sessões ativas. Ele não faz exclusão física, preservando a rastreabilidade de ações registradas na auditoria. Contas fora de `@sgq.test` não são alcançadas pelo comando.

`Gestor` e `Colaborador autorizado` não são perfis implementados e não podem ser usados nesse mecanismo sem uma decisão de regra de negócio.
