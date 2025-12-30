using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Data;

namespace Slingcessories.Tests;

public static class TestHelpers
{
    public static AppDbContext CreateInMemoryDbContext(string databaseName = "")
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            databaseName = Guid.NewGuid().ToString();
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new AppDbContext(options);
    }
}
