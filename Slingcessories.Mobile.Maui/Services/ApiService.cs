using System.Net.Http.Json;
using Slingcessories.Mobile.Maui.Models;

namespace Slingcessories.Mobile.Maui.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // BaseAddress should be configured in MauiProgram.cs
        // For Android emulator, use: http://10.0.2.2:5001
        // For iOS simulator, use: http://localhost:5001
        // For physical device, use your computer's IP address
        
        if (_httpClient.BaseAddress == null)
        {
            throw new InvalidOperationException("HttpClient BaseAddress must be configured in MauiProgram.cs");
        }
    }

    // Accessories
    public async Task<List<AccessoryDto>> GetAccessoriesAsync(bool? wishlist = null)
    {
        var url = wishlist.HasValue 
            ? $"/Accessories?wishlist={wishlist.Value}" 
            : "/Accessories";
        
        var response = await _httpClient.GetFromJsonAsync<List<AccessoryDto>>(url);
        return response ?? new List<AccessoryDto>();
    }

    public async Task<AccessoryDto?> GetAccessoryByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<AccessoryDto>($"/Accessories/{id}");
    }

    public async Task<AccessoryDto> CreateAccessoryAsync(CreateAccessoryDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/Accessories", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AccessoryDto>() 
            ?? throw new Exception("Failed to create accessory");
    }

    public async Task UpdateAccessoryAsync(int id, AccessoryDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/Accessories/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAccessoryAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/Accessories/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Categories
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("/Categories");
        return response ?? new List<CategoryDto>();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<CategoryDto>($"/Categories/{id}");
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/Categories", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryDto>() 
            ?? throw new Exception("Failed to create category");
    }

    public async Task UpdateCategoryAsync(int id, CategoryDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/Categories/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/Categories/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<SubcategoryDto>> GetSubcategoriesByCategoryAsync(int categoryId)
    {
        var response = await _httpClient.GetFromJsonAsync<List<SubcategoryDto>>($"/Categories/{categoryId}/subcategories");
        return response ?? new List<SubcategoryDto>();
    }

    // Subcategories
    public async Task<List<SubcategoryDto>> GetSubcategoriesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<SubcategoryDto>>("/Subcategories");
        return response ?? new List<SubcategoryDto>();
    }

    public async Task<List<SubcategoryDto>> GetSubcategoriesByCategoryIdAsync(int categoryId)
    {
        var response = await _httpClient.GetFromJsonAsync<List<SubcategoryDto>>($"/Subcategories/by-category/{categoryId}");
        return response ?? new List<SubcategoryDto>();
    }

    public async Task<SubcategoryDto> CreateSubcategoryAsync(CreateSubcategoryDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/Subcategories", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SubcategoryDto>() 
            ?? throw new Exception("Failed to create subcategory");
    }

    public async Task UpdateSubcategoryAsync(int id, SubcategoryDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/Subcategories/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSubcategoryAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/Subcategories/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Slingshots
    public async Task<List<SlinghotDto>> GetSlingshotsAsync(string? userId = null)
    {
        var url = !string.IsNullOrEmpty(userId) 
            ? $"/Slingshots?userId={userId}" 
            : "/Slingshots";
        
        var response = await _httpClient.GetFromJsonAsync<List<SlinghotDto>>(url);
        return response ?? new List<SlinghotDto>();
    }

    public async Task<SlinghotDto?> GetSlingshotByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<SlinghotDto>($"/Slingshots/{id}");
    }

    public async Task<SlinghotDto> CreateSlingshotAsync(CreateSlingshotDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/Slingshots", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SlinghotDto>() 
            ?? throw new Exception("Failed to create slingshot");
    }

    public async Task UpdateSlingshotAsync(int id, SlinghotDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/Slingshots/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSlingshotAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/Slingshots/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Users
    public async Task<List<UserDto>> GetUsersAsync()
    {
        try
        {
            // Use relative path without leading slash when BaseAddress ends with /
            var httpResponse = await _httpClient.GetAsync("Users");
            var requestUri = _httpClient.BaseAddress != null 
                ? new Uri(_httpClient.BaseAddress, "Users").ToString()
                : "Users";
            
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Request to {requestUri} failed with status {httpResponse.StatusCode} ({httpResponse.ReasonPhrase}). Response: {errorContent}");
            }
            
            var response = await httpResponse.Content.ReadFromJsonAsync<List<UserDto>>();
            return response ?? new List<UserDto>();
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var baseUrl = _httpClient.BaseAddress?.ToString() ?? "unknown";
            throw new HttpRequestException($"Failed to get users from {baseUrl}Users: {ex.Message}", ex);
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<UserDto>($"/Users/{id}");
    }

    public async Task<UserDto> RegisterUserAsync(CreateUserDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/Users/register", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() 
            ?? throw new Exception("Failed to register user");
    }

    public async Task UpdateUserAsync(string id, UserDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/Users/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUserAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"/Users/{id}");
        response.EnsureSuccessStatusCode();
    }
}

