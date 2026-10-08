# Matriz de rastreabilidade

Esta matriz liga requisitos, código, testes e evidências. O detalhamento por requisito deve ser acrescentado no mesmo PR que o valida ou implementa.

## Responsáveis

- validação de negócio: responsável da Qualidade;
- validação técnica: `@ViniciusLgo`;
- implementação: autor do PR;
- verificação: pessoa diferente do autor quando houver aprovação regulada.

## Matriz inicial

| Requisito | Estado | Implementação atual | Teste/evidência | Próxima ação |
| --- | --- | --- | --- | --- |
| RF-001 | Implementado | ASP.NET Core Identity | Roteiro AUTH-001 | Endurecer e automatizar |
| RF-002 | Implementado | Usuário autenticado associado à abertura de RC | Roteiro AUTH-002 | Trocar texto por FK auditável |
| RF-003–RF-004 | Proposto | Apenas `[Authorize]`, sem perfis | SEC-001 | Implementar políticas |
| RF-005–RF-006 | Proposto | Ausente | SEC-002 | Modelar auditoria |
| RF-007–RF-008 | Proposto | Ausente | CAD-004 | Validar inativação |
| RF-009 | Validado | Ausente | ANX-001–ANX-006 | Implementar após fundação |
| RF-010–RF-014 | Proposto | Ausente/parcial | A definir na validação | Validar recortes |
| RF-RC-001–RF-RC-014 | Implementado | Abertura de rascunho parcial | RC-001 | Completar validações e automatizar |
| RF-RC-015–RF-RC-039 | Validado | Ausente/parcial | RC-002–RC-010 | Implementar em fatias verticais |
| RF-NC-001–RF-NC-032 | Validado | Modelo e leitura parciais | NC-001–NC-010 | Implementar fluxo completo |
| RF-RECALL-001–RF-RECALL-039 | Validado | Ausente | REC-001–REC-010 | Implementar após NC |
| RN-001–RN-024 | Validado | Ausente/parcial | Matriz de regras | Implementar com módulos |
| RS-001–RS-002 | Implementado | Identity e hash de senha | AUTH-001 | Endurecer configuração |
| RS-003–RS-005 | Proposto | Ausente/parcial | SEC-001–SEC-003 | Implementar autorização/auditoria/anexos |
| RS-006–RS-007 | Verificado | User Secrets e varredura local | SEC-004 | Automatizar no CI |

## Registro de validação

Cada alteração de estado deve acrescentar no PR: requisito, estado anterior, estado novo, nome/função dos validadores, data e link da evidência. Não coloque assinatura, e-mail pessoal ou documento interno neste arquivo público.

## Pendências históricas

A seção “Pontos Pendentes” do documento consolidado preserva o histórico. Itens que receberam decisão na seção 14 não estão mais abertos. Os demais só poderão ser marcados como encerrados após validação conjunta e atualização desta matriz.
