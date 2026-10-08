# Arquitetura de segurança

## Modelo de acesso aprovado

```text
Cadastro público
→ confirmação de e-mail
→ aprovação administrativa
→ atribuição de um ou mais perfis
→ MFA para perfis críticos
→ acesso operacional
```

Perfis implementados: Administrador, GQ, RT, CQ e Auditor. Os requisitos e o desenho original citam também "Colaborador autorizado" e outras áreas (Comercial, Produção etc.); incluir esses perfis depende de decisão da Qualidade (DEM-2026-101) e não deve ser inventado na implementação. Aprovações que exigem GQ e RT são registradas por contas distintas, e o Administrador não aprova como RT ou GQ.

## Controles

| Controle | Estado |
| --- | --- |
| autenticação obrigatória por padrão (política de fallback; conta sem perfil recebe 403) | Implementado (DEM-2026-008) |
| autorização no backend em transições críticas | Implementado |
| autorização por perfil em todas as ações de escrita | Planejado (DEM-2026-101) |
| segregação RT/GQ com usuário de cada parecer registrado | Implementado (DEM-2026-008) |
| antiforgery em todos os POSTs de MVC | Implementado |
| bloqueio de 15 minutos após cinco falhas | Implementado |
| senha entre 12 e 128 caracteres | Planejado (DEM-2026-113); hoje vale a política padrão do Identity |
| cadastro público, confirmação de e-mail e aprovação administrativa | Parcial: o cadastro existe e a conta só acessa o sistema após receber perfil; confirmação e aprovação formais estão planejadas (DEM-2026-005) |
| TOTP obrigatório para Administrador, GQ, RT e CQ | Planejado (DEM-2026-115) |
| rate limiting de autenticação e recuperação | Planejado (DEM-2026-113) |
| sessão inativa de 30 minutos e persistente de até sete dias | Planejado (DEM-2026-113) |
| autenticação recente para ações críticas | Planejado (DEM-2026-115) |
| cookies seguros, HSTS e cabeçalhos defensivos | Parcial: HSTS e HTTPS apenas fora de Development; cabeçalhos pendentes (DEM-2026-113) |
| chaves de Data Protection persistentes | Parcial: em arquivo local somente em Development (DEM-2026-113) |
| auditoria imutável e retenção de dois anos | Parcial: a auditoria existe e deixa de gravar entidades do Identity, mas é mutável (DEM-2026-004) |

## Dados e repositório público

Não são permitidos dados reais, POPs, dumps, credenciais, tokens, nomes confidenciais ou documentos internos. Seeds e testes usam dados fictícios. Logs não registram senhas, tokens, conteúdo de anexos nem dados pessoais desnecessários. A pasta `App_Data/` (anexos enviados) fica fora do Git.

## Anexos

| Controle | Estado |
| --- | --- |
| lista de extensões permitidas e limite de 25 MB no servidor | Implementado |
| limite de 20 anexos por registro | Planejado (DEM-2026-050) |
| assinatura real compatível com o formato e validação de MIME | Planejado (DEM-2026-050) |
| hash SHA-256 | Planejado (DEM-2026-050) |
| varredura ClamAV | Planejado (DEM-2026-050) |
| download autenticado; anexo anulado só para GQ, Administrador e Auditor | Implementado (DEM-2026-008) |
| cabeçalhos que impeçam execução inline indevida | Planejado (DEM-2026-050) |
| armazenamento (disco atual x `bytea`) | Decisão a reverificar (DEM-2026-102, [ADR 0002](decisoes/0002-anexos-postgresql.md)) |

## Acompanhamento

Achados e evidências ficam em [revisao-seguranca.md](../qualidade/revisao-seguranca.md). Vulnerabilidades exploráveis seguem [SECURITY.md](../../SECURITY.md), nunca issues públicas.
