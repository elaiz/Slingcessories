using Microsoft.JSInterop;
using System.Text.Json;

namespace Slingcessories.Services;

public class OfflineDataService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<OfflineDataService>? _objectReference;
    private bool _isOnline = true;
    
    public event Action<bool>? OnlineStatusChanged;
    
    public bool IsOnline => _isOnline;

    public OfflineDataService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("indexedDbHelper.init");
            _isOnline = await _jsRuntime.InvokeAsync<bool>("indexedDbHelper.isOnline");
            
            // Set up network status listeners with proper .NET reference
            _objectReference = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("indexedDbHelper.setDotNetHelper", _objectReference);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing offline service: {ex.Message}");
        }
    }

    [JSInvokable]
    public void HandleOnline()
    {
        _isOnline = true;
        OnlineStatusChanged?.Invoke(true);
    }

    [JSInvokable]
    public void HandleOffline()
    {
        _isOnline = false;
        OnlineStatusChanged?.Invoke(false);
    }

    public async Task<T?> GetCachedDataAsync<T>(string key)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("indexedDbHelper.get", key);
            if (string.IsNullOrEmpty(json))
                return default;
            
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting cached data for {key}: {ex.Message}");
            return default;
        }
    }

    public async Task SetCachedDataAsync<T>(string key, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            await _jsRuntime.InvokeVoidAsync("indexedDbHelper.set", key, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting cached data for {key}: {ex.Message}");
        }
    }

    public async Task RemoveCachedDataAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("indexedDbHelper.remove", key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing cached data for {key}: {ex.Message}");
        }
    }

    public async Task ClearAllCacheAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("indexedDbHelper.clear");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing cache: {ex.Message}");
        }
    }

    public async Task<List<string>> GetAllCacheKeysAsync()
    {
        try
        {
            var keys = await _jsRuntime.InvokeAsync<List<string>>("indexedDbHelper.getAllKeys");
            return keys ?? new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting cache keys: {ex.Message}");
            return new List<string>();
        }
    }

    // Pending changes for offline sync
    public async Task AddPendingChangeAsync(string changeType, string entityType, int entityId, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            await _jsRuntime.InvokeVoidAsync("indexedDbHelper.addPendingChange", 
                changeType, entityType, entityId, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding pending change: {ex.Message}");
        }
    }

    public async Task<bool> HasPendingChangesAsync()
    {
        try
        {
            var changes = await _jsRuntime.InvokeAsync<List<object>>("indexedDbHelper.getPendingChanges");
            return changes?.Count > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking pending changes: {ex.Message}");
            return false;
        }
    }

    public async Task ClearPendingChangesAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("indexedDbHelper.clearPendingChanges");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing pending changes: {ex.Message}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        _objectReference?.Dispose();
    }
}
