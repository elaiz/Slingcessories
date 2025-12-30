using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Models;
using Slingcessories.Mobile.Maui.Services;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class AccessoryDetailViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private AccessoryDto? accessory;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public AccessoryDetailViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadAccessoryAsync(int id)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            Accessory = await _apiService.GetAccessoryByIdAsync(id);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load accessory: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}

