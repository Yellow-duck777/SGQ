# Requisitos de cadastros

Fonte completa: seção **6. Cadastros básicos** de [requisitos-consolidados.md](requisitos-consolidados.md).

## Situação atual

Clientes, produtos e lotes possuem criação, listagem e edição, com auditoria básica e acesso restrito a contas com perfil. Não existem inativação, paginação, pesquisa, concorrência otimista nem perfis específicos por operação.

## Itens que exigem validação da Qualidade

| Identificador | Decisão necessária | Estado |
| --- | --- | --- |
| CAD-001 | Campos e identificadores do cliente | Proposto |
| CAD-002 | Campos e identificadores do produto | Proposto |
| CAD-003 | Campos, datas e unicidade do lote por produto | Proposto |
| CAD-004 | Regras de inativação e reativação | Proposto |
| CAD-005 | Responsáveis autorizados por operação | Proposto |

Nenhum campo adicional deve ser inventado durante a implementação. A daily do módulo deverá registrar as decisões e atualizar a matriz antes do código.
