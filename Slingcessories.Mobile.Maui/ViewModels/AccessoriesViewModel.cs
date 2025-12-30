using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Models;
using Slingcessories.Mobile.Maui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class AccessoriesViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly UserStateService _userStateService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isWishlist;

    partial void OnIsWishlistChanged(bool value)
    {
        // Automatically reload when wishlist toggle changes
        _ = LoadAccessoriesCommand.ExecuteAsync(null);
    }

    [ObservableProperty]
    private string? _errorMessage;

    public ObservableCollection<AccessoryDto> Accessories { get; } = new();

    public AccessoriesViewModel(ApiService apiService, UserStateService userStateService)
    {
        _apiService = apiService;
        _userStateService = userStateService;
        Debug.WriteLine("AccessoriesViewModel created");
    }

    [RelayCommand]
    public async Task LoadAccessoriesAsync()
    {
        try
        {
            Debug.WriteLine($"LoadAccessoriesAsync started. IsWishlist={IsWishlist}");
            IsLoading = true;
            ErrorMessage = null;

            var items = await _apiService.GetAccessoriesAsync(IsWishlist);
            
            Debug.WriteLine($"Received {items.Count} accessories from API");
            
            Accessories.Clear();
            foreach (var item in items)
            {
                Debug.WriteLine($"Adding accessory: {item.Title}");
                Accessories.Add(item);
            }
            
            Debug.WriteLine($"Accessories collection now has {Accessories.Count} items");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading accessories: {ex}");
            ErrorMessage = $"Failed to load accessories: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            Debug.WriteLine($"LoadAccessoriesAsync completed. IsLoading={IsLoading}");
        }
    }

    [RelayCommand]
    public async Task DeleteAccessoryAsync(int id)
    {
        try
        {
            Debug.WriteLine($"Deleting accessory {id}");
            var success = await _apiService.DeleteAccessoryAsync(id);
            if (success)
            {
                var item = Accessories.FirstOrDefault(a => a.Id == id);
                if (item != null)
                {
                    Accessories.Remove(item);
                    Debug.WriteLine($"Removed accessory {id}");
                }
            }
            else
            {
                ErrorMessage = "Failed to delete accessory";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting accessory: {ex}");
            ErrorMessage = $"Error deleting accessory: {ex.Message}";
        }
    }
}
