# CAL-001 — Calendário de dias não úteis e prazos

**Objetivo:** conferir o cadastro de dias não úteis e o efeito deles no cálculo dos prazos (NC Maior e Menor).
**Perfis envolvidos:** GQ (`gq@sgq.test`).
**Tempo estimado:** 25 minutos.
**Pré-condição:** sistema iniciado com `scripts\executar-demo.ps1`. O calendário de demonstração traz feriados nacionais do fim do ano corrente e 1º de janeiro do ano seguinte.

## Regra de prazos (para conferir as contas)

- NC **Maior**: 15 dias úteis a partir da abertura. NC **Menor**: 30 dias úteis. NC **Crítica**: sem prazo calculado (a data-alvo é informada).
- Dia útil = segunda a sexta que **não** esteja cadastrado (e ativo) no calendário.
- Prazos já calculados **não** são recalculados quando o calendário muda.

## Passos

| Nº | Perfil (conta) | O que fazer | Resultado esperado | Resultado (✔/✘) | Observações |
| --- | --- | --- | --- | --- | --- |
| 1 | `gq@sgq.test` | Abra **Calendário** no menu. | Lista agrupada por mês, com dia da semana, tipo (Nacional/Estadual/Municipal/Interno) e o selo "Próximo" no feriado mais próximo; texto explicando que o calendário alimenta o cálculo de prazos em dias úteis. | | |
| 2 | `gq@sgq.test` | Use **Ver ano** para trocar de ano e a busca para localizar "Natal". | A lista muda de ano e a busca filtra. | | |
| 3 | `gq@sgq.test` | Abra **Novo dia não útil**; deixe tudo vazio e salve. | Mensagens de validação em português. | | |
| 4 | `gq@sgq.test` | Cadastre uma data que **não** seja fim de semana, 3 semanas à frente (ex.: uma quarta-feira), tipo **Interno**, descrição `Recesso do roteiro`, mantendo **Descontar esta data no cálculo de prazos** marcado. | Aviso "Dia não útil cadastrado."; o dia aparece no mês correto; o campo **Ano** é preenchido sozinho a partir da data. | | Anote a data cadastrada. |
| 5 | `gq@sgq.test` | Tente cadastrar de novo a **mesma data** e o mesmo tipo. | Erro de duplicidade em português; nada é salvo. | | |
| 6 | `gq@sgq.test` | Edite o dia criado para um **ano** diferente do da data. | Erro explicando que o ano deve coincidir com a data. | | |
| 7 | `gq@sgq.test` | Para ver o efeito: abra **Nova não conformidade**, classificação **Maior**, com **Data de abertura** igual a hoje, e salve. Anote o prazo exibido. | O prazo é a data 15 dias úteis depois de hoje (conte só segunda a sexta, pulando os dias cadastrados no calendário). | | Use um calendário comum para conferir a contagem. |
| 8 | `gq@sgq.test` | Edite o dia `Recesso do roteiro` e **desmarque** "Descontar esta data…" (inativar); depois abra outra NC **Maior** com a mesma data de abertura. | O prazo da nova NC fica **um dia útil antes** do prazo do passo 7 (o recesso deixou de ser descontado), se o recesso cai entre a abertura e o prazo. | | |
| 9 | `gq@sgq.test` | Abra a NC do passo 7 novamente. | O prazo dela **não** mudou (prazos já calculados não são recalculados). | | |
| 10 | `gq@sgq.test` | Abra uma NC **Menor** com a mesma data de abertura. | Prazo 30 dias úteis à frente, maior que o da Maior. | | |
| 11 | `gq@sgq.test` | Abra uma NC com data de abertura numa **sexta-feira**. | A contagem de dias úteis começa na segunda seguinte (fim de semana não conta). | | |
| 12 | `gq@sgq.test` | Abra uma **Nova reclamação** e registre. | O prazo da RC é calculado automaticamente (não há campo de data-alvo no formulário). | | |
| 13 | `gq@sgq.test` | Ao final, remova o efeito do teste: edite o dia `Recesso do roteiro` e deixe-o inativo. | O calendário volta ao estado original para os demais roteiros. | | Não há excluir. |

## Defeitos conhecidos / observações

- Se o calendário do ano do prazo não estiver cadastrado, o prazo sai calculado só com fins de semana; a tela do calendário avisa quando o ano corrente não tem nenhum dia ativo.
- O cálculo usa a data do computador onde o sistema roda (sem fuso de negócio definido; item no backlog).
