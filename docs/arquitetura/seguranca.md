# Arquitetura de segurança

## Modelo de acesso aprovado

```text
Cadastro público
→ confirmação de e-mail
→ aprovação administrativa
→ atribuição de um ou mais perfis
→ MFA para perfis críticos
→ acesso operacional
```

Perfis iniciais: Administrador, GQ, RT, CQ e Colaborador. Aprovações que exigem GQ e RT devem ser registradas por contas distintas.

## Controles planejados

- autenticação obrigatória por padrão;
- senha entre 12 e 128 caracteres;
- bloqueio de 15 minutos após cinco falhas;
- rate limiting de autenticação e recuperação;
- sessão inativa de 30 minutos e persistente de até sete dias;
- TOTP obrigatório para Administrador, GQ, RT e CQ;
- autenticação recente para ações críticas;
- cookies seguros, HSTS e cabeçalhos defensivos;
- chaves de Data Protection persistentes;
- autorização no backend;
- auditoria imutável e retenção documental de dois anos.

## Dados e repositório público

Não são permitidos dados reais, POPs, dumps, credenciais, tokens, nomes confidenciais ou documentos internos. Seeds e testes usam dados fictícios. Logs não registram senhas, tokens, conteúdo de anexos nem dados pessoais desnecessários.

## Anexos

Uploads exigirão allowlist, assinatura real compatível com o formato, hash SHA-256, limite no servidor e varredura ClamAV. Downloads passarão por autorização e usarão cabeçalhos que impeçam execução inline indevida.

## Acompanhamento

Achados e evidências ficam em [revisao-seguranca.md](../qualidade/revisao-seguranca.md). Vulnerabilidades exploráveis seguem [SECURITY.md](../../SECURITY.md), nunca issues públicas.
