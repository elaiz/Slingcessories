namespace Slingcessories.Service.Dtos;

public record AuthLoginDto(
    string Email,
    string Password,
    string ClientId
);
