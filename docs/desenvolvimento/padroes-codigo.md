# Padrões de código

## C# e .NET

- nullable reference types permanecem ativos;
- warnings serão tratados como erros na fundação técnica;
- código assíncrono usa sufixo `Async` fora de actions MVC convencionais;
- controllers recebem ViewModels/Requests, nunca entidades persistentes em edição;
- regras de negócio ficam no Domain/Application;
- acesso a dados e serviços externos ficam na Infrastructure;
- datas instantâneas usam UTC; datas civis usam `DateOnly`;
- dinheiro e quantidades usam precisão explícita;
- cancelamento deve ser propagado com `CancellationToken`.

## Banco

- migrations acompanham qualquer alteração persistente;
- FKs, índices, unicidade e deleção são explícitos;
- cadastros referenciados são inativados, não apagados;
- ações relevantes geram auditoria;
- queries de lista usam paginação e projeção.

## Interface

- HTML semântico, labels associados e navegação por teclado;
- contraste WCAG 2.2 AA;
- autorização nunca depende apenas de esconder botões;
- mensagens explicam como corrigir o problema sem revelar detalhes sensíveis;
- cores e espaçamentos usam tokens do design system.

## Testes

Um teste deve demonstrar comportamento, não detalhes internos. Toda correção começa com reprodução automatizada quando viável. Regras de Domain/Application devem manter ao menos 80% de cobertura e todos os cenários críticos definidos na daily.
