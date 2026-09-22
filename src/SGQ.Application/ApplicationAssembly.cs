using System.Reflection;

namespace SGQ.Application;

public static class ApplicationAssembly
{
    public static Assembly Reference => typeof(ApplicationAssembly).Assembly;
}
