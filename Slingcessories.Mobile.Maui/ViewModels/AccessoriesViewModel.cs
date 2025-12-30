using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Models;
using Slingcessories.Mobile.Maui.Services;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class AccessoriesViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private List<AccessoryDto> accessories = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public AccessoriesViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadAccessoriesAsync(bool? wishlist = null)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            Accessories = await _apiService.GetAccessoriesAsync(wishlist);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load accessories: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteAccessoryAsync(AccessoryDto accessory)
    {
        try
        {
            await _apiService.DeleteAccessoryAsync(accessory.Id);
            await LoadAccessoriesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete accessory: {ex.Message}";
        }
    }
}

