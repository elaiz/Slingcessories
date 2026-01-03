using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Data;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.GraphQL;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Accessory> GetAccessories([Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        var context = contextFactory.CreateDbContext();
        return context.Accessories;
    }

    public async Task<Accessory?> GetAccessoryById(
        int id,
        [Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        return await context.Accessories
            .Include(a => a.Category)
            .Include(a => a.Subcategory)
            .Include(a => a.AccessorySlingshots)
                .ThenInclude(a_s => a_s.Slingshot)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<AccessoryGroupByCategory>> GetAccessoriesGroupedByCategory(
        [Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        return await context.Accessories
            .Include(a => a.Category)
            .GroupBy(a => new { a.CategoryId, a.Category!.Name })
            .Select(g => new AccessoryGroupByCategory
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Count = g.Count(),
                Accessories = g.ToList()
            })
            .ToListAsync();
    }

    public async Task<List<AccessoryGroupBySubcategory>> GetAccessoriesGroupedBySubcategory(
        [Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        return await context.Accessories
            .Include(a => a.Subcategory)
            .Where(a => a.SubcategoryId != null)
            .GroupBy(a => new { a.SubcategoryId, a.Subcategory!.Name })
            .Select(g => new AccessoryGroupBySubcategory
            {
                SubcategoryId = g.Key.SubcategoryId!.Value,
                SubcategoryName = g.Key.Name,
                Count = g.Count(),
                Accessories = g.ToList()
            })
            .ToListAsync();
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Slingshot> GetSlingshots([Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        var context = contextFactory.CreateDbContext();
        return context.Slingshots;
    }

    public async Task<Slingshot?> GetSlingshotById(
        int id,
        [Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        return await context.Slingshots
            .Include(s => s.AccessorySlingshots)
                .ThenInclude(a_s => a_s.Accessory)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Category> GetCategories([Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        var context = contextFactory.CreateDbContext();
        return context.Categories;
    }

    public async Task<Category?> GetCategoryById(
        int id,
        [Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        return await context.Categories
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Subcategory> GetSubcategories([Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        var context = contextFactory.CreateDbContext();
        return context.Subcategories;
    }

    public async Task<Subcategory?> GetSubcategoryById(
        int id,
        [Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        return await context.Subcategories
            .Include(sc => sc.Category)
            .Include(sc => sc.Accessories)
            .FirstOrDefaultAsync(sc => sc.Id == id);
    }
}

public class AccessoryGroupByCategory
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<Accessory> Accessories { get; set; } = new();
}

public class AccessoryGroupBySubcategory
{
    public int SubcategoryId { get; set; }
    public string SubcategoryName { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<Accessory> Accessories { get; set; } = new();
}
