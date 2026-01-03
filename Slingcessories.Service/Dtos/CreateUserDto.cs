namespace Slingcessories.Service.Dtos;

public record CreateUserDto(
    string FirstName,
    string LastName,
    string Email
);
