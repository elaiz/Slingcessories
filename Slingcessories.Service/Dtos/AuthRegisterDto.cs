namespace Slingcessories.Service.Dtos;

public record AuthRegisterDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ClientId
);
