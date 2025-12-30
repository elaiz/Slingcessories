using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Models;
using Slingcessories.Mobile.Maui.Services;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class UsersViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private List<UserDto> users = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public UsersViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadUsersAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            Users = await _apiService.GetUsersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load users: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteUserAsync(UserDto user)
    {
        try
        {
            await _apiService.DeleteUserAsync(user.Id);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete user: {ex.Message}";
        }
    }
}

