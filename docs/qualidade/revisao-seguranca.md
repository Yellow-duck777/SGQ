# Revisão de segurança

Data da baseline: 2026-09-22.

## Achados

| ID | Severidade | Achado | Evidência | Tratamento |
| --- | --- | --- | --- | --- |
| SEG-001 | Alta | Cadastro padrão concede acesso aos módulos após login | Controllers usam apenas `[Authorize]` | Aprovação de conta e políticas por perfil |
| SEG-002 | Alta | Não existem perfis, permissões ou segregação de aprovações | Identity sem roles configuradas | Cinco perfis e políticas no backend |
| SEG-003 | Média | Falhas de login não contam para lockout | `lockoutOnFailure: false` | Cinco falhas e bloqueio de 15 minutos |
| SEG-004 | Média | Dashboard e navegação são públicos | `HomeController` sem autorização | Login como entrada e política global |
| SEG-005 | Alta | Não existe histórico auditável de ações críticas | Ausência de entidade/serviço de auditoria | Auditoria imutável |
| SEG-006 | Média | Edições recebem entidades persistentes e usam `Update` | Controllers de cadastros | ViewModels e atualização explícita |
| SEG-007 | Média | Não existem rate limiting e headers endurecidos | Pipeline atual | Middleware e políticas de produção |
| SEG-008 | Alta | Anexos protegidos e antimalware não existem | Requisito sem implementação | Validação, ClamAV e download autorizado |

## Controles observados

- Identity armazena hash de senha;
- POSTs próprios usam antiforgery;
- login usa `LocalRedirect`;
- conexão é esperada em User Secrets;
- nenhuma credencial foi encontrada no repositório;
- build passou sem warnings;
- a varredura atual não encontrou pacote NuGet vulnerável conhecido.

## Regras de acompanhamento

Cada correção referencia o ID do achado e adiciona teste. A severidade só muda com justificativa. Um achado passa a resolvido apenas depois de código, teste e revisão; documentação isolada não o encerra.
