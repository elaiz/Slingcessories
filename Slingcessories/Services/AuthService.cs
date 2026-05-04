using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace Slingcessories.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _jsRuntime;

    private const string TokenStorageKey = "authToken";
    private const string ClientId = "Slingcessories.Blazor";

    public AuthService(HttpClient http, IJSRuntime jsRuntime)
    {
        _http = http;
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenStorageKey);
            SetAuthorizationHeader(token);
        }
        catch
        {
            // Ignore localStorage issues
        }
    }

    public async Task<AuthResponseDto?> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new AuthLoginRequestDto(email, password, ClientId));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (result is null)
        {
            return null;
        }

        await SetTokenAsync(result.Token);
        return result;
    }

    public async Task<AuthResponseDto?> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", new AuthRegisterRequestDto(firstName, lastName, email, password, ClientId));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (result is null)
        {
            return null;
        }

        await SetTokenAsync(result.Token);
        return result;
    }

    public async Task<string?> ForgotPasswordAsync(string email)
    {
        var response = await _http.PostAsJsonAsync("api/auth/forgot-password", new ForgotPasswordRequestDto(email));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<ForgotPasswordResponseDto>();
        return payload?.ResetToken;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var response = await _http.PostAsJsonAsync("api/auth/reset-password", new ResetPasswordRequestDto(email, token, newPassword));
        return response.IsSuccessStatusCode;
    }

    public async Task LogoutAsync()
    {
        await SetTokenAsync(null);
    }

    private async Task SetTokenAsync(string? token)
    {
        SetAuthorizationHeader(token);

        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenStorageKey);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenStorageKey, token);
            }
        }
        catch
        {
            // Ignore localStorage issues
        }
    }

    private void SetAuthorizationHeader(string? token)
    {
        _http.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
    }
}

public record AuthLoginRequestDto(string Email, string Password, string ClientId);
public record AuthRegisterRequestDto(string FirstName, string LastName, string Email, string Password, string ClientId);
public record ForgotPasswordRequestDto(string Email);
public record ResetPasswordRequestDto(string Email, string Token, string NewPassword);
public record ForgotPasswordResponseDto(string Message, string? ResetToken);
public record AuthResponseDto(string Token, DateTime ExpiresAtUtc, string UserId, string Email, string FirstName, string LastName);
