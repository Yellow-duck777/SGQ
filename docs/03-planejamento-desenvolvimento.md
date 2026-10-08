# Planejamento de Desenvolvimento — SGQ

## 1. Finalidade deste documento

Este documento mantém o histórico do desenvolvimento do **Sistema de Gestão da Qualidade (SGQ)** e funciona como roteiro de continuidade. Ele permite que a equipe, o avaliador e futuros colaboradores identifiquem rapidamente:

- o objetivo do sistema;
- o que já foi implementado;
- em qual etapa o projeto se encontra;
- o que falta para o MVP;
- a ordem recomendada para as próximas entregas;
- os critérios usados para considerar cada entrega concluída.

Os requisitos de negócio detalhados estão em [requisitos consolidados](produto/requisitos-consolidados.md).

> Documento histórico de entregas. Em caso de divergência, prevalecem a [matriz de rastreabilidade](produto/matriz-rastreabilidade.md) e o [backlog](gestao/backlog.md).

---

## 2. Visão do produto

O SGQ é uma aplicação web para centralizar processos de qualidade, evidências, aprovações, ações e rastreabilidade. O MVP contempla três processos conectados:

1. **Reclamação de Cliente (RC)**;
2. **Não Conformidade (NC)**;
3. **Recall / Recolhimento de Produto**.

O princípio central é que uma RC validada pela Garantia da Qualidade (GQ) gera automaticamente uma NC vinculada. Um Recall pode ser aberto e vinculado a uma NC e/ou RC quando aplicável.

---

## 3. Situação atual do projeto

**Fase atual:** consolidação do MVP funcional.

**Última entrega consolidada em código:** fluxos principais de RC, NC e Recall, perfis, auditoria, anexos e indicadores básicos.

**Estado por requisito:** consulte a [matriz de rastreabilidade](produto/matriz-rastreabilidade.md), que é a fonte de verdade. Este documento preserva o histórico de entregas e o roteiro de continuidade.

**Verificações:** `dotnet restore`, `dotnet build --no-restore` e `dotnet test --no-build` (ver [estratégia de testes](qualidade/estrategia-de-testes.md)); a lista de testes está na [matriz de testes](qualidade/matriz-de-testes.md).

**Atenção antes de uso local:** as migrações criadas devem ser aplicadas ao banco PostgreSQL com `dotnet ef database update --project src/SGQ.Web --startup-project src/SGQ.Web`.

---

## 4. Arquitetura atual

| Camada | Situação | Descrição |
|---|---|---|
| Aplicação web | Implementada | ASP.NET Core MVC (.NET 10). |
| Autenticação | Implementada | ASP.NET Identity com login individual. |
| Banco de dados | Modelado | PostgreSQL via Entity Framework Core. |
| Migrações | Criadas | Migrações para os módulos desenvolvidos. |
| Interface | Implementada parcialmente | Layout, menu, dashboard, formulários e páginas de detalhes. |
| Armazenamento de anexos | Implementado (decisão atual) | Arquivos em `App_Data/uploads`, fora da pasta pública; ver [ADR 0002](arquitetura/decisoes/0002-anexos-postgresql.md). |

---

## 5. Entregas concluídas

### 5.1 Fundação e cadastros

- Autenticação de usuários.
- Cadastro de clientes.
- Cadastro de produtos.
- Cadastro de lotes vinculados a produtos.
- Restrição de exclusão de produto que possui lotes relacionados.
- Layout principal, navegação e dashboard inicial.

**Status:** concluído para o MVP.

### 5.2 Reclamação de Cliente

Implementado:

- Abertura de RC com número automático anual no formato `RC-AAAA-000001`.
- Cliente, contato, produto, lotes, canal, descrição, datas e quantidades.
- Validação com classificação Crítica, Maior ou Menor.
- Geração automática de NC vinculada ao validar a RC.
- Investigação, resultado Procedente/Improcedente, tratamento e resposta ao cliente.
- Encerramento formal.
- Registro de usuário e data de validação/encerramento.

**Status:** fluxo principal concluído; reabertura com justificativa e auditoria, laboratório externo, prorrogação e notificações foram implementados. A reabertura de RC é permitida a GQ e Administrador no código, enquanto o requisito prevê GQ ou RT (decisão pendente no backlog).

### 5.3 Não Conformidade

Implementado:

- Abertura manual com número automático `NC-AAAA-000001`.
- NC automática originada por RC validada.
- Registro de contenção, investigação, causa provável, causa raiz e método de análise.
- Plano de ação com responsável, prazo, obrigatoriedade, conclusão e evidência textual.
- Avaliação de eficácia: eficaz segue para aprovação; ineficaz retorna para investigação.
- Aprovação por RT e GQ.
- Reprovação retorna a NC para tratamento.
- Encerramento após aprovações necessárias.

**Status:** fluxo principal concluído; reabertura com justificativa, auditoria e nova rodada de aprovações foi implementada para NC. Notificações, laboratório externo e decisão de divergência pelo CQ foram implementados. Aprovações de RT e GQ exigem contas distintas e registram o usuário de cada parecer.

### 5.4 Recall

Implementado:

- Abertura com número automático `REC-AAAA-000001`.
- Origem, RC/NC vinculadas, produto, lote, quantidades, risco e decisão técnica.
- Aprovações RT e GQ.
- Registro de bloqueio, comunicação a clientes e autoridade sanitária.
- Registro de retornos de produto.
- Registro de destinação e evidência.
- Encerramento condicionado às aprovações, comunicação regulatória e destinação.

**Status:** fluxo principal concluído; reabertura com justificativa, auditoria e nova rodada de aprovações foi implementada para Recall. A decisão de divergência pelo CQ foi implementada; falta maior detalhamento regulatório.

### 5.5 Segurança e governança

Implementado:

- Papéis: Administrador, GQ, RT, CQ e Auditor.
- Administração de usuários e atribuição de perfis.
- Configuração de administrador inicial por `InitialAdminEmail`.
- Restrições de backend para ações críticas de GQ, RT e Administrador.
- Auditoria automática de criações, alterações e exclusões.

**Status:** base concluída. Contas sem perfil não acessam o sistema (política de fallback). Pendências: perfis por ação nas ações de escrita, MFA e hardening de produção (ver backlog).

### 5.6 Anexos e evidências

Implementado:

- Anexos em RC, NC e Recall.
- Tipos permitidos para o MVP e limite de 25 MB por arquivo.
- Metadados: nome, tipo, tamanho, descrição, usuário e data/hora.
- Download autenticado.
- Marcação de evidência crítica.
- Anulação lógica com justificativa, usuário e data/hora.

**Status:** concluído parcialmente; o vínculo múltiplo foi implementado. Faltam limite de 20 anexos por registro, validação de conteúdo, SHA-256 e antimalware.

### 5.7 Dashboard

Implementado:

- Quantidade de RC abertas.
- Quantidade de NC abertas.
- Quantidade de Recalls ativos.
- Quantidade de processos aguardando aprovação.
- Processos vencidos e próximos do vencimento.

**Status:** indicadores básicos concluídos; faltam indicadores de prazo, classificação, laboratório externo e relatórios.

---

## 6. Backlog restante do MVP

### Prioridade 1 — Prazos, calendário e notificações

**Objetivo:** controlar vencimentos e alertar responsáveis.

Entregas:

- Exibir e permitir preencher data-alvo nos formulários de RC, NC e Recall.
- Cadastro de calendário de dias não úteis (nacionais, estaduais, municipais e internos). **Concluído:** consulta para todos os usuários e criação/edição restrita a Administrador e GQ; alterações são auditadas.
- Cálculo de prazo de RC em dias úteis. **Concluído:** 15 dias úteis, contados após 24 horas das informações completas, ignorando fins de semana e dias não úteis ativos.
- Regras de prazo de NC por classificação. **Concluído:** Maior em 15 dias úteis, Menor em 30 dias úteis e Crítica com data-alvo obrigatoriamente definida pela GQ.
- Indicadores de vencidos e vencendo em breve. **Concluído:** o dashboard considera RC, NC e Recall abertos com data-alvo, usando a janela de 48 horas úteis.
- Alertas de 48 horas úteis antes do vencimento. **Concluído:** serviço em segundo plano executado a cada hora, com alerta único por prazo para GQ e RT e registro de envio.
- E-mails para os responsáveis definidos nos requisitos. **Concluído para alertas de prazo:** RC, NC e Recall com data-alvo; ações de NC vencidas notificam o responsável e GQ, incluindo RT quando a NC é Crítica. Notificações dos eventos do fluxo também foram implementadas.

Critério de aceite:

- Um processo com data-alvo vencida aparece como atrasado.
- Um processo próximo do prazo aparece no dashboard.
- O cálculo ignora fins de semana e datas cadastradas como não úteis.

### Prioridade 2 — Fluxos especiais e integridade

**Objetivo:** concluir as transições exigidas pelos requisitos regulatórios.

Entregas:

- Laboratório externo para RC e NC, com laudo crítico obrigatório quando utilizado para conclusão. **Concluído:** solicitação, envio da amostra, anexo do laudo, resultado e retorno à investigação.
- Reabertura de RC, NC e Recall com justificativa e auditoria. **Concluído:** Recall pode ser reaberto por GQ ou RT, preservando o número e o encerramento anterior, exigindo motivo e justificativa e reiniciando as aprovações. NC pode ser reaberta por GQ, RT ou Auditor, retorna à investigação e invalida as aprovações anteriores. RC pode ser reaberta por GQ ou Administrador, retorna à investigação e mantém a NC vinculada.
- Prorrogação de prazo com motivo, responsável e histórico. **Concluído:** RC, NC e Recall exigem motivo, preservam a data anterior e registram responsável e comunicação ao cliente.
- Divergência entre RT e GQ encaminhada ao CQ para decisão. **Concluído:** nas aprovações de NC e Recall, pareceres opostos encaminham o processo ao CQ; a decisão fundamentada fica rastreada e determina o prosseguimento ou retorno ao tratamento/avaliação.
- Controle de amostras internas, se confirmado como escopo imediato.

Critério de aceite:

- Não é possível encerrar processo que depende de laudo externo sem o documento correspondente.
- Reabertura preserva número, histórico e justifica a transição.
- Divergência não permite encerramento sem decisão do CQ.

### Prioridade 3 — Pesquisa, filtros e rastreabilidade

**Objetivo:** tornar os registros localizáveis e auditáveis no uso diário.

Entregas:

- Pesquisa por código, cliente, produto, lote e período. **Concluído para as listagens de RC, NC e Recall:** a busca cobre código e os relacionamentos aplicáveis de cada processo.
- Filtros por status, classificação, origem, área e responsável. **Concluído:** RC, NC e Recall possuem filtros por status, classificação/decisão, produto, lote (quando aplicável), responsável e período; NC também filtra por origem e a busca cobre a área. Em NC, o filtro de responsável considera tanto o usuário de abertura quanto o responsável por ações.
- Navegação entre RC, NC, Recall, lotes e anexos relacionados.
- Consulta de histórico de auditoria por processo. **Concluído:** RC, NC e Recall exibem as alterações, usuário e data/hora do registro.

Critério de aceite:

- Usuário encontra um processo pelo seu código, produto ou lote.
- Listagens preservam os filtros aplicados e mostram resultado coerente.

### Prioridade 4 — Relatórios e exportações

**Objetivo:** disponibilizar informação gerencial e evidência documental.

Entregas:

- Relatórios de RC por período, produto, lote e classificação.
- Relatórios de NC por área, origem e classificação.
- Relatórios de Recall por período, produto e lote.
- Indicadores de atrasos, atrasos externos e tempo médio de encerramento.
- Exportações XLSX e PDF.

Critério de aceite:

- Os números de relatórios coincidem com os filtros apresentados nas listagens.
- Exportações carregam cabeçalhos, período de referência e dados filtrados.

### Prioridade 5 — Evolução de anexos

**Objetivo:** evitar duplicação de evidências.

Entregas:

- Criar vínculos múltiplos para o mesmo anexo entre RC, NC e Recall. **Concluído em código:** o vínculo referencia o arquivo já armazenado, sem cópia física.
- Exibir todos os processos relacionados a uma evidência. **Concluído em código:** cada anexo mostra sua origem e todos os vínculos adicionais.
- Manter regras de acesso e anulação lógica centralizadas. **Concluído em código:** o vínculo e a anulação são restritos à GQ/Administrador.

Critério de aceite:

- Um único arquivo pode ser acessado por processos relacionados sem cópia física adicional.

---

## 7. Roteiro de execução recomendado

| Etapa | Entrega | Dependência | Status |
|---|---|---|---|
| 1 | Aplicar migrações e validar ambiente local | PostgreSQL configurado | Pendente |
| 2 | Datas-alvo na interface e calendário | Etapa 1 | Concluído em código (migração pendente de aplicação) |
| 3 | Alertas e notificações | Etapa 2 | Alertas de prazo e notificações de eventos concluídos em código |
| 4 | Laboratório, reabertura e prorrogação | Etapa 1 | Concluído em código (migrações pendentes de aplicação) |
| 5 | Divergência e decisão do CQ | Perfis ativos | Concluído em código (migração pendente de aplicação) |
| 6 | Pesquisa e filtros | Etapa 1 | Concluído em código |
| 7 | Relatórios e exportações | Pesquisa/filtros | Parcial: relatório único de processos com exportações CSV, XLSX e PDF; relatórios do MVP restantes no backlog |
| 8 | Vínculos múltiplos de anexos | Anexos atuais | Concluído em código (migração pendente de aplicação) |
| 9 | Testes ponta a ponta e revisão de segurança | Todas as anteriores | Em andamento: testes de fluxo com EF InMemory e testes arquiteturais (ver matriz de testes); faltam testes com PostgreSQL real e E2E |

---

## 8. Procedimento de continuidade

Ao retomar o desenvolvimento:

1. Ler a [matriz de rastreabilidade](produto/matriz-rastreabilidade.md), o [backlog](gestao/backlog.md) e a daily ativa.
2. Executar `git status` para identificar mudanças pendentes.
3. Executar `dotnet restore SGQ.slnx`, `dotnet build SGQ.slnx --no-restore` e `dotnet test SGQ.slnx --no-build` antes de iniciar uma nova entrega.
4. Selecionar uma única demanda do backlog com requisito `Validado`, `Implementado` ou `Verificado` e critérios de aceite.
5. Implementar modelo, banco, backend, interface e validação na mesma entrega.
6. Criar migração quando houver alteração de entidades.
7. Compilar e testar novamente e registrar a conclusão na daily e na matriz de rastreabilidade (este documento não é mais atualizado por entrega).
8. Criar commit seguindo [tutorial-commit.md](desenvolvimento/tutorial-commit.md).

---

## 9. Pendências de implantação

- Configurar `ConnectionStrings:DefaultConnection` com User Secrets ou variável de ambiente.
- Configurar `InitialAdminEmail` para provisionar o primeiro administrador (só vale enquanto não houver Administrador).
- Configurar SMTP por User Secrets ou variável de ambiente (`Smtp:Host`, `Smtp:Port`, `Smtp:EnableSsl`, `Smtp:UserName`, `Smtp:Password` e `Smtp:From`) antes de ativar os alertas por e-mail.
- Aplicar as migrações pendentes.
- Criar usuários de teste para Administrador, GQ, RT, CQ e usuário comum.
- Testar permissões e fluxos completos com esses usuários.

---

## 10. Definição de pronto para o MVP

O MVP estará pronto para validação quando:

- RC, NC e Recall puderem ser executados ponta a ponta por usuários com perfis adequados.
- As transições críticas estiverem protegidas no backend.
- Cada alteração relevante possuir histórico rastreável.
- Evidências puderem ser anexadas e protegidas.
- Prazos e alertas funcionarem conforme regras definidas.
- Pesquisa, filtros e relatórios básicos estiverem disponíveis.
- Todas as migrações estiverem aplicadas e o ambiente estiver validado com testes de fluxo.

---

## 11. Checklist obrigatório de pré-entrega

Executar este checklist imediatamente antes da entrega/implantação:

- [ ] Alterar a senha do usuário de banco da aplicação.
- [ ] Confirmar que a aplicação não utiliza o superusuário do PostgreSQL.
- [ ] Desativar `DevelopmentTestUsers`.
- [ ] Remover ou invalidar contas e senhas de teste.
- [ ] Criar os usuários reais.
- [ ] Verificar que nenhuma senha está no código-fonte ou em `appsettings.json`.
- [ ] Confirmar a existência e a recuperação possível de um backup do banco.
- [ ] Testar login e conexão com o banco após a troca de credenciais.
