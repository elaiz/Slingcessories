using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Data;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.Controllers;

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
            .Include(a => a.AccessorySlingshots)
                .ThenInclude(as_s => as_s.Slingshot)
            .OrderBy(a => a.Title)
            .ToListAsync();

        return Ok(results.Select(a => new AccessoryDto(
            a.Id,
            a.Title,
            a.PictureUrl,
            a.Price,
            a.Url,
            a.Wishlist,
            a.CategoryId,
            a.SubcategoryId,
            a.Category!.Name,
            a.Subcategory?.Name,
            a.AccessorySlingshots.Select(as_s => as_s.SlingshotId).ToList(),
            a.AccessorySlingshots.Select(as_s => $"{as_s.Slingshot.Year} {as_s.Slingshot.Model} ({as_s.Slingshot.Color})").ToList(),
            a.AccessorySlingshots.ToDictionary(as_s => as_s.SlingshotId, as_s => as_s.Quantity)
        )));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccessoryDto>> GetById(int id)
    {
        var a = await db.Accessories
            .Include(x => x.Category)
            .Include(x => x.Subcategory)
            .Include(x => x.AccessorySlingshots)
                .ThenInclude(as_s => as_s.Slingshot)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (a is null) return NotFound();
        
        return new AccessoryDto(
            a.Id, 
            a.Title, 
            a.PictureUrl, 
            a.Price, 
            a.Url, 
            a.Wishlist, 
            a.CategoryId, 
            a.SubcategoryId, 
            a.Category!.Name, 
            a.Subcategory?.Name,
            a.AccessorySlingshots.Select(as_s => as_s.SlingshotId).ToList(),
            a.AccessorySlingshots.Select(as_s => $"{as_s.Slingshot.Year} {as_s.Slingshot.Model} ({as_s.Slingshot.Color})").ToList(),
            a.AccessorySlingshots.ToDictionary(as_s => as_s.SlingshotId, as_s => as_s.Quantity)
        );
    }

    [HttpPost]
    public async Task<ActionResult<AccessoryDto>> Create(CreateAccessoryDto dto)
    {
        var entity = new Accessory
        {
            Title = dto.Title,
            PictureUrl = dto.PictureUrl,
            Price = dto.Price,
            Url = dto.Url,
            Wishlist = dto.Wishlist,
            CategoryId = dto.CategoryId,
            SubcategoryId = dto.SubcategoryId
        };
        
        db.Accessories.Add(entity);
        await db.SaveChangesAsync();

        // Add slingshot relationships with quantities
        if (dto.SlinghotQuantities != null && dto.SlinghotQuantities.Any())
        {
            foreach (var (slingshotId, quantity) in dto.SlinghotQuantities)
            {
                db.AccessorySlingshots.Add(new AccessorySlingshot
                {
                    AccessoryId = entity.Id,
                    SlingshotId = slingshotId,
                    Quantity = quantity
                });
            }
            await db.SaveChangesAsync();
        }
        
        var withNav = await db.Accessories
            .Include(x => x.Category)
            .Include(x => x.Subcategory)
            .Include(x => x.AccessorySlingshots)
                .ThenInclude(as_s => as_s.Slingshot)
            .FirstAsync(x => x.Id == entity.Id);
        
        var result = new AccessoryDto(
            withNav.Id, 
            withNav.Title, 
            withNav.PictureUrl, 
            withNav.Price, 
            withNav.Url, 
            withNav.Wishlist, 
            withNav.CategoryId, 
            withNav.SubcategoryId, 
            withNav.Category!.Name, 
            withNav.Subcategory?.Name,
            withNav.AccessorySlingshots.Select(as_s => as_s.SlingshotId).ToList(),
            withNav.AccessorySlingshots.Select(as_s => $"{as_s.Slingshot.Year} {as_s.Slingshot.Model} ({as_s.Slingshot.Color})").ToList(),
            withNav.AccessorySlingshots.ToDictionary(as_s => as_s.SlingshotId, as_s => as_s.Quantity)
        );
        
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, AccessoryDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        var entity = await db.Accessories
            .Include(a => a.AccessorySlingshots)
            .FirstOrDefaultAsync(a => a.Id == id);
            
        if (entity is null) return NotFound();

        entity.Title = dto.Title;
        entity.PictureUrl = dto.PictureUrl;
        entity.Price = dto.Price;
        entity.Url = dto.Url;
        entity.Wishlist = dto.Wishlist;
        entity.CategoryId = dto.CategoryId;
        entity.SubcategoryId = dto.SubcategoryId;

        // Update slingshot relationships with quantities
        // Remove existing relationships
        entity.AccessorySlingshots.Clear();
        
        // Add new relationships with quantities
        if (dto.SlinghotQuantities != null && dto.SlinghotQuantities.Any())
        {
            foreach (var (slingshotId, quantity) in dto.SlinghotQuantities)
            {
                entity.AccessorySlingshots.Add(new AccessorySlingshot
                {
                    AccessoryId = entity.Id,
                    SlingshotId = slingshotId,
                    Quantity = quantity
                });
            }
        }

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
