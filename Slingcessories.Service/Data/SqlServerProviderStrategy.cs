using Microsoft.EntityFrameworkCore;

namespace Slingcessories.Service.Data;

public class SqlServerProviderStrategy : IDatabaseProviderStrategy
{
    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseSqlServer(connectionString);
    }
}
