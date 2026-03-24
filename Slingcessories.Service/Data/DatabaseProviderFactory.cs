namespace Slingcessories.Service.Data;

public static class DatabaseProviderFactory
{
    public static IDatabaseProviderStrategy Create(DatabaseProvider provider) => provider switch
    {
        DatabaseProvider.SqlServer => new SqlServerProviderStrategy(),
        DatabaseProvider.PostgreSql => new PostgreSqlProviderStrategy(),
        _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, $"Unsupported database provider: {provider}")
    };
}
