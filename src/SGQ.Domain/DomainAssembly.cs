using System.Reflection;

namespace SGQ.Domain;

public static class DomainAssembly
{
    public static Assembly Reference => typeof(DomainAssembly).Assembly;
}
