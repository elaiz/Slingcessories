namespace Slingcessories.Service.Dtos;

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAtUtc,
    string UserId,
    string Email,
    string FirstName,
    string LastName
);
