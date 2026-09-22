# Banco de dados

## Estado atual

PostgreSQL armazena Identity, clientes, produtos, lotes, RC e NC. As migrations aplicadas são:

- `InitialIdentity`;
- `AddCadastrosBasicos`;
- `RestringeExclusaoDeProdutoComLotes`;
- `AddReclamacoesClientes`;
- `AddNaoConformidades`.

O ambiente ainda não é produção e pode ser recriado. A consolidação das migrations ocorrerá na fundação técnica, em PR separado.

## Problemas conhecidos

- abertura de RC usa `Max + 1` para sequência anual;
- referências ao usuário são texto, sem FK;
- não existem auditoria e histórico de status;
- não existem inativação e concorrência otimista;
- quantidades não possuem precisão explícita;
- unicidades dos cadastros ainda dependem de validação da Qualidade;
- anexos, calendário, notificações e Recall não estão modelados.

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

Por decisão do produto, o conteúdo ficará no PostgreSQL em coluna `bytea`, separado dos dados operacionais. O limite é de 25 MB por arquivo e 20 arquivos por registro, podendo alcançar 500 MB por processo. Backup, restauração, consumo de memória e crescimento serão critérios obrigatórios.

## Alterações

Toda mudança exige migration, snapshot coerente, atualização deste documento, teste de aplicação em banco limpo e análise de perda de dados. Migrations nunca devem executar silenciosamente na inicialização de produção.
