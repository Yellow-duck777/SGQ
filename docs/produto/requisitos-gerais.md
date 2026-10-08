# Requisitos gerais

Fonte completa: [requisitos-consolidados.md](requisitos-consolidados.md).

## Estados documentais

| Estado | Significado |
| --- | --- |
| `Proposto` | Identificado, mas ainda não aprovado pela Qualidade e pelo revisor técnico |
| `Validado` | Aprovado para implementação |
| `Implementado` | Existe no código, mas ainda precisa de verificação independente |
| `Verificado` | Implementado e aceito com evidência de teste |
| `Adiado` | Fora da entrega atual, com justificativa registrada |

Somente requisitos `Validado`, `Implementado` ou `Verificado` orientam implementação ou alteração de código; `Proposto` e `Adiado` não autorizam código funcional. A mudança de estado deve ocorrer em PR revisado pela Qualidade e pelo responsável técnico.

## Mapa

| Grupo | Tema | Situação do código |
| --- | --- | --- |
| RF-001–RF-004 | Autenticação e autorização | Perfis, lockout, política padrão e de fallback e segregação RT/GQ implementados; MFA, aprovação de conta e perfis por ação de escrita pendentes |
| RF-005–RF-006 | Histórico e auditoria | Implementado em parte; sem imutabilidade nem FK de usuário |
| RF-007–RF-008 | Exclusão e inativação | Anulação lógica de anexos; inativação de cadastros pendente |
| RF-009 | Anexos | Implementado em parte; ver matriz |
| RF-010–RF-012 | Rastreabilidade, pesquisa e filtros | Implementados nas listagens de RC, NC e Recall |
| RF-013 | Data e hora | UTC e `DateOnly`; fuso explícito pendente |
| RF-014 | Notificações | Implementado (e-mail por SMTP) |
| RS-001–RS-007 | Segurança | Parcial; consulte a revisão de segurança |

## Aprovação

Antes de alterar um estado, registre na matriz a data, o responsável da Qualidade, o revisor técnico e a evidência usada. POPs são referenciados por código e versão; seus arquivos não pertencem ao repositório público.
