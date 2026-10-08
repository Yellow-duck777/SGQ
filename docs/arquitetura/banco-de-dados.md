# Banco de dados

## Estado atual

PostgreSQL armazena Identity e as tabelas do SGQ abaixo. O contexto é `ApplicationDbContext` (em `SGQ.Web/Data`) e o snapshot está em `SGQ.Web/Migrations`. O ambiente ainda não é produção e pode ser recriado.

### Tabelas do SGQ

| Tabela | Conteúdo | Relações principais |
| --- | --- | --- |
| `Clientes`, `Produtos`, `Lotes` | Cadastros básicos | `Lotes → Produtos` (`Restrict`) |
| `ReclamacoesClientes` | RC, incluindo dados de laboratório externo, resposta, validação, encerramento e reabertura | `→ Clientes`, `→ Produtos` (`Restrict`) |
| `ReclamacaoClienteLote` | Lotes de uma RC (chave composta) | `→ ReclamacoesClientes`, `→ Lotes` |
| `NaoConformidades` | NC, aprovações, decisão do CQ, laboratório externo e reabertura | `→ ReclamacoesClientes` (1:1 opcional), `→ Produtos` |
| `AcoesNaoConformidade` | Plano de ação da NC | `→ NaoConformidades` (`Cascade`) |
| `Recalls` | Recall, decisão, aprovações, comunicação, destinação e reabertura | `→ NaoConformidades`, `→ ReclamacoesClientes`, `→ Produtos`, `→ Lotes` (`Restrict`) |
| `RetornosRecall` | Retornos de produto | `→ Recalls` (`Cascade`) |
| `Anexos` | Metadados do arquivo (nome armazenado, tipo, tamanho, crítico, anulação); o conteúdo fica em disco | `→` RC, NC ou Recall de origem |
| `AnexosProcessosVinculos` | Vínculos adicionais do mesmo anexo a outros processos; `CHECK` de exatamente um processo | `→ Anexos` e processo de destino |
| `DiasNaoUteis` | Calendário (data, tipo, ativo); único por data e tipo | — |
| `AlertasEnviados` | Registro de envios para evitar repetição; único por tipo, referência e data | — |
| `ProrrogacoesPrazo` | Prorrogações com motivo, data anterior e comunicação ao cliente | `→` RC, NC ou Recall |
| `HistoricosAuditoria` | Auditoria por entidade, chave, ação, usuário e alterações | — |

Colunas de status e classificação são gravadas como texto. A sequência anual de cada processo possui índice único por ano e sequência.

### Migrations aplicadas

`InitialIdentity`, `AddCadastrosBasicos`, `RestringeExclusaoDeProdutoComLotes`, `AddReclamacoesClientes`, `AddNaoConformidades`, `AddRecalls`, `CompleteReclamacoesClientes`, `CompleteNaoConformidades`, `CompleteRecallWorkflow`, `AddHistoricoAuditoria`, `AddAnexos`, `AddAnexoCriticalidade`, `AddDatasAlvo`, `AddCalendarioDiasNaoUteis`, `AddAlertasPrazo`, `AddReaberturaRecall`, `AddReaberturaNaoConformidade`, `AddReaberturaReclamacao`, `AddLaboratorioExterno`, `AddProrrogacoesPrazo`, `AddDecisaoCqDivergencia`, `SyncCurrentModel` e `AddAnexoProcessoVinculos`.

### Migrations a criar na branch `fix/seguranca-fluxos-e-docs` (DEM-2026-008)

- `AddParecerUsuariosRtGq`: acrescenta `UsuarioParecerRt` e `UsuarioParecerGq` em `NaoConformidades` e `Recalls`, para registrar quem emitiu cada parecer e sustentar a segregação RT/GQ.
- `LimpaAuditoriaIdentity`: remove de `HistoricosAuditoria` os registros das entidades do Identity, que continham hash de senha e carimbo de segurança. A limpeza não é reversível.

## Problemas conhecidos

- sequência anual calculada com `Max + 1`, protegida apenas por índice único (um conflito concorrente falha em vez de repetir número);
- referências ao usuário são texto, sem FK;
- auditoria não é imutável e não guarda justificativa;
- não existem histórico de status, inativação nem concorrência otimista (`xmin`);
- quantidades não possuem precisão explícita;
- nomes físicos em PascalCase, com uma tabela no singular (`ReclamacaoClienteLote`);
- unicidades dos cadastros ainda dependem de validação da Qualidade;
- a migration `AddAnexoCriticalidade` adicionou `Ativo` aos anexos existentes; a consolidação das migrations antes da produção deve preservar a semântica `Ativo = true`, para que anexos antigos não passem a constar como anulados;
- o snapshot citava `SGQ.Web.Models.*` para entidades já movidas para `SGQ.Domain.Entities` (diferença só de nomes de tipo, sem efeito no esquema); ele foi regenerado ao criar `AddParecerUsuariosRtGq` e `dotnet ef migrations has-pending-model-changes` não acusa pendências. Repita essa verificação a cada alteração de modelo;
- as migrations são aplicadas na inicialização da aplicação em qualquer ambiente (`Program.cs`), o que contraria a regra de produção abaixo.

## Padrão alvo

- nomes físicos em `snake_case`;
- datas operacionais em UTC e datas civis em `date`;
- FKs e comportamentos de exclusão explícitos;
- inativação lógica para cadastros referenciados;
- tokens de concorrência para edição;
- sequência anual transacional;
- histórico imutável de eventos e estados;
- migrations reproduzíveis e testadas em PostgreSQL vazio.

## Anexos

Decisão atual: os arquivos ficam em disco, em `App_Data/uploads`, e a tabela `Anexos` guarda apenas metadados. A intenção original era `bytea` no PostgreSQL; o desvio está registrado no [ADR 0002](decisoes/0002-anexos-postgresql.md) e o lembrete de reverificação está no backlog (DEM-2026-102). O limite funcional é de 25 MB por arquivo e 20 por registro (o limite de 20 ainda não é aplicado). Backup e restauração precisam incluir o diretório de arquivos e o banco.

## Alterações

Toda mudança exige migration, snapshot coerente, atualização deste documento, teste de aplicação em banco limpo e análise de perda de dados. Em produção, as migrations devem ser aplicadas em etapa controlada, não silenciosamente na inicialização (DEM-2026-113).
