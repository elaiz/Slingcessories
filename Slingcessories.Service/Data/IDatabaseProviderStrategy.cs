using Microsoft.EntityFrameworkCore;

namespace Slingcessories.Service.Data;

public interface IDatabaseProviderStrategy
{
    void Configure(DbContextOptionsBuilder options, string connectionString);
}
