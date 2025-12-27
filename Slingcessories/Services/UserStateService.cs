using Microsoft.JSInterop;
using System.Text.Json;

namespace Slingcessories.Services;

public class UserStateService
{
    private readonly IJSRuntime _jsRuntime;
    private string? _currentUserId;
    private const string StorageKey = "currentUserId";

    public event Action? OnUserChanged;

    public UserStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? CurrentUserId => _currentUserId;

    public async Task InitializeAsync()
    {
        try
        {
            var storedUserId = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(storedUserId))
            {
                _currentUserId = storedUserId;
            }
        }
        catch
        {
            // localStorage might not be available during prerendering
        }
    }

    public async Task SetCurrentUserAsync(string userId)
    {
        _currentUserId = userId;
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, userId);
            OnUserChanged?.Invoke();
        }
        catch
        {
            // Handle errors silently
        }
    }

    public async Task ClearCurrentUserAsync()
    {
        _currentUserId = null;
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            OnUserChanged?.Invoke();
        }
        catch
        {
            // Handle errors silently
        }
    }
}
