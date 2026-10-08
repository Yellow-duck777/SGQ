# ADR 0003 — Notificações por e-mail e deduplicação

- Estado: aceito (registro retroativo)
- Data: 2026-10-08

## Contexto

O requisito RF-014 define e-mail no MVP, uma matriz de eventos e destinatários e alertas 48 horas úteis antes do prazo para GQ e RT.

## Decisão

- `FluxoNotificacaoService` envia notificações de eventos do fluxo para os perfis definidos no requisito e para usuários envolvidos; `AlertasPrazoHostedService` roda a cada hora e envia alertas de prazo e de ação de NC vencida.
- O envio usa SMTP configurado por User Secrets ou variáveis de ambiente (`Smtp:*`). Sem configuração, nada é enviado e a operação principal prossegue.
- Uma falha de envio é registrada em log e não desfaz a operação principal.
- A tabela `AlertasEnviados` evita repetição: cada envio é único por tipo, referência e data.
- As 48 horas úteis são aproximadas por 2 dias úteis a partir do dia corrente.

## Alternativas

- Fila persistente com reprocessamento (outbox): mais robusta, adiada por custo.
- Notificação apenas na interface: rejeitada pelo requisito de e-mail.
- Serviço externo de e-mail transacional: possível no futuro; SMTP é configurável.

## Consequências

- Eventos distintos do mesmo tipo no mesmo dia para o mesmo processo ficam suprimidos pela deduplicação por data; a deduplicação por evento e o reenvio de falhas dependem da demanda DEM-2026-105.
- O envio ocorre dentro da requisição, após a gravação.
- Granularidade em horas úteis não é suportada.
