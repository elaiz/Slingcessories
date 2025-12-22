using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingsessory.service.Data;
using Slingsessory.service.Dtos;
using Slingsessory.service.Models;

namespace Slingsessory.service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var results = await db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync();

        return Ok(results);
    }

    [HttpGet("{categoryId:int}/subcategories")]
    public async Task<ActionResult<IEnumerable<SubcategoryDto>>> GetSubcategories(int categoryId)
    {
        var exists = await db.Categories.AnyAsync(c => c.Id == categoryId);
        if (!exists) return NotFound();

        var results = await db.Subcategories
            .Where(sc => sc.CategoryId == categoryId)
            .OrderBy(sc => sc.Name)
            .Select(sc => new SubcategoryDto(sc.Id, sc.Name, sc.CategoryId))
            .ToListAsync();

        return Ok(results);
    }

    public record CategoryDto(int Id, string Name);
}
