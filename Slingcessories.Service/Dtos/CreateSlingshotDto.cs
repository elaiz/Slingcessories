namespace Slingcessories.Service.Dtos;

public record CreateSlingshotDto(
    int Year,
    string Model,
    string Color,
    string UserId
);
