using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingsessory.service.Data;
using Slingsessory.service.Dtos;
using Slingsessory.service.Models;

namespace Slingsessory.service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccessoriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessoryDto>>> GetAll([FromQuery] bool? wishlist)
    {
        var query = db.Accessories.AsQueryable();
        if (wishlist is not null)
        {
            query = query.Where(a => a.Wishlist == wishlist);
        }

        var results = await query
            .Include(a => a.Category)
            .Include(a => a.Subcategory)
            .OrderBy(a => a.Title)
            .Select(a => new AccessoryDto(
                a.Id,
                a.Title,
                a.PictureUrl,
                a.Units,
                a.Price,
                a.Url,
                a.Wishlist,
                a.CategoryId,
                a.SubcategoryId,
                a.Category!.Name,
                a.Subcategory != null ? a.Subcategory.Name : null
            ))
            .ToListAsync();

        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccessoryDto>> GetById(int id)
    {
        var a = await db.Accessories.Include(x => x.Category).Include(x => x.Subcategory).FirstOrDefaultAsync(x => x.Id == id);
        if (a is null) return NotFound();
        return new AccessoryDto(a.Id, a.Title, a.PictureUrl, a.Units, a.Price, a.Url, a.Wishlist, a.CategoryId, a.SubcategoryId, a.Category!.Name, a.Subcategory?.Name);
    }

    [HttpPost]
    public async Task<ActionResult<AccessoryDto>> Create(CreateAccessoryDto dto)
    {
        var entity = new Accessory
        {
            Title = dto.Title,
            PictureUrl = dto.PictureUrl,
            Units = dto.Units,
            Price = dto.Price,
            Url = dto.Url,
            Wishlist = dto.Wishlist,
            CategoryId = dto.CategoryId,
            SubcategoryId = dto.SubcategoryId
        };
        db.Accessories.Add(entity);
        await db.SaveChangesAsync();
        var withNav = await db.Accessories.Include(x => x.Category).Include(x => x.Subcategory).FirstAsync(x => x.Id == entity.Id);
        var result = new AccessoryDto(withNav.Id, withNav.Title, withNav.PictureUrl, withNav.Units, withNav.Price, withNav.Url, withNav.Wishlist, withNav.CategoryId, withNav.SubcategoryId, withNav.Category!.Name, withNav.Subcategory?.Name);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, AccessoryDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        var entity = await db.Accessories.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Title = dto.Title;
        entity.PictureUrl = dto.PictureUrl;
        entity.Units = dto.Units;
        entity.Price = dto.Price;
        entity.Url = dto.Url;
        entity.Wishlist = dto.Wishlist;
        entity.CategoryId = dto.CategoryId;
        entity.SubcategoryId = dto.SubcategoryId;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Problem(title: "Failed to update accessory", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Accessories.FindAsync(id);
        if (entity is null) return NotFound();

        db.Accessories.Remove(entity);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Problem(title: "Failed to delete accessory", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
        return NoContent();
    }
}
