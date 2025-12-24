using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Slingcessories.Service.Data;

namespace Slingcessories.Service;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Slingcessories;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
        return new AppDbContext(optionsBuilder.Options);
    }
}
