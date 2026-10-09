# Guia de interface

Este guia reúne os componentes e as regras de apresentação do SGQ. Use-o ao criar ou alterar telas, para manter o sistema consistente, acessível e fácil de usar.

## Princípios

1. O usuário sempre deve saber onde está, em que situação o processo se encontra, o que fazer agora e o que mudou.
2. Português do Brasil com acentuação correta; nada de nomes técnicos de código na tela.
3. Poucas ações primárias por painel; ações destrutivas ou irreversíveis pedem confirmação (`data-confirm`).
4. A interface reflete as permissões do perfil, mas o servidor continua sendo a barreira real.
5. Sem rolagem horizontal da página em telas de 390 px; alvos de toque de pelo menos 44 px; contraste mínimo de 4,5:1; foco visível.

## Textos legíveis de enumerações

| Necessidade | Use |
| --- | --- |
| Texto de uma situação, classificação ou origem | `@valor.Rotulo()` (nunca `ToString()`) |
| Selo colorido de situação ou classificação | `@Html.StatusBadge(valor)` |
| Lista suspensa de uma enumeração | `Html.GetEnumSelectList<T>()` (já usa os rótulos) |
| Selo de prazo (vencido, vence em N dias) | `@Html.PrazoChipHtml(prazo, encerrado)` |

Os rótulos vêm de `[Display(Name = "...")]` nos enums. Ao criar um valor novo, defina o rótulo no próprio enum. O tom do selo (`neutro`, `info`, `alerta`, `sucesso`, `perigo`) está em `EnumRotulos.Tom`.

## Componentes de página (`site.css`)

| Classe | Uso |
| --- | --- |
| `page-header`, `page-header-main`, `page-header-lead`, `page-header-actions` | Título da página com subtítulo e botões à direita |
| `meta-row`, `meta-item`, `meta-label`, `meta-value` | Fatos-chave logo abaixo do título (prazo, responsável, abertura) |
| `dl-grid` com `dt`/`dd` (`dl-wide` ocupa a linha toda) | Dados somente leitura em grade, rótulo acima do valor |
| `_Stepper` com `StepperModel` | Trilho de etapas do fluxo; `Atual` é o índice da etapa em andamento |
| `next-action` (`data-tom`: padrão, `espera`, `alerta`, `concluido`) | Painel "Próxima ação": o que fazer agora e quem faz |
| `filter-bar`, `filter-search`, `filter-actions`, `chip-row`, `chip`, `chip-count` | Barra de filtros e atalhos por situação |
| `table-wrap` + `table-modern` (+ `table-cards` com `data-label`) | Tabelas; no celular viram cartões |
| `form-section`, `form-grid` com `span-3/4/5/6/8/9`, `required`, `form-actions` | Formulários em seções, com ajuda curta |
| `panel`, `panel-heading`, `eyebrow`, `empty-state` | Painéis, títulos de seção e estados vazios |
| `status-badge` | Selo genérico (prefira `Html.StatusBadge`) |
| `timeline`, `file-*` | Histórico de alterações e anexos (partials `_HistoricoAuditoria` e `_Anexos`) |

Estilos específicos de um módulo ficam em `wwwroot/css/modulos/<modulo>.css`, carregados pela seção `Styles` da view:

```cshtml
@section Styles {
    <link rel="stylesheet" href="~/css/modulos/reclamacoes.css" asp-append-version="true" />
}
```

## Comportamentos globais (`site.js`)

| Atributo ou recurso | Efeito |
| --- | --- |
| `data-confirm="Pergunta"` em `form` ou `button` | Abre um diálogo acessível (foco preso, Esc cancela) com "Cancelar" e "Confirmar"; só envia se confirmar. Use `data-confirm-neutral` para um botão de confirmação sem destaque de perigo |
| Formulários `POST` | Ao enviar, os botões ficam desabilitados e o clicado mostra "Enviando…"; se a validação barrar o envio nada muda. Use `data-no-lock` no formulário para desligar |
| `TempData["Success"]`, `["Warning"]`, `["Error"]` | O layout exibe avisos no topo (`aviso`): sucesso some em cerca de 6 s, alerta e erro permanecem. Não é preciso renderizar alertas nas views; alertas iguais no corpo da página são removidos |
| `window.sgqAviso(texto, tipo)` | Mostra um aviso por script (`sucesso`, `alerta`, `erro`, `info`) |
| `<tr data-href="/url">` | Linha clicável; mantenha também um link real na linha para teclado. Cliques em links, botões e campos da linha não navegam |
| `data-history-back` em um link | Volta à página anterior quando houver, senão segue o `href` |
| `data-file-picker` | Seletor de arquivo em português (ver `_Anexos`) |

## Estrutura da página (layout)

- A barra superior mostra a trilha de navegação (Início / Seção / Página, derivada do controller e de `ViewData["Title"]`) e o menu do usuário (nome, perfis em português, Minha conta, Alterar senha, Sair).
- O menu lateral só mostra o que o perfil pode usar: Relatórios para Administrador, GQ e Auditor; Usuários só para Administrador. Em telas até 900 px ele vira gaveta (botão de menu, fundo escurecido, Esc fecha, foco preso).
- Contas autenticadas sem perfil não recebem o menu: veem o visual de acesso e a página de acesso negado orienta a procurar um Administrador.
- Páginas de conta (Identity) ficam em `Areas/Identity/Pages/Account`, usam o layout `_AuthLayout` (mesmo visual do login) e os componentes `account-alert`, `account-state-icon`, `btn-account`, `account-help`. As mensagens de validação do Identity vêm de `PortugueseIdentityErrorDescriber`.

## Visão geral (dashboard)

Saudação com o nome (derivado do e-mail, pois a conta não guarda nome), bloco "Precisa da sua atenção" por perfil, cartões de Reclamações, Não conformidades e Recalls em aberto e os últimos processos abertos. As regras por perfil estão em `HomeController.MontarAtencaoAsync` e são cobertas por `HomeControllerTests`.

## Como verificar uma tela

1. Suba a aplicação com os dados fictícios (`docs/desenvolvimento/ambiente-local.md`).
2. Percorra a tela com cada perfil relevante (Administrador, GQ, RT, CQ, Auditor).
3. Reduza a janela para 390 px de largura e confirme que não há rolagem horizontal.
4. Navegue só com o teclado (Tab, Enter, Esc) e confirme que o foco é sempre visível.
5. Envie o formulário vazio e com dados inválidos para conferir as mensagens de validação.
