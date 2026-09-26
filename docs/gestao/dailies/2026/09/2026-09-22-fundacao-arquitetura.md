# Daily — Fundação arquitetural — 2026-09-22

## Identificação

- Demanda: `DEM-2026-002`
- Responsável: desenvolvimento
- Revisor técnico: `@ViniciusLgo`
- Validador da Qualidade: não aplicável para reorganização sem mudança funcional
- Branch: `refactor/fundacao-arquitetura`
- Estado: Concluída

## Objetivo

Criar as fronteiras iniciais do monólito modular e mover modelos operacionais puros para `Domain` sem alterar o comportamento ou o esquema físico do sistema.

## Contexto

O código de domínio, EF Core, Identity, controllers e apresentação estavam concentrados em `SGQ.Web`. A mudança integra primeiro a documentação e o frontend aprovados e depois separa a base técnica de forma incremental.

## Critérios de aceite

- [x] Documentação e frontend integrados numa branch limpa.
- [x] Projetos Domain, Application, Infrastructure e Web compilam juntos.
- [x] Testes automatizados impedem dependências proibidas entre camadas.
- [x] Entidades e enums operacionais pertencem a Domain.
- [x] O snapshot do EF permanece coerente e não existe alteração pendente de modelo.
- [x] Login, dashboard e módulos atuais continuam acessíveis no navegador.

## Fora do escopo

- mover `ApplicationDbContext`, Identity ou migrations para Infrastructure;
- criar casos de uso na Application;
- alterar tabelas ou recriar o banco;
- modificar regras funcionais de cadastros, RC ou NC;
- configurar CI ou cobertura de 80%.

## Execução

- [x] Criar worktree e integrar commits documentais e visuais.
- [x] Criar projetos e referências arquiteturais.
- [x] Criar e provar os testes de dependência.
- [x] Mover entidades e enums sem alterar assinaturas.
- [x] Verificar EF Core, testes e execução no navegador.
- [x] Atualizar arquitetura e backlog.

## Evidências

- `dotnet restore SGQ.slnx`: projetos restaurados.
- `dotnet build SGQ.slnx --no-restore`: zero erro e zero warning.
- `dotnet test SGQ.slnx --no-build`: três testes aprovados.
- `dotnet ef migrations has-pending-model-changes`: nenhuma mudança de modelo encontrada.
- `curl.exe -I http://localhost:5026`: `302` para `/Identity/Account/Login`.
- navegador: login, dashboard, Clientes, Produtos, Lotes, Reclamações, Não Conformidades e logout verificados.
- console do navegador: nenhum erro encontrado.

## Decisões e riscos

- `ApplicationDbContext` permanece em Web para que a mudança seja revisável e não misture persistência com casos de uso.
- Web referencia Domain temporariamente enquanto controllers e Razor usam as entidades diretamente.
- Migrations históricas mantêm os namespaces originais; somente o snapshot corrente foi alinhado.
- A proteção de chaves de Data Protection em desenvolvimento continua pendente para a onda de segurança.

## Encerramento

- Resultado: primeira fronteira arquitetural implementada e verificada.
- Pendência: executar `DEM-2026-003` antes de mover persistência e casos de uso.
- Revisão técnica: aguardando revisão dos commits locais.
- Validação da Qualidade: não aplicável nesta fatia sem mudança de regra.
