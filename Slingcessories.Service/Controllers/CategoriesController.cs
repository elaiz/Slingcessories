using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Data;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.Controllers;

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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        return Ok(new CategoryDto(category.Id, category.Name));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required.");

        var category = new Category 
        { 
            Name = dto.Name
        };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var result = new CategoryDto(category.Id, category.Name);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch.");

        var category = await db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required.");

        category.Name = dto.Name;
        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        // Check if category has subcategories
        var hasSubcategories = await db.Subcategories.AnyAsync(sc => sc.CategoryId == id);
        if (hasSubcategories)
            return BadRequest("Cannot delete category with existing subcategories.");

        // Check if category has accessories
        var hasAccessories = await db.Accessories.AnyAsync(a => a.CategoryId == id);
        if (hasAccessories)
            return BadRequest("Cannot delete category with existing accessories.");

        db.Categories.Remove(category);
        await db.SaveChangesAsync();

        return NoContent();
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
