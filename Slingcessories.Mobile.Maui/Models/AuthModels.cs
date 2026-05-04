namespace Slingcessories.Mobile.Maui.Models;

public record AuthLoginRequestDto(string Email, string Password, string ClientId);

public record AuthRegisterRequestDto(string FirstName, string LastName, string Email, string Password, string ClientId);

public record ForgotPasswordRequestDto(string Email);

public record ResetPasswordRequestDto(string Email, string Token, string NewPassword);

public record ForgotPasswordResponseDto(string Message, string? ResetToken);

public record AuthResponseDto(string Token, DateTime ExpiresAtUtc, string UserId, string Email, string FirstName, string LastName);
