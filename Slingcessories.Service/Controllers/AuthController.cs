using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Slingcessories.Service.Data;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<IdentityUser> userManager,
    IConfiguration configuration,
    IWebHostEnvironment environment,
    AppDbContext db) : ControllerBase
{
    private static readonly string[] AllowedClientIds =
    [
        "Slingcessories.Blazor",
        "Slingcessories.Maui",
        "Slingcessories.React"
    ];

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] AuthRegisterDto dto)
    {
        if (!IsValidClientId(dto.ClientId))
        {
            return BadRequest("Invalid client identifier.");
        }

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        if (await userManager.FindByEmailAsync(normalizedEmail) is not null)
        {
            return BadRequest("A user with this email already exists.");
        }

        var identityUser = new IdentityUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail
        };

        var createResult = await userManager.CreateAsync(identityUser, dto.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(createResult.Errors.Select(e => e.Description));
        }

        var profile = new User
        {
            Id = identityUser.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = normalizedEmail
        };

        db.Users.Add(profile);
        await db.SaveChangesAsync();

        return Ok(CreateAuthResponse(identityUser, profile, dto.ClientId));
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var identityUser = await userManager.FindByEmailAsync(normalizedEmail);

        // Always return success to avoid email enumeration
        if (identityUser is null)
        {
            return Ok(new { message = "If the email exists, a password reset token has been generated." });
        }

        await EnsureIdentityStampsAsync(identityUser);

        var token = await userManager.GeneratePasswordResetTokenAsync(identityUser);

        if (environment.IsDevelopment())
        {
            return Ok(new
            {
                message = "Password reset token generated.",
                resetToken = token
            });
        }

        return Ok(new { message = "If the email exists, a password reset token has been generated." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var identityUser = await userManager.FindByEmailAsync(normalizedEmail);

        if (identityUser is null)
        {
            return BadRequest("Invalid reset request.");
        }

        await EnsureIdentityStampsAsync(identityUser);

        var resetResult = await userManager.ResetPasswordAsync(identityUser, dto.Token, dto.NewPassword);
        if (!resetResult.Succeeded)
        {
            return BadRequest(resetResult.Errors.Select(e => e.Description));
        }

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] AuthLoginDto dto)
    {
        if (!IsValidClientId(dto.ClientId))
        {
            return BadRequest("Invalid client identifier.");
        }

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var identityUser = await userManager.FindByEmailAsync(normalizedEmail);
        if (identityUser is null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var passwordValid = await userManager.CheckPasswordAsync(identityUser, dto.Password);
        if (!passwordValid)
        {
            return Unauthorized("Invalid email or password.");
        }

        var profile = await db.Users.FindAsync(identityUser.Id);
        if (profile is null)
        {
            profile = new User
            {
                Id = identityUser.Id,
                FirstName = string.Empty,
                LastName = string.Empty,
                Email = identityUser.Email ?? normalizedEmail
            };

            db.Users.Add(profile);
            await db.SaveChangesAsync();
        }

        return Ok(CreateAuthResponse(identityUser, profile, dto.ClientId));
    }

    private AuthResponseDto CreateAuthResponse(IdentityUser identityUser, User profile, string clientId)
    {
        var jwt = configuration.GetSection("Jwt");
        var expiresMinutes = int.TryParse(jwt["ExpiresMinutes"], out var m) ? m : 60;
        var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identityUser.Id),
            new(JwtRegisteredClaimNames.Email, identityUser.Email ?? profile.Email),
            new(ClaimTypes.NameIdentifier, identityUser.Id),
            new(ClaimTypes.Email, identityUser.Email ?? profile.Email),
            new(ClaimTypes.Name, $"{profile.FirstName} {profile.LastName}".Trim())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: clientId,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expires,
            identityUser.Id,
            identityUser.Email ?? profile.Email,
            profile.FirstName,
            profile.LastName
        );
    }

    private async Task EnsureIdentityStampsAsync(IdentityUser identityUser)
    {
        if (!string.IsNullOrWhiteSpace(identityUser.SecurityStamp) &&
            !string.IsNullOrWhiteSpace(identityUser.ConcurrencyStamp))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(identityUser.SecurityStamp))
        {
            var securityStampResult = await userManager.UpdateSecurityStampAsync(identityUser);
            if (!securityStampResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to initialize user security stamp.");
            }
        }

        if (string.IsNullOrWhiteSpace(identityUser.ConcurrencyStamp))
        {
            identityUser.ConcurrencyStamp = Guid.NewGuid().ToString();
            var concurrencyStampResult = await userManager.UpdateAsync(identityUser);
            if (!concurrencyStampResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to initialize user concurrency stamp.");
            }
        }
    }

    private static bool IsValidClientId(string? clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            return false;
        }

        return AllowedClientIds.Contains(clientId, StringComparer.Ordinal);
    }
}
