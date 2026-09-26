# Requisitos de Reclamação de Cliente

Fonte completa: seções **7**, **10** e **Status da Reclamação de Cliente** de [requisitos-consolidados.md](requisitos-consolidados.md).

## Escopo documentado

- RF-RC-001–RF-RC-019: registro e completude;
- RF-RC-020–RF-RC-025: classificação, recorrência e risco;
- RF-RC-026–RF-RC-033: investigação, conclusão e tratamento;
- RF-RC-034–RF-RC-039: resposta, prazo, NC e encerramento;
- ST-RC-001–ST-RC-007: estados permitidos;
- RN-RC-STATUS-001: reclamação não pode ser cancelada.

## Situação atual

A aplicação cria uma RC em `Rascunho`, gera código anual e relaciona cliente, produto e lotes. Não implementa validação da GQ, transições, prazo, anexos, investigação, conclusão, resposta, auditoria nem geração automática da NC.

## Critério para iniciar implementação

Todos os requisitos do recorte do PR devem estar `Validado`, possuir critérios de aceite e casos negativos. A criação automática da NC deve ser atômica com a validação da GQ.
