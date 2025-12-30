using System.Net.Http.Json;
using Slingcessories.Mobile.Maui.Models;
using System.Diagnostics;

namespace Slingcessories.Mobile.Maui.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        Debug.WriteLine($"ApiService created with BaseAddress: {_httpClient.BaseAddress}");
    }

    // Users
    public async Task<List<UserDto>> GetUsersAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<UserDto>>("users");
        return result ?? new List<UserDto>();
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<UserDto>($"users/{id}");
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("users", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    // Categories
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("categories");
        return result ?? new List<CategoryDto>();
    }

    // Get ALL subcategories at once (Blazor approach - more efficient)
    public async Task<List<SubcategoryDto>> GetAllSubcategoriesAsync()
    {
        Debug.WriteLine("=== GetAllSubcategoriesAsync ===");
        Debug.WriteLine($"URL: subcategories");
        
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<SubcategoryDto>>("subcategories");
            Debug.WriteLine($"Received {result?.Count ?? 0} subcategories");
            return result ?? new List<SubcategoryDto>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetAllSubcategoriesAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<List<SubcategoryDto>> GetSubcategoriesAsync(int categoryId)
    {
        var url = $"categories/{categoryId}/subcategories";
        Debug.WriteLine($"=== GetSubcategoriesAsync ===");
        Debug.WriteLine($"Category ID: {categoryId}");
        Debug.WriteLine($"Relative URL: {url}");
        Debug.WriteLine($"Full URL: {_httpClient.BaseAddress}{url}");
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            Debug.WriteLine($"Response Status: {response.StatusCode}");
            Debug.WriteLine($"Response Success: {response.IsSuccessStatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error Response: {errorContent}");
                return new List<SubcategoryDto>();
            }
            
            var contentString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Response Content Length: {contentString.Length}");
            Debug.WriteLine($"Response Content: {contentString}");
            
            var result = await response.Content.ReadFromJsonAsync<List<SubcategoryDto>>();
            Debug.WriteLine($"Deserialized {result?.Count ?? 0} subcategories");
            
            return result ?? new List<SubcategoryDto>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetSubcategoriesAsync: {ex.Message}");
            Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }

    // Slingshots
    public async Task<List<SlinghotDto>> GetSlingshotsAsync(string? userId = null)
    {
        var url = string.IsNullOrEmpty(userId) ? "slingshots" : $"slingshots?userId={Uri.EscapeDataString(userId)}";
        var result = await _httpClient.GetFromJsonAsync<List<SlinghotDto>>(url);
        return result ?? new List<SlinghotDto>();
    }

    public async Task<SlinghotDto?> CreateSlingshotAsync(CreateSlingshotDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("slingshots", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SlinghotDto>();
    }

    public async Task<bool> UpdateSlingshotAsync(int id, SlinghotDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"slingshots/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSlingshotAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"slingshots/{id}");
        return response.IsSuccessStatusCode;
    }

    // Accessories
    public async Task<List<AccessoryDto>> GetAccessoriesAsync(bool? wishlist = null)
    {
        var url = wishlist.HasValue ? $"accessories?wishlist={wishlist.Value.ToString().ToLower()}" : "accessories";
        var result = await _httpClient.GetFromJsonAsync<List<AccessoryDto>>(url);
        return result ?? new List<AccessoryDto>();
    }

    public async Task<AccessoryDto?> GetAccessoryByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<AccessoryDto>($"accessories/{id}");
    }

    public async Task<AccessoryDto?> CreateAccessoryAsync(CreateAccessoryDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("accessories", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AccessoryDto>();
    }

    public async Task<bool> UpdateAccessoryAsync(int id, AccessoryDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"accessories/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAccessoryAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"accessories/{id}");
        return response.IsSuccessStatusCode;
    }
}
