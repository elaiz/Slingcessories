using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Data;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await db.Users
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync();

        return Ok(users.Select(u => new UserDto(
            u.Id,
            u.FirstName,
            u.LastName,
            u.Email
        )));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var user = await db.Users.FindAsync(id);
        
        if (user is null) return NotFound();
        
        return new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email
        );
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(CreateUserDto dto)
    {
        // Check if email already exists
        if (await db.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest("A user with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
        
        db.Users.Add(user);
        
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Problem(title: "Failed to register user", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
        
        var result = new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email
        );
        
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UserDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Problem(title: "Failed to update user", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();

        db.Users.Remove(user);
        
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Problem(title: "Failed to delete user", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
        
        return NoContent();
    }
}
