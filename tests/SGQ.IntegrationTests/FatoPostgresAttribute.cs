namespace SGQ.IntegrationTests;

/// <summary>
/// Fato que só roda quando há um PostgreSQL configurado em SGQ_TEST_PG. No CI a variável é sempre definida,
/// então os testes nunca são ignorados lá; localmente, sem o banco, aparecem como ignorados com o motivo.
/// </summary>
public sealed class FatoPostgresAttribute : FactAttribute
{
    public FatoPostgresAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(SgqWebFactory.VariavelConexao)))
            Skip = $"Defina {SgqWebFactory.VariavelConexao} (conexão PostgreSQL sem o nome do banco) para executar.";
    }
}

/// <summary>Teoria equivalente ao <see cref="FatoPostgresAttribute"/>.</summary>
public sealed class TeoriaPostgresAttribute : TheoryAttribute
{
    public TeoriaPostgresAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(SgqWebFactory.VariavelConexao)))
            Skip = $"Defina {SgqWebFactory.VariavelConexao} (conexão PostgreSQL sem o nome do banco) para executar.";
    }
}
