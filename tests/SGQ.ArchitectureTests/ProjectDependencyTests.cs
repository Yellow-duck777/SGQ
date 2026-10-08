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
