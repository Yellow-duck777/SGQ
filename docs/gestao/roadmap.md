# Roadmap do SGQ

O roadmap define ordem e critérios de saída. Datas são registradas nas dailies e PRs, não prometidas neste documento.

## Onda 1 — Governança documental

- documentação organizada;
- tutorial de commit e regras para IA;
- matriz inicial de rastreabilidade;
- estratégia de testes e revisão de segurança.

Saída: documentos coerentes, links válidos e aprovação do revisor técnico.

## Onda 2 — Fundação técnica

- projetos Domain, Application, Infrastructure e Web;
- projetos de testes e CI;
- PostgreSQL descartável via Testcontainers;
- migrations consolidadas e banco documentado.

Saída: build e testes verdes, fronteiras arquiteturais verificadas.

## Onda 3 — Identidade e segurança

- cadastro e confirmação por SMTP;
- aprovação administrativa e múltiplos perfis;
- MFA para perfis críticos;
- políticas, auditoria, rate limiting e headers.

Saída: cenários positivos e negativos automatizados, sem acesso operacional indevido.

## Onda 4 — Design system e navegação

- paleta industrial refinada;
- login institucional;
- shell autenticado com sidebar;
- acessibilidade WCAG 2.2 AA.

Saída: interface responsiva, navegável por teclado e filtrada por permissão.

## Onda 5 — Cadastros

- requisitos de campos validados;
- clientes, produtos e lotes com inativação;
- pesquisa, paginação, concorrência e auditoria.

## Onda 6 — Reclamação de Cliente

- ciclo completo da RC;
- prazos e transições;
- geração atômica da NC após validação GQ.

## Onda 7 — Não Conformidade

- contenção, investigação, causa, ações e eficácia;
- aprovações segregadas;
- encerramento e reabertura auditados.

## Onda 8 — Recall

- avaliação, aprovação, recolhimento, retorno e destinação;
- comunicações e evidências;
- encerramento e reabertura.

## Onda 9 — Serviços e informação gerencial

- anexos no PostgreSQL e ClamAV;
- calendário, dias úteis e notificações;
- dashboard, relatórios, PDF e XLSX.

## Onda 10 — Produção

- Docker Linux, PostgreSQL, proxy HTTPS e ClamAV;
- homologação separada;
- health checks, logs e backups;
- ensaio de restauração e checklist de publicação.
