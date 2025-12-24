using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Data;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.GraphQL;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Accessory> GetAccessories([Service] AppDbContext context)
    {
        return context.Accessories;
    }

    public async Task<Accessory?> GetAccessoryById(
        int id,
        [Service] AppDbContext context)
    {
        return await context.Accessories
            .Include(a => a.Category)
            .Include(a => a.Subcategory)
            .Include(a => a.AccessorySlingshots)
                .ThenInclude(a_s => a_s.Slingshot)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Slingshot> GetSlingshots([Service] AppDbContext context)
    {
        return context.Slingshots;
    }

    public async Task<Slingshot?> GetSlingshotById(
        int id,
        [Service] AppDbContext context)
    {
        return await context.Slingshots
            .Include(s => s.AccessorySlingshots)
                .ThenInclude(a_s => a_s.Accessory)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Category> GetCategories([Service] AppDbContext context)
    {
        return context.Categories;
    }

    public async Task<Category?> GetCategoryById(
        int id,
        [Service] AppDbContext context)
    {
        return await context.Categories
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Subcategory> GetSubcategories([Service] AppDbContext context)
    {
        return context.Subcategories;
    }

    public async Task<Subcategory?> GetSubcategoryById(
        int id,
        [Service] AppDbContext context)
    {
        return await context.Subcategories
            .Include(sc => sc.Category)
            .Include(sc => sc.Accessories)
            .FirstOrDefaultAsync(sc => sc.Id == id);
    }
}
