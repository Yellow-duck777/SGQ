using System.Reflection;

namespace SGQ.Infrastructure;

public static class InfrastructureAssembly
{
    public static Assembly Reference => typeof(InfrastructureAssembly).Assembly;
}
