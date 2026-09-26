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

Somente requisitos `Validado`, `Implementado` ou `Verificado` podem orientar código. A mudança de estado deve ocorrer em PR revisado pela Qualidade e pelo responsável técnico.

## Mapa

| Grupo | Tema | Situação do código |
| --- | --- | --- |
| RF-001–RF-004 | Autenticação e autorização | Autenticação parcial; autorização granular ausente |
| RF-005–RF-006 | Histórico e auditoria | Não implementado |
| RF-007–RF-008 | Exclusão e inativação | Não implementado |
| RF-009 | Anexos | Não implementado |
| RF-010–RF-012 | Rastreabilidade, pesquisa e filtros | Não implementado |
| RF-013 | Data e hora | Uso parcial de UTC |
| RF-014 | Notificações | Não implementado |
| RS-001–RS-007 | Segurança | Parcial; consulte a revisão de segurança |

## Aprovação

Antes de alterar um estado, registre na matriz a data, o responsável da Qualidade, o revisor técnico e a evidência usada. POPs são referenciados por código e versão; seus arquivos não pertencem ao repositório público.
