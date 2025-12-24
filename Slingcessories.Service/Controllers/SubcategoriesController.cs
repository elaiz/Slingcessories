using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Data;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubcategoriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubcategoryDto>>> GetAll()
        => Ok(await db.Subcategories
            .OrderBy(sc => sc.Name)
            .Select(sc => new SubcategoryDto(sc.Id, sc.Name, sc.CategoryId))
            .ToListAsync());

    [HttpGet("by-category/{categoryId:int}")]
    public async Task<ActionResult<IEnumerable<SubcategoryDto>>> GetByCategory(int categoryId)
        => Ok(await db.Subcategories
            .Where(sc => sc.CategoryId == categoryId)
            .OrderBy(sc => sc.Name)
            .Select(sc => new SubcategoryDto(sc.Id, sc.Name, sc.CategoryId))
            .ToListAsync());

    [HttpPost]
    public async Task<ActionResult<SubcategoryDto>> Create(CreateSubcategoryDto dto)
    {
        var entity = new Subcategory { Name = dto.Name, CategoryId = dto.CategoryId };
        db.Subcategories.Add(entity);
        await db.SaveChangesAsync();
        var result = new SubcategoryDto(entity.Id, entity.Name, entity.CategoryId);
        return CreatedAtAction(nameof(GetByCategory), new { categoryId = entity.CategoryId }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SubcategoryDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        var entity = await db.Subcategories.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        entity.CategoryId = dto.CategoryId;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Problem(title: "Failed to update subcategory", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Subcategories.FindAsync(id);
        if (entity is null) return NotFound();

        db.Subcategories.Remove(entity);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Deleting a subcategory sets FK to null on accessories, but other constraints could still apply
            return Problem(title: "Failed to delete subcategory", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
        return NoContent();
    }
}
