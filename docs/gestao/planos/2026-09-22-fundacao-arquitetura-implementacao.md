# Fundação Arquitetural — Plano de Implementação

> **Para agentes de IA:** use a skill `executing-plans` para executar este plano tarefa por tarefa. Cada passo usa checkbox para permitir acompanhamento e revisão.

**Objetivo:** integrar a fundação documental e a interface aprovada, criar as fronteiras iniciais do monólito modular e mover os modelos de negócio puros para `SGQ.Domain` sem alterar o comportamento do sistema.

**Arquitetura:** esta primeira fatia mantém EF Core e Identity em `SGQ.Web` para reduzir risco, mas cria `Domain`, `Application` e `Infrastructure` com dependências unidirecionais. Os modelos operacionais puros migram para `Domain`; identidade, ViewModels, controllers e persistência permanecem em `Web` até os planos seguintes.

**Stack:** .NET 10, ASP.NET Core MVC, EF Core 10.0.11, Npgsql 10.0.3, PostgreSQL 17, xUnit e PowerShell 7.

## Restrições globais

- Trabalhar em branch diferente de `main` e worktree isolada.
- Não enviar commits, abrir PR ou executar merge sem autorização específica.
- Preservar o comportamento e o esquema físico do banco nesta fase.
- Não editar migrations históricas; atualizar somente o snapshot corrente quando necessário.
- Não mover `ApplicationUser` nem `ErrorViewModel` para `Domain`.
- Não introduzir casos de uso, repositórios genéricos ou abstrações sem consumidor.
- Manter .NET 10 e as versões atuais dos pacotes de produção.
- Tratar warnings do código próprio como erros.
- Usar dados sintéticos e manter segredos somente em User Secrets.
- Encerrar cada tarefa com build e commit local por intenção.

---

### Tarefa 1: Criar a linha integrada e registrar a baseline

**Arquivos:**

- Consumir commits documentais: `dfb2348` e `3a9bfd5`
- Consumir commit visual: `bdaa9f4`
- Criar worktree: `C:\laragon\www\sis_cris-foundation`
- Criar branch: `refactor/fundacao-arquitetura`

**Interfaces:**

- Consome: documentação versionada e interface autenticada já verificadas.
- Produz: uma única branch-base limpa para todas as tarefas seguintes.

- [x] **Passo 1: confirmar que as branches de origem estão limpas**

```powershell
git -C C:\laragon\www\sis_cris status --short
git -C C:\laragon\www\sis_cris-ui status --short
```

Esperado: nenhuma saída em ambas.

- [x] **Passo 2: criar a branch a partir da documentação**

```powershell
git -C C:\laragon\www\sis_cris worktree add `
  -b refactor/fundacao-arquitetura `
  C:\laragon\www\sis_cris-foundation `
  docs/fundacao-governanca
```

Esperado: worktree criada no commit `f05c292`, que já contém o plano de implementação corrigido.

- [x] **Passo 3: integrar o frontend aprovado**

```powershell
git -C C:\laragon\www\sis_cris-foundation cherry-pick bdaa9f4
```

Esperado: cherry-pick sem conflito, preservando a mensagem `feat(ui): redesenhar login e área autenticada`.

- [x] **Passo 4: restaurar e compilar a baseline**

```powershell
$env:DOTNET_ROOT = "$env:LOCALAPPDATA\SGQ\dotnet"
$env:PATH = "$env:DOTNET_ROOT;$env:LOCALAPPDATA\SGQ\tools;$env:PATH"
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore
```

Esperado: build concluído com zero erro e zero warning.

- [x] **Passo 5: confirmar o histórico integrado**

```powershell
git log --oneline --decorate -3
git status --short
```

Esperado: commits documental e visual no histórico; worktree limpa. Não criar commit adicional nesta tarefa.

---

### Tarefa 2: Centralizar regras de compilação e criar os projetos

**Arquivos:**

- Criar: `Directory.Build.props`
- Criar: `src/SGQ.Domain/SGQ.Domain.csproj`
- Criar: `src/SGQ.Domain/DomainAssembly.cs`
- Criar: `src/SGQ.Application/SGQ.Application.csproj`
- Criar: `src/SGQ.Application/ApplicationAssembly.cs`
- Criar: `src/SGQ.Infrastructure/SGQ.Infrastructure.csproj`
- Criar: `src/SGQ.Infrastructure/InfrastructureAssembly.cs`
- Modificar: `src/SGQ.Web/SGQ.Web.csproj`
- Modificar: `SGQ.slnx`

**Interfaces:**

- Consome: .NET 10 e `SGQ.Web` existentes.
- Produz: assemblies `SGQ.Domain`, `SGQ.Application` e `SGQ.Infrastructure` referenciáveis pelas tarefas seguintes.

- [x] **Passo 1: criar as regras compartilhadas**

Adicionar `Directory.Build.props`:

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

- [x] **Passo 2: criar o projeto Domain sem dependências**

Adicionar `src/SGQ.Domain/SGQ.Domain.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
</Project>
```

Adicionar `src/SGQ.Domain/DomainAssembly.cs`:

```csharp
using System.Reflection;

namespace SGQ.Domain;

public static class DomainAssembly
{
    public static Assembly Reference => typeof(DomainAssembly).Assembly;
}
```

- [x] **Passo 3: criar o projeto Application dependente somente de Domain**

Adicionar `src/SGQ.Application/SGQ.Application.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\SGQ.Domain\SGQ.Domain.csproj" />
  </ItemGroup>
</Project>
```

Adicionar `src/SGQ.Application/ApplicationAssembly.cs`:

```csharp
using System.Reflection;

namespace SGQ.Application;

public static class ApplicationAssembly
{
    public static Assembly Reference => typeof(ApplicationAssembly).Assembly;
}
```

- [x] **Passo 4: criar Infrastructure dependente de Application**

Adicionar `src/SGQ.Infrastructure/SGQ.Infrastructure.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\SGQ.Application\SGQ.Application.csproj" />
  </ItemGroup>
</Project>
```

Adicionar `src/SGQ.Infrastructure/InfrastructureAssembly.cs`:

```csharp
using System.Reflection;

namespace SGQ.Infrastructure;

public static class InfrastructureAssembly
{
    public static Assembly Reference => typeof(InfrastructureAssembly).Assembly;
}
```

- [x] **Passo 5: referenciar as camadas no projeto Web**

Manter os `PackageReference` atuais e adicionar em `src/SGQ.Web/SGQ.Web.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\SGQ.Application\SGQ.Application.csproj" />
  <ProjectReference Include="..\SGQ.Infrastructure\SGQ.Infrastructure.csproj" />
</ItemGroup>
```

- [x] **Passo 6: registrar os projetos na solução**

Atualizar `SGQ.slnx` para:

```xml
<Solution>
  <Folder Name="/src/">
    <Project Path="src/SGQ.Application/SGQ.Application.csproj" />
    <Project Path="src/SGQ.Domain/SGQ.Domain.csproj" />
    <Project Path="src/SGQ.Infrastructure/SGQ.Infrastructure.csproj" />
    <Project Path="src/SGQ.Web/SGQ.Web.csproj" />
  </Folder>
</Solution>
```

- [x] **Passo 7: restaurar e compilar**

```powershell
dotnet restore SGQ.slnx
dotnet build SGQ.slnx --no-restore
```

Esperado: quatro projetos compilados; zero erro e zero warning.

- [x] **Passo 8: criar o commit local**

```powershell
git add -- Directory.Build.props SGQ.slnx src/SGQ.Domain src/SGQ.Application src/SGQ.Infrastructure src/SGQ.Web/SGQ.Web.csproj
git commit -m "refactor(arquitetura): cria fronteiras do monólito"
```

---

### Tarefa 3: Criar testes que protegem as dependências

**Arquivos:**

- Criar: `tests/SGQ.ArchitectureTests/SGQ.ArchitectureTests.csproj`
- Criar: `tests/SGQ.ArchitectureTests/ProjectDependencyTests.cs`
- Modificar: `SGQ.slnx`

**Interfaces:**

- Consome: propriedades `DomainAssembly.Reference`, `ApplicationAssembly.Reference` e `InfrastructureAssembly.Reference`.
- Produz: testes que falham quando uma camada passa a referenciar uma camada proibida.

- [x] **Passo 1: gerar o projeto xUnit com as versões do template .NET 10 instalado**

```powershell
dotnet new xunit --framework net10.0 --output tests/SGQ.ArchitectureTests
dotnet add tests/SGQ.ArchitectureTests/SGQ.ArchitectureTests.csproj reference src/SGQ.Domain/SGQ.Domain.csproj
dotnet add tests/SGQ.ArchitectureTests/SGQ.ArchitectureTests.csproj reference src/SGQ.Application/SGQ.Application.csproj
dotnet add tests/SGQ.ArchitectureTests/SGQ.ArchitectureTests.csproj reference src/SGQ.Infrastructure/SGQ.Infrastructure.csproj
```

Remover o arquivo `UnitTest1.cs` gerado pelo template.

- [x] **Passo 2: escrever inicialmente um teste com uma dependência proibida para provar a proteção**

Adicionar temporariamente a `ProjectDependencyTests.cs`:

```csharp
using SGQ.Domain;

namespace SGQ.ArchitectureTests;

public class ProjectDependencyTests
{
    [Fact]
    public void ProvaDoTeste_DeveFalharComDependenciaInexistente()
    {
        var references = DomainAssembly.Reference.GetReferencedAssemblies();

        Assert.Contains(references, reference => reference.Name == "SGQ.Web");
    }
}
```

Executar `dotnet test tests/SGQ.ArchitectureTests/SGQ.ArchitectureTests.csproj` e confirmar falha em `Assert.Contains`. Depois substituir pelo conteúdo definitivo do passo seguinte.

- [x] **Passo 3: escrever os testes definitivos**

Adicionar `tests/SGQ.ArchitectureTests/ProjectDependencyTests.cs`:

```csharp
using SGQ.Application;
using SGQ.Domain;
using SGQ.Infrastructure;

namespace SGQ.ArchitectureTests;

public class ProjectDependencyTests
{
    [Fact]
    public void Domain_NaoDeveDependerDeCamadasExternas()
    {
        var references = Names(DomainAssembly.Reference);

        Assert.DoesNotContain("SGQ.Application", references);
        Assert.DoesNotContain("SGQ.Infrastructure", references);
        Assert.DoesNotContain("SGQ.Web", references);
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.EntityFrameworkCore"));
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.AspNetCore"));
        Assert.DoesNotContain(references, name => name.StartsWith("Npgsql"));
    }

    [Fact]
    public void Application_NaoDeveDependerDeInfrastructureOuWeb()
    {
        var references = Names(ApplicationAssembly.Reference)
            .Where(name => name.StartsWith("SGQ."))
            .ToArray();

        Assert.DoesNotContain("SGQ.Infrastructure", references);
        Assert.DoesNotContain("SGQ.Web", references);
    }

    [Fact]
    public void Infrastructure_NaoDeveDependerDeWeb()
    {
        var references = Names(InfrastructureAssembly.Reference);

        Assert.DoesNotContain("SGQ.Web", references);
    }

    private static string[] Names(System.Reflection.Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
}
```

- [x] **Passo 4: adicionar o projeto à solução**

Adicionar ao `SGQ.slnx`:

```xml
<Folder Name="/tests/">
  <Project Path="tests/SGQ.ArchitectureTests/SGQ.ArchitectureTests.csproj" />
</Folder>
```

- [x] **Passo 5: executar o teste**

```powershell
dotnet test tests/SGQ.ArchitectureTests/SGQ.ArchitectureTests.csproj
```

Esperado: três testes aprovados.

- [x] **Passo 6: criar o commit local**

```powershell
git add -- SGQ.slnx tests/SGQ.ArchitectureTests
git commit -m "test(arquitetura): protege dependências entre camadas"
```

---

### Tarefa 4: Mover os modelos operacionais puros para Domain

**Arquivos:**

- Criar em `src/SGQ.Domain/Entities/`: `Cliente.cs`, `Produto.cs`, `Lote.cs`, `ReclamacaoCliente.cs`, `ReclamacaoClienteLote.cs` e `NaoConformidade.cs`
- Criar em `src/SGQ.Domain/Enums/`: `ClassificacaoOcorrencia.cs`, `OrigemNaoConformidade.cs`, `StatusNaoConformidade.cs` e `StatusReclamacao.cs`
- Remover os dez arquivos correspondentes de `src/SGQ.Web/Models/`
- Modificar: controllers, `ApplicationDbContext.cs`, `DashboardViewModel.cs`, `ReclamacaoCreateViewModel.cs`, `_ViewImports.cshtml` e views tipadas
- Modificar: `src/SGQ.Web/Migrations/ApplicationDbContextModelSnapshot.cs`
- Modificar: `src/SGQ.Web/SGQ.Web.csproj`

**Interfaces:**

- Consome: propriedades e relacionamentos existentes sem alterar assinaturas.
- Produz: entidades em `SGQ.Domain.Entities` e enums em `SGQ.Domain.Enums`.

- [x] **Passo 1: adicionar a referência direta e temporária de Web para Domain**

Adicionar ao grupo de referências de `src/SGQ.Web/SGQ.Web.csproj`:

```xml
<ProjectReference Include="..\SGQ.Domain\SGQ.Domain.csproj" />
```

Essa referência é temporariamente aceita porque controllers, EF e views ainda consomem os modelos diretamente; será removida quando os casos de uso migrarem para `Application`.

- [x] **Passo 2: mover as entidades preservando todo o conteúdo**

Em cada entidade, alterar somente o namespace:

```csharp
namespace SGQ.Domain.Entities;
```

Quando uma entidade utilizar enum, adicionar:

```csharp
using SGQ.Domain.Enums;
```

Não alterar propriedades, validações, valores padrão ou relacionamentos nesta tarefa.

- [x] **Passo 3: mover os enums preservando membros e valores**

Em cada enum, alterar somente o namespace:

```csharp
namespace SGQ.Domain.Enums;
```

- [x] **Passo 4: atualizar consumidores C#**

Substituir `using SGQ.Web.Models;` pelos imports específicos abaixo quando aplicável:

```csharp
using SGQ.Domain.Entities;
using SGQ.Domain.Enums;
```

Manter `ApplicationUser` e `ErrorViewModel` em `SGQ.Web.Models`; arquivos que usam esses tipos continuam importando esse namespace.

- [x] **Passo 5: atualizar Razor**

Adicionar a `src/SGQ.Web/Views/_ViewImports.cshtml`:

```razor
@using SGQ.Domain.Entities
@using SGQ.Domain.Enums
```

Alterar declarações totalmente qualificadas, por exemplo:

```razor
@model IEnumerable<SGQ.Domain.Entities.Cliente>
@model SGQ.Domain.Entities.ReclamacaoCliente
```

Não alterar `_LoginPartial.cshtml`, `_Layout.cshtml` ou imports de Identity que usam `ApplicationUser`.

- [x] **Passo 6: alinhar somente o snapshot atual**

Em `ApplicationDbContextModelSnapshot.cs`, substituir:

```text
SGQ.Web.Models.Cliente                 → SGQ.Domain.Entities.Cliente
SGQ.Web.Models.Produto                 → SGQ.Domain.Entities.Produto
SGQ.Web.Models.Lote                    → SGQ.Domain.Entities.Lote
SGQ.Web.Models.ReclamacaoCliente       → SGQ.Domain.Entities.ReclamacaoCliente
SGQ.Web.Models.ReclamacaoClienteLote   → SGQ.Domain.Entities.ReclamacaoClienteLote
SGQ.Web.Models.NaoConformidade         → SGQ.Domain.Entities.NaoConformidade
```

Não alterar `SGQ.Web.Models.ApplicationUser` nem os arquivos de migrations históricas.

- [x] **Passo 7: comprovar que o modelo não gerou mudança física**

```powershell
dotnet build SGQ.slnx --no-restore
dotnet ef migrations has-pending-model-changes `
  --project src/SGQ.Web/SGQ.Web.csproj `
  --startup-project src/SGQ.Web/SGQ.Web.csproj
```

Esperado: build verde e mensagem informando que nenhuma alteração de modelo foi encontrada.

- [x] **Passo 8: executar todos os testes**

```powershell
dotnet test SGQ.slnx --no-build
```

Esperado: três testes arquiteturais aprovados.

- [x] **Passo 9: criar o commit local**

```powershell
git add -- src/SGQ.Domain src/SGQ.Web tests/SGQ.ArchitectureTests
git commit -m "refactor(dominio): move modelos operacionais para Domain"
```

---

### Tarefa 5: Verificar execução e documentar a dívida transitória

**Arquivos:**

- Modificar: `docs/arquitetura/visao-geral.md`
- Modificar: `docs/gestao/backlog.md`
- Criar: `docs/gestao/dailies/2026/09/2026-09-22-fundacao-arquitetura.md`

**Interfaces:**

- Consome: solução modular compilável e frontend integrado.
- Produz: evidências de execução e escopo preciso para o plano seguinte.

- [x] **Passo 1: executar a aplicação em porta isolada**

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --no-build `
  --project src/SGQ.Web/SGQ.Web.csproj `
  --urls http://localhost:5026
```

Esperado: aplicação ouvindo em `http://localhost:5026`.

- [x] **Passo 2: verificar o redirecionamento anônimo**

```powershell
curl.exe -I http://localhost:5026
```

Esperado: `HTTP/1.1 302 Found` e `Location: /Identity/Account/Login`.

- [x] **Passo 3: executar o roteiro no navegador**

Verificar com dados sintéticos:

```text
login
→ dashboard
→ Clientes
→ Produtos
→ Lotes
→ Reclamações
→ Não Conformidades
→ logout
```

Esperado: nenhuma exceção, link quebrado ou erro de console.

- [x] **Passo 4: atualizar a visão arquitetural**

Registrar em `visao-geral.md`:

- projetos criados;
- entidades já movidas;
- `ApplicationDbContext` e Identity ainda em `Web`;
- referência temporária `Web → Domain`;
- próxima fatia: contratos de casos de uso e persistência em `Infrastructure`.

- [x] **Passo 5: atualizar backlog e daily**

Marcar `DEM-2026-002` como `Implementado` somente para a fatia descrita e criar demanda separada para retirar o acesso direto dos controllers ao EF Core. Registrar comandos, resultados, riscos e evidência do navegador na daily.

- [x] **Passo 6: validação final**

```powershell
dotnet build SGQ.slnx --no-restore
dotnet test SGQ.slnx --no-build
git diff --check
git status --short
```

Esperado: build e testes verdes; nenhuma inconsistência de whitespace; somente os três documentos desta tarefa pendentes.

- [x] **Passo 7: criar o commit local**

```powershell
git add -- docs/arquitetura/visao-geral.md docs/gestao/backlog.md docs/gestao/dailies/2026/09/2026-09-22-fundacao-arquitetura.md
git commit -m "docs(arquitetura): registra primeira separação de camadas"
```

- [x] **Passo 8: revisar a branch**

```powershell
git log --oneline --decorate docs/fundacao-governanca..HEAD
git status --short
```

Esperado: frontend integrado e três commits da fundação arquitetural; worktree limpa. Nenhum push ou PR será executado.

## Resultado esperado da primeira execução

Ao final, o SGQ continuará funcional com o novo frontend, terá quatro projetos de produção, testes automatizados de dependência e modelos operacionais puros em `Domain`. Persistência, Identity e controllers continuarão em `Web` conscientemente; a próxima implementação criará casos de uso em `Application`, moverá EF Core para `Infrastructure` e eliminará o acesso direto dos controllers ao contexto.
