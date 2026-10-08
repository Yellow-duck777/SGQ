# Revisão de segurança

Baseline: 2026-09-22. Atualização: 2026-10-08 (branch `fix/seguranca-fluxos-e-docs`, DEM-2026-008).

Estados: `Aberto`, `Parcial`, `Corrigido na branch` (código e teste na branch, a confirmar por revisão e CI), `Corrigido, com teste` e `Resolvido` (após revisão e CI). Um achado só passa a `Resolvido` depois de código, teste e revisão; documentação isolada não o encerra.

## Achados

| ID | Severidade | Achado | Evidência | Estado | Tratamento |
| --- | --- | --- | --- | --- | --- |
| SEG-001 | Alta | Cadastro padrão concedia acesso aos módulos após login | Controllers usavam apenas `[Authorize]` | Corrigido, com teste de integração | Política padrão e de fallback: conta sem perfil recebe 403; testes `AutenticadoSemPerfil_RecebeAcessoNegado` e `QualquerPerfilReconhecido_AcessaListagensEDashboard`; roteiro SEC-001. Ver Errata |
| SEG-002 | Alta | Não existiam perfis nem segregação de aprovações | Identity sem roles | Parcial | Cinco perfis existem; segregação RT/GQ corrigida na branch; perfis por ação de escrita pendentes (DEM-2026-101) |
| SEG-003 | Média | Falhas de login não contavam para lockout | `lockoutOnFailure: false` | Resolvido no código | Hoje `lockoutOnFailure: true`, cinco falhas e 15 minutos; falta teste automatizado |
| SEG-004 | Média | Dashboard e navegação públicos | `HomeController` sem autorização | Corrigido, com teste de integração | Anônimo é bloqueado e conta sem perfil recebe 403 em `/`, listagens e `/Home/Privacy`; páginas públicas limitam-se a login, estilos e erro |
| SEG-005 | Alta | Sem histórico auditável de ações críticas | Ausência de serviço de auditoria | Parcial | Auditoria existe ([ADR 0010](../arquitetura/decisoes/0010-auditoria-savechanges.md)); é mutável e o usuário é texto (DEM-2026-004) |
| SEG-006 | Média | Edições recebem entidades persistentes e usam `Update` | Controllers de cadastros | Aberto | ViewModels e atualização explícita (DEM-2026-010) |
| SEG-007 | Média | Sem rate limiting e cabeçalhos endurecidos | Pipeline atual | Aberto | DEM-2026-113 |
| SEG-008 | Alta | Anexos sem validação de conteúdo e antimalware | Apenas extensão e tamanho | Aberto | Assinatura, SHA-256, ClamAV, limite de 20 e download seguro (DEM-2026-050) |
| SEG-009 | Alta | `AvancarStatus` permitia a qualquer conta autenticada avançar uma RC até `Encerrada`, e `Classificar` não tinha restrição de perfil | `ReclamacoesController` | Corrigido na branch | `AvancarStatus` removido; `Classificar` restrito a GQ e Administrador, somente com a RC em andamento, com enum validado |
| SEG-010 | Alta | Mesma conta (ou o Administrador) podia emitir os pareceres RT e GQ | `Aprovar` e `Reprovar` de NC e Recall | Corrigido na branch | Contas distintas, Administrador excluído e usuário do parecer gravado ([ADR 0011](../arquitetura/decisoes/0011-autorizacao-perfis-segregacao.md)) |
| SEG-011 | Alta | A auditoria gravava entidades do Identity, incluindo hash de senha | `SaveChangesAsync` | Corrigido, verificado em PostgreSQL real | Entidades do Identity excluídas; `LimpaAuditoriaIdentity` mascara `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp` e tokens e apaga linhas de `IdentityUserToken` e `IdentityUserLogin`; testes `CriarUsuario_NaoGravaHashNemCarimbosNaAuditoria` e `AtribuirPerfil_FicaRegistradoNaAuditoria` |
| SEG-012 | Média | `App_Data/uploads` não estava no `.gitignore`, permitindo versionar evidências | `.gitignore` | Corrigido na branch | `App_Data/` ignorado |
| SEG-013 | Média | Exportação CSV sem neutralização de fórmulas | `RelatoriosController` | Corrigido na branch | Células iniciadas por `=`, `+`, `-` ou `@` são neutralizadas |
| SEG-014 | Média | `InitialAdminEmail` promovia a conta a Administrador em toda inicialização | `Program.cs` | Corrigido na branch | Só promove se ainda não existir Administrador |
| SEG-015 | Média | Download de anexo anulado liberado a qualquer autenticado | `AnexosController.Baixar` | Corrigido na branch | Anexo anulado só para GQ, Administrador e Auditor |
| SEG-016 | Média | Reabertura e novas rodadas de aprovação não zeravam decisão do CQ e flags; Recall podia travar em divergência | NC e Recall | Corrigido na branch | Estado de aprovação limpo; encerramento aceita decisão favorável do CQ |
| SEG-017 | Média | Migrations aplicadas na inicialização em qualquer ambiente | `Program.cs` | Aberto | DEM-2026-113 |
| SEG-018 | Baixa | Dedupe de notificações por tipo, referência e dia pode suprimir eventos legítimos | `FluxoNotificacaoService` | Aberto | DEM-2026-105 |

## Errata: política de acesso por perfil

A correção do SEG-001 entregue no PR #3 definia apenas a política de fallback. Um `[Authorize]` simples, usado pelos controllers, ignora o fallback e usa a política padrão; assim, contas sem perfil continuaram a acessar os módulos. O defeito foi encontrado pelo novo teste de integração em PostgreSQL real (`SGQ.IntegrationTests`) e corrigido definindo a mesma política como padrão e como fallback. Lição: correções de autorização precisam de teste contra o pipeline real; a revisão de código e os testes unitários de controller não detectam esse tipo de falha.

## Controles observados

- Identity armazena hash de senha;
- antiforgery global em POSTs de MVC;
- login usa `LocalRedirect`;
- conexão e SMTP em User Secrets;
- a limpeza da auditoria foi executada em PostgreSQL 17 real, em banco vazio e em banco existente com segredos na auditoria;
- nenhuma credencial encontrada no repositório (varredura manual de 2026-10-08); a senha de exemplo do README foi substituída por placeholder;
- a varredura de pacotes NuGet vulneráveis da baseline não foi repetida; fica a cargo do CI (ver estratégia de testes).

## Regras de acompanhamento

Cada correção referencia o ID do achado e adiciona teste. A severidade só muda com justificativa.
