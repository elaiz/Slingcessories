using System.Net.Http.Json;

namespace Slingcessories.Services;

public class UserService
{
    private readonly HttpClient _http;

    public UserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _http.GetFromJsonAsync<List<UserDto>>("api/users");
        return users ?? new List<UserDto>();
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        return await _http.GetFromJsonAsync<UserDto>($"api/users/{id}");
    }

    public async Task<UserDto?> RegisterUserAsync(CreateUserDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/users/register", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }
        return null;
    }
}

public record UserDto(string Id, string FirstName, string LastName, string Email);
public record CreateUserDto(string FirstName, string LastName, string Email);
