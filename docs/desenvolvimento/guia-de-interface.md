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

| Atributo | Efeito |
| --- | --- |
| `data-confirm="Pergunta"` em `form` ou `button` | Pede confirmação antes de enviar |
| Formulários `POST` | O botão de envio é desabilitado após o primeiro clique para evitar duplicidade |
| `data-file-picker` | Seletor de arquivo em português (ver `_Anexos`) |

## Como verificar uma tela

1. Suba a aplicação com os dados fictícios (`docs/desenvolvimento/ambiente-local.md`).
2. Percorra a tela com cada perfil relevante (Administrador, GQ, RT, CQ, Auditor).
3. Reduza a janela para 390 px de largura e confirme que não há rolagem horizontal.
4. Navegue só com o teclado (Tab, Enter, Esc) e confirme que o foco é sempre visível.
5. Envie o formulário vazio e com dados inválidos para conferir as mensagens de validação.
