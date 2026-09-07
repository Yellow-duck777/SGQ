# Requisitos do SGQ

## 1. Objetivo

Este documento registra os requisitos funcionais,
regras de negócio, permissões, responsabilidades,
restrições e pontos pendentes necessários para o
desenvolvimento do Sistema de Gestão da Qualidade - SGQ.

O documento deverá ser atualizado durante o levantamento
de requisitos e sempre que uma nova regra de negócio
for formalmente definida.

---

# 2. Escopo inicial do sistema

O SGQ terá inicialmente três processos principais:

1. Reclamação de Cliente - RC;
2. Não Conformidade - NC;
3. Recolhimento / Recall de Produto.

Os processos deverão possuir relacionamento e
rastreabilidade entre si.

O sistema deverá permitir acompanhar uma ocorrência
desde seu registro até sua conclusão.

---

# 3. Base documental

O desenvolvimento inicial considera:

- POP.GQ.02 - Reclamação do Cliente;
- POP.GQ.03 - Recall / Recolhimento de Produtos;
- POP.GQ.04 - Não Conformidade;
- RDC ANVISA nº 47/2013;
- demais documentos internos aplicáveis.

Quando houver divergência entre documentação anterior
e uma regra de negócio posteriormente confirmada para
o sistema, a divergência deverá ser registrada e
formalmente tratada.

---

# 4. Usuários e responsabilidades

## 4.1. Usuários do sistema

O sistema poderá possuir usuários relacionados às seguintes
funções ou áreas:

- Administrador do sistema;
- Garantia da Qualidade - GQ;
- Responsável Técnico - RT;
- Controle de Qualidade - CQ;
- Comercial / Atendimento;
- Produção;
- Expedição / Almoxarifado;
- Diretoria;
- QSMS;
- Gestores;
- Auditores;
- Colaboradores autorizados.

A relação definitiva de perfis será definida durante
a modelagem de permissões.

---

## 4.2. Criação de registros

Poderão criar registros:

- gestores;
- auditores;
- colaboradores autorizados.

O sistema deverá identificar automaticamente o usuário
responsável pela abertura do registro.

---

## 4.3. Edição de registros

Poderão editar um registro:

- o usuário responsável pela abertura;
- Garantia da Qualidade - GQ.

Essa regra poderá sofrer restrições de acordo com o
status do processo.

As regras específicas de edição após aprovação ainda
precisam ser detalhadas.

---

## 4.4. Aprovações

Os seguintes perfis poderão participar de aprovações:

- Responsável Técnico - RT;
- Controle de Qualidade - CQ;
- Garantia da Qualidade - GQ.

A etapa exata em que cada perfil deverá aprovar ainda
deverá ser detalhada para cada processo.

---

## 4.5. Encerramento

O encerramento dos processos será realizado pela
Garantia da Qualidade - GQ.

O sistema não deverá permitir encerramento quando
existirem requisitos obrigatórios pendentes.

---

# 5. Requisitos gerais do sistema

## RF-001 - Autenticação

Cada usuário deverá possuir acesso individual ao sistema.

O acesso deverá ser realizado por credenciais individuais.

---

## RF-002 - Identificação do usuário

Toda operação relevante deverá ser associada ao usuário
que a realizou.

---

## RF-003 - Controle de acesso

O sistema deverá controlar quais informações e ações
cada usuário poderá acessar.

---

## RF-004 - Autorização

O sistema deverá validar as permissões também no backend.

A simples ocultação de um botão na interface não deverá
ser considerada controle de segurança suficiente.

---

## RF-005 - Histórico de alterações

O sistema deverá manter histórico das operações importantes.

No mínimo:

- criação;
- alteração;
- classificação;
- aprovação;
- reprovação;
- encerramento;
- reabertura;
- prorrogação;
- alteração de prazo.

---

## RF-006 - Dados do histórico

Cada evento do histórico deverá registrar, quando aplicável:

- usuário;
- data;
- hora;
- ação executada;
- registro afetado;
- informação anterior;
- nova informação;
- justificativa.

---

## RF-007 - Exclusão

Registros de qualidade não deverão ser excluídos
definitivamente pelo usuário comum.

---

## RF-008 - Inativação

Quando houver necessidade de retirada lógica de algum
cadastro, deverá ser avaliada a utilização de inativação.

---

## RF-009 - Anexos

O sistema deverá permitir anexar documentos e evidências.

Exemplos:

- fotografias;
- vídeos;
- notas fiscais;
- e-mails;
- laudos;
- relatórios;
- formulários;
- evidências das ações;
- documentos laboratoriais.

---

## RF-010 - Rastreabilidade

O sistema deverá permitir localizar informações utilizando,
quando aplicável:

- código da ocorrência;
- cliente;
- produto;
- lote;
- período;
- classificação;
- status;
- responsável;
- área.

---

## RF-011 - Pesquisa

O sistema deverá possuir mecanismo de pesquisa.

---

## RF-012 - Filtros

As listagens deverão permitir filtros adequados ao processo.

---

## RF-013 - Data e hora

Eventos relevantes deverão registrar data e hora.

---

## RF-014 - Notificações

O sistema deverá possuir mecanismo de notificações.

Os principais usuários considerados são:

- RT;
- CQ;
- GQ.

Cada evento que gera uma notificação deverá ser
definido individualmente.

---

# 6. Cadastros básicos

## 6.1. Cadastro de clientes

Os clientes serão cadastrados manualmente no sistema.

O cadastro deverá possuir as informações necessárias
para utilização nas reclamações e processos relacionados.

---

## 6.2. Cadastro de produtos

Os produtos serão cadastrados manualmente no sistema.

---

## 6.3. Cadastro de lotes

Os lotes serão cadastrados manualmente no sistema.

Cada lote deverá estar relacionado ao respectivo produto.

---

## 6.4. Integrações futuras

Integração com ERP ou outro sistema poderá ser avaliada
em versão futura.

Não faz parte do MVP inicial.

---

# 7. Reclamação de Cliente - RC

## RF-RC-001 - Registro da reclamação

O sistema deverá permitir registrar uma Reclamação de Cliente.

---

## RF-RC-002 - Identificação da reclamação

Cada reclamação deverá possuir código único.

Formato definitivo ainda deverá ser definido.

Exemplo provisório:

RC-2026-000001

---

## RF-RC-003 - Data do recebimento

Deverá ser registrada a data do recebimento da reclamação.

---

## RF-RC-004 - Canal de recebimento

O sistema deverá permitir registrar o canal pelo qual
a reclamação foi recebida.

---

## RF-RC-005 - Responsável pelo registro

O sistema deverá identificar quem realizou o registro.

---

## RF-RC-006 - Cliente

A reclamação deverá possuir cliente relacionado.

---

## RF-RC-007 - Contato do cliente

Deverá ser registrado o contato do cliente.

---

## RF-RC-008 - Produto

Deverá ser identificado o produto reclamado.

---

## RF-RC-009 - Lote

Deverá ser identificado o lote envolvido.

---

## RF-RC-010 - Descrição

Deverá existir uma descrição detalhada do problema relatado.

---

## RF-RC-011 - Fabricação

A data de fabricação deverá ser registrada quando aplicável.

---

## RF-RC-012 - Validade

A validade deverá ser registrada quando aplicável.

---

## RF-RC-013 - Quantidade envolvida

Deverá ser possível informar a quantidade de produto envolvida.

---

## RF-RC-014 - Local de aquisição

Deverá ser possível registrar onde o produto foi adquirido.

---

## RF-RC-015 - Nota fiscal

Deverá ser possível registrar ou anexar a nota fiscal.

---

## RF-RC-016 - Produto disponível

Deverá existir indicação sobre existência de produto
disponível para avaliação ou coleta.

---

## RF-RC-017 - Quantidade disponível

Quando houver produto disponível, deverá ser possível
registrar:

- quantidade;
- volume.

---

## RF-RC-018 - Evidências

Deverá ser possível anexar evidências fornecidas pelo cliente.

---

## RF-RC-019 - Informações incompletas

Quando faltarem informações necessárias, a reclamação deverá
permanecer aguardando complementação.

---

## RF-RC-020 - Classificação

A reclamação deverá possuir uma classificação:

- Crítica;
- Maior;
- Menor.

---

## RF-RC-021 - Responsável pela classificação

Somente a Garantia da Qualidade poderá classificar
a Reclamação de Cliente.

---

## RF-RC-022 - Reclamações semelhantes

A Garantia da Qualidade deverá conseguir verificar
a existência de reclamações semelhantes.

---

## RF-RC-023 - Reclamações do mesmo lote

Deverá ser possível pesquisar ocorrências relacionadas
ao mesmo lote.

---

## RF-RC-024 - Bloqueio preventivo

Deverá ser possível registrar necessidade de bloqueio
preventivo.

---

## RF-RC-025 - Avaliação de risco

Deverá ser possível registrar avaliação de risco:

- sanitário;
- ambiental;
- segurança;
- regulatório;
- qualidade.

---

## RF-RC-026 - Investigação

O sistema deverá permitir registrar a investigação.

---

## RF-RC-027 - Registros avaliados

A investigação poderá considerar:

- ordem de produção;
- registros de fabricação;
- registros de envase;
- registros de rotulagem;
- controle de qualidade;
- certificados;
- histórico de manutenção;
- armazenamento;
- distribuição;
- rastreabilidade;
- reclamações anteriores.

---

## RF-RC-028 - Amostra

Deverá ser possível registrar análise de amostra.

---

## RF-RC-029 - Amostra de retenção

Quando aplicável, a amostra reclamada poderá ser comparada
com a amostra de retenção do lote.

---

## RF-RC-030 - Parâmetros de análise

Quando aplicável, poderão ser registrados:

- aspecto;
- cor;
- odor;
- pH;
- viscosidade;
- densidade;
- volume;
- integridade da embalagem;
- condição do rótulo;
- outros ensaios aplicáveis.

---

## RF-RC-031 - Laudos

Resultados e laudos laboratoriais deverão poder ser anexados
ao processo.

---

## RF-RC-032 - Resultado da reclamação

A reclamação deverá ser classificada ao final da investigação
como:

- Procedente;
- Improcedente.

---

## RF-RC-033 - Tratamento

O sistema deverá permitir registrar tratamento aplicado.

Exemplos:

- orientação ao cliente;
- substituição de produto;
- bloqueio preventivo;
- segregação;
- descarte;
- revisão de procedimento;
- treinamento;
- ação corretiva;
- ação preventiva.

---

## RF-RC-034 - Resposta ao cliente

O sistema deverá permitir registrar o retorno formal
realizado ao cliente.

---

## RF-RC-035 - Dados da resposta

A resposta poderá registrar:

- resultado da investigação;
- procedência ou improcedência;
- ações adotadas;
- orientações;
- tratamento comercial;
- data;
- responsável.

---

## RF-RC-036 - Prazo

A reclamação estará sujeita ao prazo de 15 dias úteis,
conforme as regras de negócio confirmadas neste documento.

---

## RF-RC-037 - Geração automática de NC

Toda Reclamação de Cliente validada pela Garantia da
Qualidade deverá gerar automaticamente uma Não Conformidade.

---

## RF-RC-038 - Vínculo com NC

A Reclamação deverá permitir acessar sua NC vinculada.

---

## RF-RC-039 - Encerramento

A Reclamação somente poderá ser encerrada após cumprimento
das condições obrigatórias definidas para o processo.

---

# 8. Não Conformidade - NC

## RF-NC-001 - Registro

O sistema deverá permitir registrar uma Não Conformidade.

---

## RF-NC-002 - Código

Cada NC deverá possuir código único.

Exemplo provisório:

NC-2026-000001

---

## RF-NC-003 - Origem

A NC poderá possuir origem em:

- Reclamação de Cliente;
- Auditoria;
- Inspeção;
- Monitoramento de processo;
- Controle de mudança;
- outros desvios autorizados.

---

## RF-NC-004 - Reclamação vinculada

Quando originada por Reclamação de Cliente, a NC deverá
manter vínculo obrigatório com a RC.

---

## RF-NC-005 - Data

A NC deverá registrar sua data de abertura.

---

## RF-NC-006 - Área

A NC deverá possuir área relacionada.

---

## RF-NC-007 - Produto

O produto deverá ser registrado quando aplicável.

---

## RF-NC-008 - Lote

O lote deverá ser registrado quando aplicável.

---

## RF-NC-009 - Descrição

A NC deverá possuir descrição detalhada.

---

## RF-NC-010 - Evidências

A NC deverá permitir anexar evidências.

---

## RF-NC-011 - Classificação

A NC deverá possuir uma das classificações:

- Crítica;
- Maior;
- Menor.

---

## RF-NC-012 - Responsável pela classificação

Somente a Garantia da Qualidade poderá definir ou alterar
a classificação da NC.

---

## RF-NC-013 - NC crítica

Uma NC Crítica poderá envolver:

- risco à saúde;
- risco à segurança;
- risco ao meio ambiente;
- impacto regulatório;
- produto fora de especificação.

---

## RF-NC-014 - NC maior

Uma NC Maior poderá envolver:

- impacto significativo na qualidade;
- falha sistêmica;
- falha relevante de processo.

---

## RF-NC-015 - NC menor

Uma NC Menor será um desvio pontual sem impacto
imediato relevante.

---

## RF-NC-016 - Contenção

O sistema deverá permitir registrar ação imediata.

---

## RF-NC-017 - Tipos de contenção

Exemplos:

- bloqueio de produto;
- interrupção do processo;
- isolamento de área;
- outra ação de contenção.

---

## RF-NC-018 - Investigação

A NC deverá possuir investigação.

---

## RF-NC-019 - Causa provável

Deverá ser possível registrar causa provável.

---

## RF-NC-020 - Causa raiz

Deverá ser possível registrar causa raiz.

---

## RF-NC-021 - Método de análise

Deverá ser possível registrar o método utilizado.

Exemplos:

- 5 Porquês;
- Diagrama de Ishikawa;
- outro método aprovado.

---

## RF-NC-022 - Plano de ação

Uma NC poderá possuir uma ou mais ações.

---

## RF-NC-023 - Dados da ação

Cada ação deverá possuir, quando aplicável:

- descrição;
- responsável;
- prazo;
- status;
- data de execução;
- evidência.

---

## RF-NC-024 - Ação corretiva

O sistema deverá permitir registrar ação corretiva.

---

## RF-NC-025 - Ação preventiva

O sistema deverá permitir registrar ação preventiva.

---

## RF-NC-026 - Revisão documental

Quando necessário, deverá ser possível registrar necessidade
de revisão de:

- POP;
- instrução de trabalho;
- especificação;
- formulário;
- outro documento.

---

## RF-NC-027 - Treinamento

Quando uma alteração documental exigir treinamento,
essa necessidade deverá poder ser registrada.

---

## RF-NC-028 - Avaliação de eficácia

Após implementação das ações deverá existir avaliação
de eficácia.

---

## RF-NC-029 - Resultado da eficácia

A eficácia deverá possuir resultado:

- Eficaz;
- Ineficaz.

---

## RF-NC-030 - Ineficácia

Quando a ação for considerada ineficaz, deverá existir
possibilidade de novo tratamento ou reabertura conforme
regra ainda a ser detalhada.

---

## RF-NC-031 - Encerramento

A NC somente poderá ser encerrada quando os requisitos
obrigatórios estiverem concluídos.

---

## RF-NC-032 - Indicadores

O sistema deverá possibilitar futuramente indicadores como:

- quantidade de NC por período;
- tempo médio de fechamento;
- reincidência;
- NC por área.

---

# 9. Recall / Recolhimento

## RF-RECALL-001 - Registro

O sistema deverá permitir criar processo de Recall /
Recolhimento quando aplicável.

---

## RF-RECALL-002 - Código

Cada Recall deverá possuir código único.

Formato definitivo ainda pendente.

---

## RF-RECALL-003 - Origem

A necessidade de recolhimento poderá ser identificada por:

- Reclamação de Cliente;
- resultado analítico fora de especificação;
- desvio de produção;
- erro de rotulagem;
- falha de embalagem;
- Não Conformidade;
- determinação de autoridade sanitária;
- avaliação técnica.

---

## RF-RECALL-004 - Vínculo com NC

Quando relacionado a uma NC, o Recall deverá manter
rastreabilidade com ela.

---

## RF-RECALL-005 - Vínculo com Reclamação

Quando aplicável, deverá ser possível acessar a Reclamação
relacionada.

---

## RF-RECALL-006 - Produto

Deverá ser registrado o produto envolvido.

---

## RF-RECALL-007 - Lote

Deverá ser registrado o lote envolvido.

---

## RF-RECALL-008 - Fabricação

Deverá ser possível registrar data de fabricação.

---

## RF-RECALL-009 - Validade

Deverá ser possível registrar validade.

---

## RF-RECALL-010 - Quantidade produzida

Deverá ser registrada a quantidade produzida.

---

## RF-RECALL-011 - Quantidade em estoque

Deverá ser registrada a quantidade existente em estoque.

---

## RF-RECALL-012 - Quantidade distribuída

Deverá ser registrada a quantidade distribuída.

---

## RF-RECALL-013 - Clientes envolvidos

Deverão ser identificados os clientes que receberam
o produto envolvido.

---

## RF-RECALL-014 - Natureza da ocorrência

Deverá ser registrada a natureza da não conformidade
ou problema.

---

## RF-RECALL-015 - Risco

Deverá ser registrado o risco potencial associado.

---

## RF-RECALL-016 - Decisão de recolhimento

O sistema deverá registrar a decisão sobre recolhimento.

---

## RF-RECALL-017 - Justificativa

A decisão deverá possuir justificativa técnica quando
aplicável.

---

## RF-RECALL-018 - Bloqueio

Após decisão de recolhimento deverá ser possível registrar
bloqueio preventivo dos produtos existentes na empresa.

---

## RF-RECALL-019 - Comunicação aos clientes

Deverá ser possível registrar comunicação formal aos clientes.

---

## RF-RECALL-020 - Dados da comunicação

Quando aplicável, a comunicação deverá contemplar:

- produto;
- lote;
- motivo;
- orientação para interrupção do uso;
- orientação para segregação;
- forma de devolução;
- contato da empresa.

---

## RF-RECALL-021 - Autoridade sanitária

Quando aplicável, deverá ser possível registrar comunicação
à autoridade sanitária.

---

## RF-RECALL-022 - Protocolo

Quando houver comunicação regulatória, deverá ser possível
registrar protocolo e documentos relacionados.

---

## RF-RECALL-023 - Retorno de produto

O sistema deverá permitir registrar cada retorno de produto.

---

## RF-RECALL-024 - Cliente do retorno

Cada retorno deverá identificar o cliente.

---

## RF-RECALL-025 - Produto do retorno

Cada retorno deverá identificar o produto.

---

## RF-RECALL-026 - Lote do retorno

Cada retorno deverá identificar o lote.

---

## RF-RECALL-027 - Quantidade retornada

Deverá ser registrada a quantidade recebida.

---

## RF-RECALL-028 - Data do retorno

Deverá ser registrada a data do retorno.

---

## RF-RECALL-029 - Embalagem

Deverá ser registrada a condição da embalagem.

---

## RF-RECALL-030 - Lacre

Deverá ser possível registrar condição do lacre,
tampa ou fechamento.

---

## RF-RECALL-031 - Avarias

Deverá ser possível registrar:

- vazamento;
- avaria;
- violação;
- outras condições relevantes.

---

## RF-RECALL-032 - Documento de transporte

Deverá ser possível registrar nota fiscal ou documento
de transporte quando aplicável.

---

## RF-RECALL-033 - Evidências

Deverá ser possível anexar fotografias e demais evidências.

---

## RF-RECALL-034 - Segregação

O produto devolvido ou recolhido deverá permanecer
segregado enquanto aguardar avaliação.

---

## RF-RECALL-035 - Avaliação da GQ

A Garantia da Qualidade deverá registrar a avaliação
do produto devolvido.

---

## RF-RECALL-036 - Classificação do produto devolvido

O produto poderá ser classificado como:

- Aprovado para Retorno ao Estoque;
- Reprovado;
- Aguardando Investigação.

---

## RF-RECALL-037 - Destinação

O sistema deverá permitir registrar a destinação.

Possibilidades previstas:

- retorno ao estoque;
- retrabalho;
- correção de rotulagem;
- descarte;
- outra destinação autorizada.

---

## RF-RECALL-038 - Evidência de destinação

A destinação deverá possuir evidência quando aplicável.

---

## RF-RECALL-039 - Encerramento

O Recall somente poderá ser encerrado quando todos os
requisitos obrigatórios estiverem atendidos.

---

# 10. Regras de Negócio Confirmadas

## RN-001 - Geração automática de Não Conformidade

Toda Reclamação de Cliente validada pela Garantia da
Qualidade deverá gerar automaticamente uma Não Conformidade
vinculada.

A NC deverá possuir identificação própria.

A criação da NC somente ocorrerá depois da validação da
Reclamação pela GQ.

---

## RN-002 - Classificações utilizadas

No MVP, Reclamações e Não Conformidades utilizarão somente:

- Crítica;
- Maior;
- Menor.

---

## RN-003 - Critério de classificação Crítica

Será considerada Crítica uma ocorrência que possa envolver:

- risco à saúde;
- risco à segurança;
- risco ao meio ambiente;
- impacto regulatório;
- produto fora de especificação.

---

## RN-004 - Critério de classificação Maior

Será considerada Maior uma ocorrência que possa representar:

- impacto significativo na qualidade;
- falha relevante;
- falha sistêmica.

---

## RN-005 - Critério de classificação Menor

Será considerada Menor uma ocorrência pontual sem impacto
imediato relevante.

---

## RN-006 - Responsável pela classificação

Somente a Garantia da Qualidade poderá definir ou alterar
a classificação de Reclamação ou Não Conformidade.

---

## RN-007 - Retenção

Os registros do SGQ deverão permanecer armazenados pelo
período mínimo de 2 anos.

---

## RN-008 - Rastreabilidade RC x NC

A NC gerada a partir de uma Reclamação deverá manter vínculo
permanente com a Reclamação de origem.

A Reclamação deverá mostrar sua NC.

A NC deverá mostrar sua Reclamação de origem.

---

## RN-009 - Prazo da Reclamação

O prazo para conclusão do processo de Reclamação será de
15 dias úteis.

---

## RN-010 - Informações completas

O prazo somente começará após o recebimento de todas as
informações necessárias para análise.

Enquanto houver informação obrigatória pendente, o prazo
não deverá ser iniciado.

---

## RN-011 - Início após 24 horas

Após todas as informações necessárias estarem completas,
a contagem dos 15 dias úteis começará 24 horas depois.

---

## RN-012 - Laboratório externo

Quando a investigação depender de laboratório externo,
a conclusão do processo ficará condicionada ao resultado
emitido pelo laboratório.

---

## RN-013 - Atraso externo

Quando o laboratório externo atrasar a emissão do resultado,
o atraso deverá ser registrado como atraso externo.

Esse período deverá ser diferenciado de atraso interno
da empresa nos indicadores.

---

## RN-014 - Prorrogação

Somente:

- Garantia da Qualidade - GQ;
- Responsável Técnico - RT;

poderão autorizar ou registrar prorrogação.

---

## RN-015 - Justificativa da prorrogação

Toda prorrogação deverá possuir motivo registrado no sistema.

---

## RN-016 - Comunicação ao cliente

Quando houver necessidade de prorrogação ou impossibilidade
de conclusão dentro do prazo previsto, o cliente deverá
ser comunicado.

---

## RN-017 - Registro da comunicação

A comunicação ao cliente deverá registrar, quando aplicável:

- data;
- responsável;
- motivo;
- meio utilizado.

---

## RN-018 - Alerta de vencimento

O sistema deverá enviar alerta por e-mail antes do vencimento.

---

## RN-019 - Antecedência do alerta

O alerta deverá ser enviado com antecedência mínima de
48 horas úteis.

---

## RN-020 - Destinatários do alerta de prazo

O alerta de prazo será enviado para:

- Garantia da Qualidade - GQ;
- Responsável Técnico - RT.

---

## RN-021 - Cálculo das 48 horas

O cálculo deverá considerar horas em dias úteis.

Sábados, domingos e demais dias considerados não úteis
não entrarão nessa contagem.

---

## RN-022 - Abrangência dos 15 dias

O prazo de 15 dias úteis será destinado à conclusão de
todo o processo de Reclamação.

Inclui:

- investigação;
- avaliação;
- conclusão;
- ações aplicáveis;
- resposta ao cliente;
- encerramento.

---

## RN-023 - Processo atrasado

Quando o prazo for ultrapassado sem conclusão, o registro
deverá ser identificado como:

Atrasado.

---

## RN-024 - Atraso não bloqueia o processo

Um processo marcado como Atrasado continuará podendo
ser tratado.

O atraso não bloqueará automaticamente as ações necessárias
para conclusão.

---

# 11. Segurança e rastreabilidade

## RS-001

O sistema deverá possuir login e senha individual.

## RS-002

Senhas não deverão ser armazenadas em texto puro.

## RS-003

O sistema deverá possuir controle de autorização.

## RS-004

Ações críticas deverão ser registradas em histórico.

## RS-005

Anexos deverão ser protegidos contra acesso não autorizado.

## RS-006

Credenciais, senhas, tokens e chaves não deverão ser
armazenados no código-fonte.

## RS-007

Informações confidenciais não deverão ser expostas em
repositório público.

---

# 12. Relatórios e indicadores

O sistema deverá permitir evolução futura para relatórios
e indicadores.

Possibilidades já identificadas:

- quantidade de reclamações por período;
- reclamações por produto;
- reclamações por lote;
- reclamações por classificação;
- reclamações procedentes;
- reclamações improcedentes;
- processos atrasados;
- atrasos externos;
- quantidade de NC;
- NC por área;
- NC por classificação;
- tempo médio de encerramento;
- reincidência;
- quantidade de Recall;
- quantidades recolhidas.

A relação definitiva deverá ser confirmada.

---

# 13. Pontos Pendentes

Os itens abaixo ainda precisam ser definidos antes da
modelagem definitiva do banco e dos fluxos.

## P-001 - Status da Reclamação

Definir os possíveis status.

Exemplos apenas para discussão:

- Rascunho;
- Aguardando informações;
- Em avaliação;
- Em investigação;
- Aguardando laboratório;
- Aguardando resposta;
- Encerrada;
- Atrasada.

Não implementar esses status sem confirmação.

---

## P-002 - Status da NC

Definir os possíveis status da Não Conformidade.

---

## P-003 - Status do Recall

Definir os possíveis status do processo de Recall.

---

## P-004 - Reabertura

Confirmar se processos encerrados poderão ser reabertos.

---

## P-005 - Responsável pela reabertura

Caso seja permitida reabertura, definir quem poderá realizá-la.

---

## P-006 - Justificativa de reabertura

Confirmar se justificativa será obrigatória.

---

## P-007 - Numeração da Reclamação

Definir padrão definitivo.

Exemplo:

RC-2026-000001

---

## P-008 - Numeração da NC

Definir padrão definitivo.

Exemplo:

NC-2026-000001

---

## P-009 - Numeração do Recall

Definir padrão definitivo.

---

## P-010 - Mais de um produto

Confirmar se uma Reclamação poderá envolver mais de
um produto.

---

## P-011 - Mais de um lote

Confirmar se uma Reclamação poderá envolver mais de um lote.

---

## P-012 - Formatos de anexos

Definir quais extensões serão permitidas.

---

## P-013 - Tamanho dos anexos

Definir tamanho máximo por arquivo.

---

## P-014 - Quantidade de anexos

Definir se haverá limite de anexos por processo.

---

## P-015 - Edição após aprovação

Definir se um registro aprovado poderá ser alterado.

---

## P-016 - Alteração após aprovação

Caso possa ser alterado, definir se nova aprovação
será obrigatória.

---

## P-017 - Aprovação da Reclamação

Definir exatamente quais etapas exigirão:

- GQ;
- CQ;
- RT.

---

## P-018 - Aprovação da NC

Definir exatamente quais etapas exigirão aprovação.

---

## P-019 - Aprovação do Recall

Definir exatamente quem deverá aprovar o início do Recall.

---

## P-020 - Recall obrigatório

Definir situações em que Recall será obrigatório.

---

## P-021 - Recall facultativo

Definir situações em que Recall ficará sujeito à
avaliação técnica.

---

## P-022 - Recall não aplicável

Definir como deverá ser registrada a decisão de
não realizar Recall.

---

## P-023 - Justificativa de não Recall

Confirmar se será obrigatória justificativa técnica.

---

## P-024 - Comunicação regulatória

Definir exatamente quando deverá haver comunicação
à autoridade sanitária.

---

## P-025 - Notificações

Definir todos os eventos que deverão gerar notificação.

---

## P-026 - Notificação de nova Reclamação

Confirmar quem deverá receber.

---

## P-027 - Notificação de NC crítica

Confirmar quem deverá receber imediatamente.

---

## P-028 - Notificação de Recall

Confirmar todos os destinatários.

---

## P-029 - Notificação de ação vencida

Confirmar se responsáveis deverão receber alertas.

---

## P-030 - Feriados

Definir como o sistema reconhecerá feriados para cálculo
de dias úteis.

---

## P-031 - Calendário

Definir se serão considerados:

- feriados nacionais;
- estaduais;
- municipais.

---

## P-032 - Prazo da NC

O POP atual apresenta prazos recomendados conforme
classificação.

Confirmar como esses prazos serão implementados no sistema.

---

## P-033 - Prazo do Recall

Definir se Recall possuirá prazo próprio.

---

## P-034 - Laboratório externo

Definir se o laudo do laboratório externo será obrigatório
como anexo antes do encerramento.

---

## P-035 - Amostra interna

Definir como será registrado o controle da amostra utilizada.

---

## P-036 - Indicadores

Definir quais indicadores obrigatoriamente estarão
no dashboard do MVP.

---

## P-037 - Relatórios

Definir quais relatórios serão obrigatórios no MVP.

---

## P-038 - Exportação

Definir necessidade de:

- PDF;
- Excel;
- impressão.

---

## P-039 - Dashboard

Definir informações que deverão aparecer ao entrar no sistema.

---

## P-040 - Tratamento comercial

Definir se o tratamento comercial será registrado dentro
do SGQ ou somente referenciado.

---

# 14. Decisões já encerradas

Os seguintes pontos não deverão permanecer como pendentes:

- Toda Reclamação validada pela GQ gera NC automaticamente;
- classificações do MVP: Crítica, Maior e Menor;
- somente GQ classifica RC e NC;
- retenção de registros: 2 anos;
- prazo da Reclamação: 15 dias úteis;
- prazo somente inicia após informações completas;
- início da contagem: 24 horas após informações completas;
- laboratório externo condiciona a conclusão;
- atraso de laboratório é atraso externo;
- GQ e RT podem tratar prorrogação;
- prorrogação exige motivo;
- cliente deve ser comunicado;
- alerta de prazo: 48 horas úteis;
- alerta de prazo para GQ e RT;
- vencimento marca processo como Atrasado;
- processo Atrasado não fica bloqueado.

---

# 15. Situação atual do levantamento

O sistema permanece em fase de levantamento de requisitos.

Ainda não iniciar modelagem definitiva do banco enquanto
os pontos de maior impacto estiverem pendentes.

# Próximas etapas

1. Concluir pendências de negócio;
2. Definir status dos processos;
3. Definir matriz de permissões;
4. Desenhar fluxos;
5. Definir MVP;
6. Modelar entidades e relacionamentos;
7. Modelar banco de dados;
8. Criar projeto ASP.NET Core.

# Status da Reclamação de Cliente

## ST-RC-001 - Rascunho

A Reclamação de Cliente poderá ser salva como **Rascunho** enquanto ainda estiver sendo preenchida.

Nesse status, o registro ainda não será considerado pronto para análise da Garantia da Qualidade.

---

## ST-RC-002 - Informações Pendentes

Quando faltarem informações necessárias para análise da reclamação, o processo ficará com o status:

**Informações Pendentes**

Enquanto estiver nesse status:

- o prazo de 15 dias úteis não será iniciado;
- deverão ser identificadas as informações faltantes;
- o processo aguardará a complementação dos dados.

---

## ST-RC-003 - Aguardando Validação da GQ

Quando todas as informações obrigatórias estiverem completas, a reclamação ficará com o status:

**Aguardando Validação da GQ**

A Garantia da Qualidade deverá analisar e validar as informações registradas.

---

## ST-RC-004 - Em Investigação

Após a validação pela Garantia da Qualidade:

- a reclamação passará para **Em Investigação**;
- será criada automaticamente uma Não Conformidade vinculada;
- o processo de investigação poderá ser iniciado.

---

## ST-RC-005 - Aguardando Laboratório Externo

Quando a investigação depender de resultado emitido por laboratório externo, a reclamação poderá assumir o status:

**Aguardando Laboratório Externo**

Nesse período, deverá ser aplicada a regra de atraso externo já definida no sistema.

Após o recebimento do resultado, o processo deverá retornar ao fluxo normal da investigação.

---

## ST-RC-006 - Aguardando Conclusão

Quando a investigação estiver concluída, mas ainda existirem atividades necessárias antes do encerramento, a reclamação ficará com o status:

**Aguardando Conclusão**

Poderão existir nessa etapa atividades como:

- registro da conclusão;
- definição da procedência ou improcedência;
- registro das ações aplicáveis;
- resposta formal ao cliente;
- aprovações necessárias.

---

## ST-RC-007 - Encerrada

Após o cumprimento de todos os requisitos obrigatórios, a reclamação poderá assumir o status:

**Encerrada**

O encerramento será realizado pela Garantia da Qualidade.

---

## RN-RC-STATUS-001 - Reclamações não poderão ser canceladas

Toda Reclamação de Cliente registrada deverá seguir o fluxo definido pelo SGQ até sua conclusão.

Não existirá status de Cancelada para Reclamação de Cliente.

Mesmo quando a reclamação for considerada improcedente, ela deverá concluir o processo de investigação, registro da conclusão e encerramento.

---

## Fluxo de status da Reclamação

Rascunho

→ Informações Pendentes, quando necessário

→ Aguardando Validação da GQ

→ Em Investigação

→ Aguardando Laboratório Externo, quando aplicável

→ Em Investigação

→ Aguardando Conclusão

→ Encerrada