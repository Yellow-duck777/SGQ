# Política de Segurança

## Como relatar

Não publique vulnerabilidades exploráveis em issues, discussões ou Pull Requests. Use a opção **Report a vulnerability** na aba **Security** do repositório, que cria um GitHub Security Advisory privado.

Inclua, quando possível:

- componente e versão afetados;
- pré-condições;
- passos mínimos para reprodução com dados fictícios;
- impacto observado;
- sugestão de correção, se houver.

Não inclua credenciais, dados pessoais, dumps reais, documentos internos ou evidências confidenciais.

## Escopo atual

O SGQ está em desenvolvimento e ainda não deve ser tratado como pronto para produção. Perfis, autorização granular, MFA, auditoria, anexos protegidos e hardening de produção constam no roadmap.

## Princípios do projeto

- segredos ficam fora do Git;
- todo acesso é validado no backend;
- dados e documentos reais não entram no repositório público;
- dependências e código passam por verificações automáticas;
- eventos críticos mantêm auditoria imutável;
- correções de segurança recebem prioridade sobre funcionalidades.

## Divulgação

A divulgação pública deverá ocorrer somente depois da correção estar disponível e dos responsáveis avaliarem o risco. O histórico público não deverá revelar detalhes que coloquem ambientes ainda não atualizados em risco.
