# Checklist de publicação

Use este checklist antes de aplicar uma versão em qualquer ambiente com dados que precisem ser preservados. Ele substitui o checklist de pré-entrega do planejamento histórico. O SGQ ainda não está pronto para produção (ver [roadmap](../gestao/roadmap.md), onda 10); os itens abaixo reduzem o risco enquanto isso.

## Antes de aplicar

- [ ] **Backup do banco** com `pg_dump` (formato custom) imediatamente antes da publicação, e confirmação de que a restauração funciona em um banco de teste.
- [ ] **Backup da pasta `App_Data`** junto com o banco, enquanto os anexos forem gravados em disco (ver [ADR 0002](../arquitetura/decisoes/0002-anexos-postgresql.md)). Banco e arquivos precisam ser do mesmo instante para que não haja anexos órfãos.
- [ ] **Revisar as migrations pendentes** gerando o script: `dotnet ef migrations script <última-aplicada> --idempotent --project src/SGQ.Web/SGQ.Web.csproj --startup-project src/SGQ.Web/SGQ.Web.csproj`. Leia o SQL antes de executar.
- [ ] **Atenção à `LimpaAuditoriaIdentity`**: ela mascara dados sensíveis em `HistoricosAuditoria` (`PasswordHash`, `SecurityStamp`, `ConcurrencyStamp` e tokens) e apaga linhas de `IdentityUserToken` e `IdentityUserLogin`. É irreversível: o `Down` é intencionalmente vazio.
- [ ] **Rodar os testes de integração contra uma cópia do banco** (restauração do backup em um servidor de teste) e `dotnet test SGQ.slnx` com `SGQ_TEST_PG` apontando para esse servidor.
- [ ] Confirmar que o CI do commit a publicar está verde.

## Aplicando

- [ ] Aplicar as migrations pelo script revisado (ou conscientemente pela inicialização da aplicação, que hoje executa `Migrate()` em qualquer ambiente; ver DEM-2026-113 no [backlog](../gestao/backlog.md)).
- [ ] Verificar `dotnet ef migrations has-pending-model-changes` no commit publicado.

## Configuração e acessos

- [ ] **Contas sem perfil**: consultar o aviso de inicialização (quantidade de contas sem perfil) e as tabelas `AspNetUsers` e `AspNetUserRoles`. Contas sem perfil ficam bloqueadas até um Administrador atribuir perfil em **Usuários**.
- [ ] **`InitialAdminEmail`** definido (por User Secrets ou variável de ambiente) antes do primeiro start, se ainda não existir Administrador. Só promove a conta quando não existe nenhum Administrador.
- [ ] **SMTP** configurado (`Smtp:Host`, `Port`, `EnableSsl`, `UserName`, `Password`, `From`); sem isso nenhuma notificação ou alerta é enviado.
- [ ] Desativar `DevelopmentTestUsers` e `SeedAdmin`; remover ou bloquear contas de teste (`@sgq.test`).
- [ ] Nenhuma senha em código-fonte ou `appsettings.json`; usuário de banco da aplicação sem privilégios de superusuário e com senha trocada.
- [ ] Pasta `App_Data` e chaves de Data Protection em volume persistente.
- [ ] Testar login, conexão com o banco e um fluxo completo com contas de cada perfil, usando o roteiro [SEC-001](../qualidade/roteiros/sec-001-acesso-por-perfil.md).

## Plano de rollback

1. Interromper a aplicação.
2. Restaurar o backup do banco (`pg_restore`) e a pasta `App_Data` do mesmo instante.
3. Publicar novamente a versão anterior do aplicativo.

Não há rollback por migration: a `Down` da `LimpaAuditoriaIdentity` é vazia e a restauração do backup é o único caminho para recuperar os dados originais da auditoria (que continham segredos; avalie se restaurá-los é aceitável).
