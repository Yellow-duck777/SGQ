# SGQ — Sistema de Gestão da Qualidade

Aplicação web para registrar, tratar e rastrear processos de qualidade. O produto foi planejado para Reclamações de Cliente (RC), Não Conformidades (NC) e Recall.

> O repositório é público. Não inclua dados reais, documentos internos, POPs, credenciais ou informações confidenciais.

## Estado atual

| Área | Situação |
| --- | --- |
| Autenticação com ASP.NET Core Identity | Em desenvolvimento |
| Cadastros de clientes, produtos e lotes | Parcialmente implementado |
| Abertura de RC como rascunho | Parcialmente implementado |
| Fluxo completo de RC | Planejado |
| Fluxos de NC e Recall | Planejados |
| Perfis, permissões, MFA e aprovação de contas | Planejados |
| Auditoria, anexos, prazos e notificações | Planejados |
| Testes automatizados e CI | Planejados |

O status documental de cada requisito é controlado na [matriz de rastreabilidade](docs/produto/matriz-rastreabilidade.md).

## Tecnologias

- .NET 10 e ASP.NET Core MVC;
- Entity Framework Core;
- PostgreSQL 17;
- ASP.NET Core Identity;
- Bootstrap, HTML, CSS e JavaScript;
- Docker, xUnit e Playwright, previstos para a fundação de testes.

## Pré-requisitos

- SDK .NET 10;
- PostgreSQL 17;
- Git;
- Docker Desktop para os futuros testes de integração e ponta a ponta.

Confirme o ambiente:

```powershell
dotnet --version
psql --version
docker version
```

## Configuração local

Clone o repositório e entre na pasta:

```powershell
git clone https://github.com/Yellow-duck777/SGQ.git
cd SGQ
```

Configure a conexão sem gravar senha no repositório:

```powershell
$senha = Read-Host "Senha do PostgreSQL" -AsSecureString
$senhaTexto = [System.Net.NetworkCredential]::new('', $senha).Password
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=sgq_dev;Username=postgres;Password=$senhaTexto" --project .\src\SGQ.Web\SGQ.Web.csproj
Remove-Variable senha, senhaTexto
```

Aplique as migrations e inicie a aplicação:

```powershell
dotnet ef database update --project .\src\SGQ.Web\SGQ.Web.csproj --startup-project .\src\SGQ.Web\SGQ.Web.csproj
dotnet run --project .\src\SGQ.Web\SGQ.Web.csproj --launch-profile http
```

Acesse `http://localhost:5024`.

## Validação disponível

O projeto ainda não possui testes automatizados. A validação mínima atual é:

```powershell
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore
dotnet package list --project .\src\SGQ.Web\SGQ.Web.csproj --vulnerable --include-transitive
```

Não trate build bem-sucedido como homologação funcional. Use também os roteiros em [docs/qualidade/roteiros](docs/qualidade/roteiros/README.md).

## Documentação

- [Índice da documentação](docs/README.md)
- [Visão do produto](docs/produto/visao-do-produto.md)
- [Requisitos consolidados](docs/produto/requisitos-consolidados.md)
- [Roadmap](docs/gestao/roadmap.md)
- [Como contribuir](CONTRIBUTING.md)
- [Política de segurança](SECURITY.md)

## Como trabalhar

Não desenvolva diretamente na `main`. Toda demanda deve possuir uma daily, branch própria, critérios de aceite, testes proporcionais e Pull Request revisado. Consulte [CONTRIBUTING.md](CONTRIBUTING.md) e o [tutorial de commit](docs/desenvolvimento/tutorial-commit.md).
