# Matriz de testes

Esta matriz será expandida junto com a [matriz de rastreabilidade](../produto/matriz-rastreabilidade.md).

| ID | Área | Nível | Resultado esperado | Estado |
| --- | --- | --- | --- | --- |
| AUTH-001 | Login válido e inválido | Integração/E2E | Autentica somente conta apta | Manual |
| AUTH-002 | Identidade do autor | Integração | Grava FK do usuário autenticado | Planejado |
| SEC-001 | Acesso por perfil e URL | Integração/E2E | Retorna 403 sem permissão | Planejado |
| SEC-002 | Auditoria | Unidade/Integração | Evento imutável contém ator e mudança | Planejado |
| SEC-003 | Segregação GQ/RT | Unidade/Integração | Mesma conta não registra duas aprovações | Planejado |
| SEC-004 | Segredos e dependências | CI | Nenhum segredo ou vulnerabilidade conhecida | Manual |
| CAD-004 | Inativação | Integração/E2E | Registro referenciado não é apagado | Planejado |
| RC-001 | Abertura de rascunho | Integração/E2E | Gera código anual e vínculos válidos | Manual |
| RC-002–RC-010 | Fluxo de RC | Todos | Cobre transições, prazo e encerramento | Planejado |
| NC-001–NC-010 | Fluxo de NC | Todos | Cobre origem, tratamento e aprovações | Planejado |
| REC-001–REC-010 | Fluxo de Recall | Todos | Cobre avaliação até encerramento | Planejado |
| ANX-001–ANX-006 | Anexos | Integração/Segurança | Valida acesso, formato, malware e limites | Planejado |

Estados: `Planejado`, `Automatizado`, `Manual`, `Verificado` ou `Bloqueado`.
