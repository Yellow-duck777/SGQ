# Roadmap do SGQ

O roadmap define ordem e critérios de saída. Datas são registradas nas dailies e PRs, não prometidas neste documento. Estado em 2026-10-08. A execução real não seguiu a ordem das ondas: os módulos funcionais foram entregues antes da fundação de segurança e testes, e as ondas abaixo registram o que falta.

| Onda | Tema | Estado |
| --- | --- | --- |
| 1 | Governança documental | Concluída |
| 2 | Fundação técnica | Parcial |
| 3 | Identidade e segurança | Parcial |
| 4 | Design system e navegação | Concluída (acessibilidade WCAG a revisar) |
| 5 | Cadastros | Parcial |
| 6 | Reclamação de Cliente | Parcial |
| 7 | Não Conformidade | Concluída |
| 8 | Recall | Concluída |
| 9 | Serviços e informação gerencial | Parcial |
| 10 | Produção | Pendente |

## Onda 1 — Governança documental (Concluída)

- documentação organizada;
- tutorial de commit e regras para IA;
- matriz de rastreabilidade;
- estratégia de testes e revisão de segurança.

Saída: documentos coerentes, links válidos e aprovação do revisor técnico.

## Onda 2 — Fundação técnica (Parcial)

- projetos Domain, Application, Infrastructure e Web (feitos, mas `Application` e `Infrastructure` vazios);
- projetos de testes (existem dois) e CI (pendente);
- PostgreSQL descartável via Testcontainers (pendente);
- migrations consolidadas e banco documentado (documentado; consolidação pendente).

Saída: build e testes verdes, fronteiras arquiteturais verificadas.

## Onda 3 — Identidade e segurança (Parcial)

- cadastro e confirmação por SMTP (pendente);
- aprovação administrativa e múltiplos perfis (perfis feitos; aprovação formal pendente);
- MFA para perfis críticos (pendente);
- políticas, auditoria, rate limiting e headers (política de fallback e auditoria feitas; restante pendente).

Saída: cenários positivos e negativos automatizados, sem acesso operacional indevido.

## Onda 4 — Design system e navegação (Concluída)

- paleta industrial, login institucional e shell autenticado com sidebar;
- acessibilidade WCAG 2.2 AA (revisão pendente).

## Onda 5 — Cadastros (Parcial)

- requisitos de campos validados (pendente);
- clientes, produtos e lotes (feitos); inativação, pesquisa, paginação, concorrência (pendentes).

## Onda 6 — Reclamação de Cliente (Parcial)

- ciclo da RC, prazos e transições (feitos);
- geração da NC após validação GQ (feita);
- pendentes: início do prazo com informações completas, tratamento comercial, reabertura conforme requisito.

## Onda 7 — Não Conformidade (Concluída)

- contenção, investigação, causa, ações e eficácia;
- aprovações segregadas e decisão do CQ;
- encerramento e reabertura auditados.

## Onda 8 — Recall (Concluída)

- avaliação, aprovação, recolhimento, retorno e destinação;
- comunicações e encerramento;
- reabertura.

## Onda 9 — Serviços e informação gerencial (Parcial)

- anexos (feitos em disco; ClamAV e validações pendentes);
- calendário, dias úteis e notificações (feitos);
- dashboard, relatórios, PDF e XLSX (básicos feitos; relatórios completos pendentes).

## Onda 10 — Produção (Pendente)

- Docker Linux, PostgreSQL, proxy HTTPS e ClamAV;
- homologação separada;
- health checks, logs e backups;
- ensaio de restauração e checklist de publicação.
