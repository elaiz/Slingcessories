using Microsoft.JSInterop;
using System.Text.Json;

namespace Slingcessories.Services;

/// <summary>
/// Service for persisting page UI state (view modes, filters, etc.) using browser localStorage.
/// This provides a lightweight cache-based state management solution.
/// </summary>
public class PageStateService
{
    private readonly IJSRuntime _jsRuntime;
    private const string StateKeyPrefix = "pageState_";

    public PageStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Save a state value for a specific page/component.
    /// </summary>
    /// <typeparam name="T">Type of the state value</typeparam>
    /// <param name="pageKey">Unique identifier for the page (e.g., "accessories_view")</param>
    /// <param name="value">The value to persist</param>
    public async Task SaveStateAsync<T>(string pageKey, T value)
    {
        try
        {
            var key = $"{StateKeyPrefix}{pageKey}";
            var json = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch
        {
            // Silently handle localStorage errors (e.g., during prerendering or if disabled)
        }
    }

    /// <summary>
    /// Load a previously saved state value for a specific page/component.
    /// </summary>
    /// <typeparam name="T">Type of the state value</typeparam>
    /// <param name="pageKey">Unique identifier for the page</param>
    /// <param name="defaultValue">Default value if no state is found</param>
    /// <returns>The saved state or default value</returns>
    public async Task<T> LoadStateAsync<T>(string pageKey, T defaultValue)
    {
        try
        {
            var key = $"{StateKeyPrefix}{pageKey}";
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
            
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<T>(json) ?? defaultValue;
            }
        }
        catch
        {
            // Return default on error
        }

        return defaultValue;
    }

    /// <summary>
    /// Clear a specific page state.
    /// </summary>
    /// <param name="pageKey">Unique identifier for the page</param>
    public async Task ClearStateAsync(string pageKey)
    {
        try
        {
            var key = $"{StateKeyPrefix}{pageKey}";
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch
        {
            // Silently handle errors
        }
    }

    /// <summary>
    /// Clear all page states.
    /// </summary>
    public async Task ClearAllStatesAsync()
    {
        try
        {
            // Get all localStorage keys and remove those with our prefix
            var allKeys = await _jsRuntime.InvokeAsync<string[]>("eval", 
                $"Object.keys(localStorage).filter(k => k.startsWith('{StateKeyPrefix}'))");
            
            foreach (var key in allKeys)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            }
        }
        catch
        {
            // Silently handle errors
        }
    }
}
