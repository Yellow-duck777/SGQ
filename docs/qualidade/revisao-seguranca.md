# Revisão de segurança

Baseline: 2026-09-22. Atualização: 2026-10-08 (branch `fix/seguranca-fluxos-e-docs`, DEM-2026-008).

Estados: `Aberto`, `Parcial`, `Corrigido na branch` (código e teste na branch, a confirmar por revisão e CI) e `Resolvido` (após revisão e CI). Um achado só passa a `Resolvido` depois de código, teste e revisão; documentação isolada não o encerra.

## Achados

| ID | Severidade | Achado | Evidência | Estado | Tratamento |
| --- | --- | --- | --- | --- | --- |
| SEG-001 | Alta | Cadastro padrão concedia acesso aos módulos após login | Controllers usavam apenas `[Authorize]` | Corrigido na branch | Política de fallback: conta sem perfil recebe 403; roteiro SEC-001 |
| SEG-002 | Alta | Não existiam perfis nem segregação de aprovações | Identity sem roles | Parcial | Cinco perfis existem; segregação RT/GQ corrigida na branch; perfis por ação de escrita pendentes (DEM-2026-101) |
| SEG-003 | Média | Falhas de login não contavam para lockout | `lockoutOnFailure: false` | Resolvido no código | Hoje `lockoutOnFailure: true`, cinco falhas e 15 minutos; falta teste automatizado |
| SEG-004 | Média | Dashboard e navegação públicos | `HomeController` sem autorização | Parcial | `Index` redireciona anônimos ao login e a política de fallback cobre o restante; confirmar o comportamento de `Privacy` |
| SEG-005 | Alta | Sem histórico auditável de ações críticas | Ausência de serviço de auditoria | Parcial | Auditoria existe ([ADR 0010](../arquitetura/decisoes/0010-auditoria-savechanges.md)); é mutável e o usuário é texto (DEM-2026-004) |
| SEG-006 | Média | Edições recebem entidades persistentes e usam `Update` | Controllers de cadastros | Aberto | ViewModels e atualização explícita (DEM-2026-010) |
| SEG-007 | Média | Sem rate limiting e cabeçalhos endurecidos | Pipeline atual | Aberto | DEM-2026-113 |
| SEG-008 | Alta | Anexos sem validação de conteúdo e antimalware | Apenas extensão e tamanho | Aberto | Assinatura, SHA-256, ClamAV, limite de 20 e download seguro (DEM-2026-050) |
| SEG-009 | Alta | `AvancarStatus` permitia a qualquer conta autenticada avançar uma RC até `Encerrada`, e `Classificar` não tinha restrição de perfil | `ReclamacoesController` | Corrigido na branch | `AvancarStatus` removido; `Classificar` restrito a GQ e Administrador, somente com a RC em andamento, com enum validado |
| SEG-010 | Alta | Mesma conta (ou o Administrador) podia emitir os pareceres RT e GQ | `Aprovar` e `Reprovar` de NC e Recall | Corrigido na branch | Contas distintas, Administrador excluído e usuário do parecer gravado ([ADR 0011](../arquitetura/decisoes/0011-autorizacao-perfis-segregacao.md)) |
| SEG-011 | Alta | A auditoria gravava entidades do Identity, incluindo hash de senha | `SaveChangesAsync` | Corrigido na branch | Entidades do Identity excluídas; migration `LimpaAuditoriaIdentity` apaga os registros antigos |
| SEG-012 | Média | `App_Data/uploads` não estava no `.gitignore`, permitindo versionar evidências | `.gitignore` | Corrigido na branch | `App_Data/` ignorado |
| SEG-013 | Média | Exportação CSV sem neutralização de fórmulas | `RelatoriosController` | Corrigido na branch | Células iniciadas por `=`, `+`, `-` ou `@` são neutralizadas |
| SEG-014 | Média | `InitialAdminEmail` promovia a conta a Administrador em toda inicialização | `Program.cs` | Corrigido na branch | Só promove se ainda não existir Administrador |
| SEG-015 | Média | Download de anexo anulado liberado a qualquer autenticado | `AnexosController.Baixar` | Corrigido na branch | Anexo anulado só para GQ, Administrador e Auditor |
| SEG-016 | Média | Reabertura e novas rodadas de aprovação não zeravam decisão do CQ e flags; Recall podia travar em divergência | NC e Recall | Corrigido na branch | Estado de aprovação limpo; encerramento aceita decisão favorável do CQ |
| SEG-017 | Média | Migrations aplicadas na inicialização em qualquer ambiente | `Program.cs` | Aberto | DEM-2026-113 |
| SEG-018 | Baixa | Dedupe de notificações por tipo, referência e dia pode suprimir eventos legítimos | `FluxoNotificacaoService` | Aberto | DEM-2026-105 |

## Controles observados

- Identity armazena hash de senha;
- antiforgery global em POSTs de MVC;
- login usa `LocalRedirect`;
- conexão e SMTP em User Secrets;
- nenhuma credencial encontrada no repositório (varredura manual de 2026-10-08); a senha de exemplo do README foi substituída por placeholder;
- a varredura de pacotes NuGet vulneráveis da baseline não foi repetida em 2026-10-08 e deve passar a rodar no CI.

## Regras de acompanhamento

Cada correção referencia o ID do achado e adiciona teste. A severidade só muda com justificativa.
