using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Data;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SlingshotsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SlinghotDto>>> GetAll()
    {
        var results = await db.Slingshots
            .OrderBy(s => s.Year)
            .ThenBy(s => s.Model)
            .Select(s => new SlinghotDto(s.Id, s.Year, s.Model, s.Color))
            .ToListAsync();

        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SlinghotDto>> GetById(int id)
    {
        var slingshot = await db.Slingshots.FindAsync(id);
        if (slingshot == null) return NotFound();

        return Ok(new SlinghotDto(slingshot.Id, slingshot.Year, slingshot.Model, slingshot.Color));
    }

    [HttpPost]
    public async Task<ActionResult<SlinghotDto>> Create([FromBody] CreateSlingshotDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Model))
            return BadRequest("Model is required.");
        
        if (string.IsNullOrWhiteSpace(dto.Color))
            return BadRequest("Color is required.");

        var slingshot = new Slingshot 
        { 
            Year = dto.Year, 
            Model = dto.Model, 
            Color = dto.Color 
        };
        
        db.Slingshots.Add(slingshot);
        
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("A slingshot with this Year, Model, and Color already exists.");
        }

        var result = new SlinghotDto(slingshot.Id, slingshot.Year, slingshot.Model, slingshot.Color);
        return CreatedAtAction(nameof(GetById), new { id = slingshot.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SlinghotDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch.");

        var slingshot = await db.Slingshots.FindAsync(id);
        if (slingshot == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Model))
            return BadRequest("Model is required.");
        
        if (string.IsNullOrWhiteSpace(dto.Color))
            return BadRequest("Color is required.");

        slingshot.Year = dto.Year;
        slingshot.Model = dto.Model;
        slingshot.Color = dto.Color;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Problem(title: "Failed to update slingshot", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var slingshot = await db.Slingshots.FindAsync(id);
        if (slingshot == null) return NotFound();

        // Check if slingshot has accessories
        var hasAccessories = await db.AccessorySlingshots.AnyAsync(as_s => as_s.SlingshotId == id);
        if (hasAccessories)
            return BadRequest("Cannot delete slingshot with existing accessories.");

        db.Slingshots.Remove(slingshot);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
