# Política de Segurança

## Como relatar

Não publique vulnerabilidades exploráveis em issues, discussões ou Pull Requests. Use a opção **Report a vulnerability** na aba **Security** do repositório, que cria um GitHub Security Advisory privado. O recurso precisa estar habilitado nas configurações do repositório.

Inclua, quando possível:

- componente e versão afetados;
- pré-condições;
- passos mínimos para reprodução com dados fictícios;
- impacto observado;
- sugestão de correção, se houver.

Não inclua credenciais, dados pessoais, dumps reais, documentos internos ou evidências confidenciais.

## Escopo atual

O SGQ está em desenvolvimento e ainda não deve ser tratado como pronto para produção. Situação dos controles:

- perfis e autorização no backend: parcial (transições críticas e política de fallback implementadas; perfis por ação de escrita planejados);
- auditoria: parcial (existe e é mutável);
- anexos protegidos: parcial (extensão, tamanho e download autorizado; validação de conteúdo e antimalware planejados);
- aprovação de conta, MFA, rate limiting e hardening de produção: planejados.

O detalhamento está em [arquitetura de segurança](docs/arquitetura/seguranca.md) e [revisão de segurança](docs/qualidade/revisao-seguranca.md).

## Princípios do projeto

- segredos ficam fora do Git;
- todo acesso é validado no backend;
- dados e documentos reais não entram no repositório público;
- dependências e código passarão por verificações automáticas no CI (planejado);
- eventos críticos mantêm auditoria (imutabilidade planejada);
- correções de segurança recebem prioridade sobre funcionalidades.

## Divulgação

A divulgação pública deverá ocorrer somente depois da correção estar disponível e dos responsáveis avaliarem o risco. O histórico público não deverá revelar detalhes que coloquem ambientes ainda não atualizados em risco.
