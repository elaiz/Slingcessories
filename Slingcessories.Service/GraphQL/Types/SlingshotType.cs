using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.GraphQL.Types;

public class SlingshotType : ObjectType<Slingshot>
{
    protected override void Configure(IObjectTypeDescriptor<Slingshot> descriptor)
    {
        descriptor
            .Field(s => s.AccessorySlingshots)
            .Ignore();

        descriptor
            .Field("accessories")
            .ResolveWith<SlingshotResolvers>(r => r.GetAccessories(default!, default!))
            .Description("Gets the accessories associated with this slingshot.");
    }
}

public class SlingshotResolvers
{
    public async Task<List<Accessory>> GetAccessories(
        [Parent] Slingshot slingshot,
        [Service] Data.AppDbContext context)
    {
        return await context.Accessories
            .Where(a => a.AccessorySlingshots.Any(a_s => a_s.SlingshotId == slingshot.Id))
            .ToListAsync();
    }
}
