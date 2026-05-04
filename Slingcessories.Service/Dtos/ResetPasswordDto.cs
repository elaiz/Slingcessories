namespace Slingcessories.Service.Dtos;

public record ResetPasswordDto(
    string Email,
    string Token,
    string NewPassword
);
