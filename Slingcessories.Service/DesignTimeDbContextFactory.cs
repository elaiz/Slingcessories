using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Slingcessories.Service.Data;

namespace Slingcessories.Service;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var dbProviderName = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";
        var dbProvider = Enum.Parse<DatabaseProvider>(dbProviderName);
        var connectionString = configuration.GetConnectionString(dbProviderName)
            ?? throw new InvalidOperationException($"Connection string '{dbProviderName}' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var strategy = DatabaseProviderFactory.Create(dbProvider);
        strategy.Configure(optionsBuilder, connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
