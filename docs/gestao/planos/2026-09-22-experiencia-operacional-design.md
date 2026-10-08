# Desenho da experiência operacional do SGQ

**Data:** 22/09/2026  
**Estado:** aprovado para detalhamento técnico; implementação não iniciada em 2026-10-08
**Escopo:** dashboard, cadastros básicos e configurações da conta

## 1. Objetivo

Consolidar a identidade visual industrial do SGQ e tornar as tarefas diárias
mais rápidas, previsíveis e seguras. A entrega cobrirá o dashboard autenticado,
os CRUDs de clientes, produtos e lotes e as páginas de configurações da conta.

Este desenho não altera os fluxos regulados de RC, NC ou Recall. Ele prepara
componentes e comportamentos compartilhados que esses módulos poderão reutilizar.

## 2. Decisões aprovadas

- manter a sidebar fixa como estrutura principal após o login;
- manter a paleta verde profundo, verde secundário, verde-limão e fundo claro;
- exibir somente números reais no dashboard;
- padronizar tabelas, formulários, mensagens, estados vazios e ações;
- incluir busca, filtros, paginação e inativação nos cadastros básicos;
- traduzir e integrar visualmente as configurações do ASP.NET Core Identity;
- preservar validações e controles de segurança no backend;
- não realizar exclusão física de clientes, produtos ou lotes.

## 3. Princípios de interface

### 3.1 Hierarquia

Cada página terá:

1. contexto do ambiente e estado operacional no cabeçalho;
2. título, descrição curta e ação principal;
3. filtros e resumo dos resultados;
4. conteúdo principal;
5. paginação ou estado vazio;
6. feedback claro após ações.

### 3.2 Sistema visual

Os componentes usarão os tokens já definidos para o SGQ:

- verde estrutural: `#123D2B`;
- verde secundário: `#1A673B`;
- acento: `#98DF00`;
- fundo: `#F4F7F3`;
- texto: `#163D2C`;
- cores semânticas independentes para sucesso, atenção, atraso e erro.

Os estados não dependerão apenas de cor. Ícone, texto e contraste devem manter
o entendimento por teclado, leitor de tela e pessoas com baixa visão.

### 3.3 Componentes compartilhados

Serão criados componentes de apresentação reutilizáveis para:

- cabeçalho de página;
- barra de busca e filtros;
- seletor de quantidade por página;
- resumo de resultados;
- tabela responsiva;
- badge de situação;
- menu de ações por registro;
- paginação;
- confirmação de ação crítica;
- estado vazio;
- mensagens de sucesso, atenção e erro.

## 4. Dashboard

O dashboard será o centro de controle após a autenticação. Ele deverá apresentar:

- saudação e identificação do usuário autenticado;
- ação principal para nova reclamação;
- totais reais de reclamações abertas e não conformidades abertas;
- totais de clientes e produtos **ativos**;
- quantidade de lotes ativos como informação complementar;
- últimas reclamações, com estado vazio quando não houver registros;
- atalhos para novo cliente, produto e lote;
- resumo de pendências somente quando existirem dados confiáveis para calculá-las.

Nenhum número demonstrativo será exibido. Recursos ainda não implementados serão
marcados como indisponíveis ou omitidos, sem simular operação.

## 5. Cadastros básicos

### 5.1 Padrão das listagens

Clientes, produtos e lotes seguirão o mesmo padrão:

- busca textual submetida ao servidor;
- filtro de situação com `Ativos`, `Inativos` e `Todos`;
- ordenação estável adequada a cada cadastro;
- paginação no servidor;
- 10 registros por página como padrão;
- opções de 10, 25 e 50 registros por página;
- contador no formato “Exibindo X–Y de Z registros”;
- preservação dos filtros ao navegar entre páginas;
- ação principal de cadastro visível no cabeçalho;
- ações de editar, inativar e reativar conforme a situação;
- estado vazio específico para base vazia e para filtro sem resultado.

A busca será normalizada com remoção de espaços nas extremidades e limite de
100 caracteres. Página ou tamanho inválido serão normalizados para valores
seguros. A ordenação sempre terá um segundo critério por identificador para
evitar repetição ou salto de itens entre páginas.

### 5.2 Regras de inativação

Cada registro terá:

- indicador `Ativo`, verdadeiro por padrão;
- data e hora UTC da inativação;
- identificador do usuário responsável pela inativação;
- reversão por reativação, também auditável quando a infraestrutura existir.

A inativação e a reativação serão ações `POST`, protegidas por antiforgery e
autorização no backend. A interface solicitará confirmação antes da inativação.
Não haverá exclusão física.

Registros inativos:

- continuam visíveis em históricos e processos existentes;
- aparecem nas listagens quando o filtro permitir;
- não podem ser escolhidos em novos processos;
- podem ser reativados por usuário autorizado.

Ao inativar um produto, seus lotes não serão alterados em cascata. Contudo,
esses lotes também ficarão indisponíveis para novos processos enquanto o produto
estiver inativo. Essa regra evita modificar silenciosamente o estado do lote.

### 5.3 Persistência e consultas

Os parâmetros de busca, situação, página e tamanho chegarão ao controller, serão
validados e enviados a uma consulta EF Core. A consulta aplicará filtros antes de
contar e paginar os resultados e retornará um ViewModel próprio para a tela.

Índices iniciais apoiarão o filtro e a ordenação mais usados:

- cliente por situação e nome;
- produto por situação e nome;
- lote por situação e número.

A busca parcial poderá exigir otimização específica do PostgreSQL quando houver
volume real. `pg_trgm` não será introduzido sem medição que demonstre necessidade.

## 6. Formulários e detalhes

Os formulários de criar e editar terão:

- títulos e descrições coerentes com a tarefa;
- agrupamento visual por assunto;
- labels permanentes, ajuda curta e indicação de obrigatoriedade;
- resumo de validação e mensagens próximas ao campo;
- ações primária e secundária consistentes;
- proteção contra envio duplicado no navegador;
- ViewModels de entrada para evitar binding direto de entidades.

As telas de detalhe de RC e NC adotarão progressivamente o mesmo cabeçalho,
badges, painéis de informação e histórico visual, sem alterar nesta entrega as
transições de negócio desses módulos.

## 7. Configurações da conta

As páginas necessárias do ASP.NET Core Identity serão sobrescritas localmente,
preservando seus handlers e controles de segurança. O menu terá:

- Perfil;
- E-mail;
- Senha;
- Autenticação em duas etapas;
- Dados pessoais.

Todo o conteúdo visível será apresentado em português e integrado ao layout do
SGQ. Mensagens de sucesso e erro seguirão o sistema visual compartilhado.

A autenticação em duas etapas continuará disponível. A obrigatoriedade por
perfil pertence à etapa de segurança e não será simulada nesta entrega.

## 8. Acessibilidade e responsividade

- navegação completa por teclado;
- foco visível em todos os controles;
- labels e nomes acessíveis em ações de ícone;
- associação entre mensagens de erro e campos;
- contraste mínimo compatível com WCAG 2.2 AA;
- tabelas adaptadas para telas estreitas sem perder contexto;
- respeito a `prefers-reduced-motion`;
- confirmação e feedback anunciáveis por tecnologias assistivas.

## 9. Segurança

- nenhuma autorização dependerá de ocultar botões;
- ações mutáveis usarão `POST` e antiforgery;
- parâmetros de consulta serão limitados e validados;
- mensagens não revelarão detalhes internos ou dados sensíveis;
- não serão registrados segredos ou conteúdo pessoal desnecessário;
- páginas de conta manterão as proteções nativas do Identity;
- perfis e políticas granulares serão tratados na etapa de autenticação e
  segurança, sem criar uma autorização provisória enganosa.

## 10. Testes e critérios de aceite

### 10.1 Listagens

- busca encontra e restringe os registros esperados;
- filtro padrão mostra somente ativos;
- filtros de inativos e todos funcionam;
- paginação não repete nem omite registros;
- tamanhos 10, 25 e 50 funcionam;
- filtros são preservados entre páginas;
- parâmetros inválidos são normalizados com segurança;
- estados vazios distinguem base vazia de busca sem resultado.

### 10.2 Inativação

- inativar e reativar exigem usuário autenticado e antiforgery válido;
- não existe exclusão física pela interface;
- registro inativo permanece em históricos;
- registro inativo não aparece em novas seleções;
- lotes de produto inativo não aparecem em novas seleções;
- a alteração grava data e responsável;
- acesso direto por URL não contorna a autorização disponível.

### 10.3 Dashboard e conta

- dashboard calcula somente dados reais e cadastros ativos;
- estado vazio é exibido quando não existem reclamações;
- todas as páginas de conta previstas estão em português;
- alteração de perfil, e-mail e senha mantém o comportamento seguro do Identity;
- fluxos principais são verificados em desktop e viewport móvel;
- navegação por teclado e contraste passam pela revisão de acessibilidade.

## 11. Entregas recomendadas

Para manter os Pull Requests pequenos e revisáveis, a implementação será dividida:

1. componentes visuais compartilhados e refinamento do dashboard;
2. infraestrutura de consulta e paginação dos cadastros;
3. migration e regras de inativação/reativação;
4. aplicação do padrão a clientes, produtos e lotes;
5. páginas de configurações da conta em português;
6. testes automatizados e roteiro visual consolidado.

Cada fatia deverá possuir uma daily própria, critérios verificáveis e um único
objetivo de commit/PR.

## 12. Limites e validações pendentes

Os requisitos `CAD-004` e `CAD-005` permanecem `Proposto` na documentação atual.
Antes de implementar a inativação e definir quais perfis podem executá-la, é
necessária a validação conjunta do revisor técnico e da responsável da Qualidade,
seguida da atualização da matriz de rastreabilidade.

Os campos definitivos de clientes, produtos e lotes também não serão ampliados
sem validação dos requisitos `CAD-001`, `CAD-002` e `CAD-003`. Esta especificação
define a experiência e o ciclo de vida dos registros, mas não inventa campos de
negócio.
