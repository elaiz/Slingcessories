using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.GraphQL.Types;

public class AccessoryType : ObjectType<Accessory>
{
    protected override void Configure(IObjectTypeDescriptor<Accessory> descriptor)
    {
        descriptor
            .Field(a => a.AccessorySlingshots)
            .Ignore();

        descriptor
            .Field("slingshots")
            .ResolveWith<AccessoryResolvers>(r => r.GetSlingshots(default!, default!))
            .Description("Gets the slingshots associated with this accessory.");
    }
}

public class AccessoryResolvers
{
    public async Task<List<Slingshot>> GetSlingshots(
        [Parent] Accessory accessory,
        [Service] IDbContextFactory<Data.AppDbContext> contextFactory)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        return await context.Slingshots
            .Where(s => s.AccessorySlingshots.Any(a_s => a_s.AccessoryId == accessory.Id))
            .ToListAsync();
    }
}
