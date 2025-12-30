using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Models;
using Slingcessories.Mobile.Maui.Services;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class SlingshotsViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private List<SlinghotDto> slingshots = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public SlingshotsViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadSlingshotsAsync(string? userId = null)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            Slingshots = await _apiService.GetSlingshotsAsync(userId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load slingshots: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteSlingshotAsync(SlinghotDto slingshot)
    {
        try
        {
            await _apiService.DeleteSlingshotAsync(slingshot.Id);
            await LoadSlingshotsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete slingshot: {ex.Message}";
        }
    }
}

