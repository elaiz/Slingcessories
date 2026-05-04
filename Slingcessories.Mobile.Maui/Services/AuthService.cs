using System.Net.Http.Headers;
using System.Net.Http.Json;
using Slingcessories.Mobile.Maui.Models;

namespace Slingcessories.Mobile.Maui.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    private const string TokenPreferenceKey = "AuthToken";
    private const string ClientId = "Slingcessories.Maui";

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        var token = Preferences.Get(TokenPreferenceKey, null);
        SetAuthorizationHeader(token);
    }

    public async Task<AuthResponseDto?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login", new AuthLoginRequestDto(email, password, ClientId));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (payload is null)
        {
            return null;
        }

        SetToken(payload.Token);
        return payload;
    }

    public async Task<AuthResponseDto?> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/register", new AuthRegisterRequestDto(firstName, lastName, email, password, ClientId));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (payload is null)
        {
            return null;
        }

        SetToken(payload.Token);
        return payload;
    }

    public async Task<string?> ForgotPasswordAsync(string email)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/forgot-password", new ForgotPasswordRequestDto(email));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<ForgotPasswordResponseDto>();
        return payload?.ResetToken;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/reset-password", new ResetPasswordRequestDto(email, token, newPassword));
        return response.IsSuccessStatusCode;
    }

    public void Logout()
    {
        Preferences.Remove(TokenPreferenceKey);
        SetAuthorizationHeader(null);
    }

    private void SetToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            Preferences.Remove(TokenPreferenceKey);
            SetAuthorizationHeader(null);
            return;
        }

        Preferences.Set(TokenPreferenceKey, token);
        SetAuthorizationHeader(token);
    }

    private void SetAuthorizationHeader(string? token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
    }
}
