# Offline Mode Implementation for Blazor App

## Overview
This implementation provides comprehensive offline support for the Slingcessories Blazor WebAssembly application. Users can now view cached data when offline, and the app provides clear visual feedback about connectivity status.

## What Was Implemented

### 1. Enhanced IndexedDB Helper (`wwwroot/js/indexedDb.js`)
**Features:**
- ? Key-value storage for cached data
- ? Pending changes queue for offline sync (future use)
- ? Network status detection (online/offline)
- ? Event listeners for network changes
- ? Cache management (get, set, remove, clear)
- ? Timestamp tracking for cache entries

**New Methods:**
- `set(key, value)` - Store data with timestamp
- `get(key)` - Retrieve cached data
- `remove(key)` - Delete specific cache entry
- `clear()` - Clear all cached data
- `getAllKeys()` - Get all cache keys
- `addPendingChange()` - Track offline changes
- `getPendingChanges()` - Retrieve queued changes
- `clearPendingChanges()` - Clear sync queue
- `setDotNetHelper()` - Store .NET object reference for callbacks

### 2. Service Worker (`wwwroot/service-worker.published.js`)
**Caching Strategy:**
- ?? **Network-first** for API calls (with cache fallback)
- ?? **Cache-first** for static assets
- ?? Automatic cache updates when online
- ?? Graceful degradation when offline

**Benefits:**
- Faster page loads (cached assets)
- API data available offline
- Automatic background updates
- Progressive Web App (PWA) capabilities

### 3. OfflineDataService (`Services/OfflineDataService.cs`)
**Purpose:** Centralized cache management and offline state handling

**Features:**
- Generic cache methods for any data type
- Network status monitoring
- Event-driven architecture
- Pending changes tracking
- JSON serialization/deserialization
- Proper disposal with IAsyncDisposable

**Key Methods:**
```csharp
Task<T?> GetCachedDataAsync<T>(string key)
Task SetCachedDataAsync<T>(string key, T data)
Task RemoveCachedDataAsync(string key)
Task ClearAllCacheAsync()
bool IsOnline { get; }
event Action<bool>? OnlineStatusChanged
```

### 4. Offline Indicator Component (`Components/OfflineIndicator.razor`)
**Features:**
- ?? Visual notification when offline
- ? Shows pending changes badge
- ?? Animated appearance
- ?? Sticky positioning at top of page

**UI/UX:**
- Only shows when offline
- Yellow warning color scheme
- Persistent across page navigation
- Clear messaging about limited functionality

### 5. Styling (`wwwroot/css/app.css`)
**Added:**
- Offline indicator styles
- Slide-down animation
- Pulse animation for pending changes badge
- Responsive design

## How It Works

### Data Flow
```
1. User Online:
   ?? Fetch from API
   ?? Cache response in IndexedDB
   ?? Display data
   ?? Service worker caches API responses

2. User Goes Offline:
   ?? Network detection triggers
   ?? Offline indicator appears
   ?? API calls fail gracefully
   ?? Data loaded from IndexedDB cache

3. User Returns Online:
   ?? Network detection triggers
   ?? Offline indicator disappears
   ?? Fresh data fetched from API
   ?? Cache updated automatically
```

### Pages with Full Offline Support

#### ? Home Page (`Pages/Home.razor`)
- Caches user list
- Loads from cache when offline
- Disables registration button when offline
- Shows offline warnings
- Auto-refreshes on reconnection

#### ? AccessoryList Component (`Components/AccessoryList.razor`)
- Caches accessories data (owned & wishlist)
- Loads from cache when offline
- Prevents create/edit/delete when offline
- Shows offline warnings in modals
- User-specific caching per wishlist type

#### ? Slingshots Page (`Pages/Slingshots.razor`)
- Caches slingshots per user
- Caches all accessories for associations
- Loads from cache when offline
- Disables all write operations when offline
- Shows info banner when viewing cached data
- Auto-refreshes on reconnection

#### ? Categories Page (`Pages/Categories.razor`)
- Caches categories and subcategories
- Loads from cache when offline
- Disables create/edit/delete when offline
- Shows info banner when viewing cached data
- Auto-refreshes on reconnection

### Implementation Pattern

All pages follow this consistent pattern:

```csharp
@inject OfflineDataService OfflineDataService

protected override async Task OnInitializedAsync()
{
    OfflineDataService.OnlineStatusChanged += HandleNetworkChange;
    await LoadData();
}

private async Task LoadData()
{
    if (OfflineDataService.IsOnline)
    {
        // Fetch from API
        var data = await Http.GetFromJsonAsync<DataType>(url);
        
        // Cache the data
        await OfflineDataService.SetCachedDataAsync("cache-key", data);
    }
    else
    {
        // Load from cache
        data = await OfflineDataService.GetCachedDataAsync<DataType>("cache-key");
    }
}

private void HandleNetworkChange(bool isOnline)
{
    InvokeAsync(async () =>
    {
        if (isOnline)
        {
            await LoadData(); // Refresh when back online
        }
        StateHasChanged();
    });
}

public void Dispose()
{
    OfflineDataService.OnlineStatusChanged -= HandleNetworkChange;
}
```

## Usage in Other Pages

To add offline support to additional pages:

### 1. Inject OfflineDataService
```csharp
@inject OfflineDataService OfflineDataService
```

### 2. Check Online Status
```csharp
if (OfflineDataService.IsOnline)
{
    // Fetch from API and cache
}
else
{
    // Load from cache
}
```

### 3. Use Generic Cache Methods
```csharp
// Save to cache
await OfflineDataService.SetCachedDataAsync("my-key", myData);

// Load from cache
var data = await OfflineDataService.GetCachedDataAsync<MyDataType>("my-key");
```

### 4. Listen for Network Changes
```csharp
protected override void OnInitialized()
{
    OfflineDataService.OnlineStatusChanged += HandleNetworkChange;
}

private void HandleNetworkChange(bool isOnline)
{
    InvokeAsync(async () => 
    {
        if (isOnline)
        {
            await RefreshData(); // Reload fresh data
        }
        StateHasChanged();
    });
}

public void Dispose()
{
    OfflineDataService.OnlineStatusChanged -= HandleNetworkChange;
}
```

### 5. Disable Write Operations
```html
<button @onclick="CreateItem" disabled="@(!OfflineDataService.IsOnline)">
    Create
</button>
```

### 6. Show Offline Info Banner
```html
@if (!OfflineDataService.IsOnline)
{
    <div class="alert alert-info">
        <i class="bi bi-info-circle"></i> You're viewing cached data. 
        Write operations are disabled while offline.
    </div>
}
```

## Future Enhancements

### Potential Improvements:
1. **Offline Sync Queue**
   - Store create/update/delete operations while offline
   - Auto-sync when connection restored
   - Conflict resolution strategy

2. **Cache Expiration**
   - Set TTL (time-to-live) for cached data
   - Automatic cache invalidation
   - Manual refresh controls

3. **Storage Management**
   - Cache size monitoring
   - Automatic cleanup of old data
   - User-configurable cache settings

4. **Enhanced User Feedback**
   - Sync progress indicator
   - Last updated timestamps
   - Data freshness indicators

5. **Offline-First CRUD**
   - Allow edits while offline
   - Queue operations for sync
   - Optimistic UI updates

## Testing Offline Mode

### In Browser DevTools:
1. Open DevTools (F12)
2. Go to **Network** tab
3. Select **Offline** from throttling dropdown
4. Navigate the app to see offline behavior

### Testing Checklist:
```
? Home page loads cached users
? Accessories page loads cached items
? Slingshots page loads cached data
? Categories page loads cached data
? Offline banner appears when offline
? Write operations disabled when offline
? Data refreshes when back online
? Offline banner disappears when online
? No console errors when offline
? IndexedDB contains cached data
```

### Using Service Worker:
1. Build in Release mode
2. Publish the app
3. Serve from a web server
4. Disable network to test

## Technical Notes

### Browser Compatibility
- ? Chrome/Edge (full support)
- ? Firefox (full support)
- ? Safari (full support)
- ? Mobile browsers (iOS/Android)

### Storage Limits
- IndexedDB: ~50MB-1GB (browser dependent)
- Service Worker Cache: Similar limits
- Automatically managed by browser

### Security Considerations
- Cache only public/user-specific data
- Don't cache sensitive credentials
- HTTPS required for service workers in production

### Cache Keys Used
- `users_list` - All registered users
- `accessories_false` - Owned accessories
- `accessories_true` - Wishlist accessories
- `slingshots_{userId}` - User-specific slingshots
- `accessories_all` - All accessories (for slingshot associations)
- `categories_all` - All categories
- `subcategories_all` - All subcategories

## Files Modified/Created

### New Files:
- `Slingcessories/Services/OfflineDataService.cs`
- `Slingcessories/Components/OfflineIndicator.razor`
- `OFFLINE_MODE_IMPLEMENTATION.md` (this file)

### Modified Files:
- `Slingcessories/wwwroot/js/indexedDb.js` - Enhanced with proper callbacks
- `Slingcessories/wwwroot/service-worker.published.js` - Implemented caching
- `Slingcessories/wwwroot/css/app.css` - Added offline indicator styles
- `Slingcessories/Program.cs` - Registered OfflineDataService
- `Slingcessories/Layout/MainLayout.razor` - Added OfflineIndicator component
- `Slingcessories/Pages/Home.razor` - ? Added offline support
- `Slingcessories/Pages/Slingshots.razor` - ? Added offline support
- `Slingcessories/Pages/Categories.razor` - ? Added offline support
- `Slingcessories/Components/AccessoryList.razor` - Already had offline support

## Summary

Your Blazor app now has:
- ? **Complete offline support** across all major pages
- ? Robust offline data caching with IndexedDB
- ? Visual offline indicators
- ? Network status detection with auto-refresh
- ? Service worker for PWA capabilities
- ? Extensible architecture for future enhancements
- ? Consistent patterns across all pages
- ? Graceful degradation when offline

**All major data pages now work offline!**

Users can:
- ? Browse all cached data when offline
- ? See clear visual feedback about offline status
- ? Have write operations automatically disabled for safety
- ? Automatically refresh data when reconnecting
- ? Continue using the app seamlessly during network interruptions

The app is now a true **Progressive Web App (PWA)** with robust offline capabilities! ??
