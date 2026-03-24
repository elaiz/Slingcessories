using Microsoft.EntityFrameworkCore;

namespace Slingcessories.Service.Data;

public class PostgreSqlProviderStrategy : IDatabaseProviderStrategy
{
    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseNpgsql(connectionString);
    }
}
