using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Models;
using Slingcessories.Mobile.Maui.Services;
using System.Collections.ObjectModel;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly UserStateService _userStateService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private UserDto? _currentUser;

    [ObservableProperty]
    private string _newFirstName = string.Empty;

    [ObservableProperty]
    private string _newLastName = string.Empty;

    [ObservableProperty]
    private string _newEmail = string.Empty;

    public ObservableCollection<UserDto> Users { get; } = new();

    public SettingsViewModel(ApiService apiService, UserStateService userStateService)
    {
        _apiService = apiService;
        _userStateService = userStateService;
    }

    [RelayCommand]
    public async Task LoadUsersAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var items = await _apiService.GetUsersAsync();
            
            Users.Clear();
            foreach (var item in items)
            {
                Users.Add(item);
            }

            // Load current user if set
            if (!string.IsNullOrEmpty(_userStateService.CurrentUserId))
            {
                CurrentUser = Users.FirstOrDefault(u => u.Id == _userStateService.CurrentUserId);
            }
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
    public async Task CreateUserAsync()
    {
        if (string.IsNullOrWhiteSpace(NewFirstName) || string.IsNullOrWhiteSpace(NewLastName))
        {
            ErrorMessage = "First name and last name are required";
            return;
        }

        try
        {
            var dto = new CreateUserDto
            {
                FirstName = NewFirstName.Trim(),
                LastName = NewLastName.Trim(),
                Email = NewEmail.Trim()
            };

            var created = await _apiService.CreateUserAsync(dto);
            if (created != null)
            {
                Users.Add(created);
                NewFirstName = string.Empty;
                NewLastName = string.Empty;
                NewEmail = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error creating user: {ex.Message}";
        }
    }

    [RelayCommand]
    public void SelectUser(UserDto? user)
    {
        if (user == null)
        {
            return;
        }
        
        _userStateService.CurrentUserId = user.Id;
        CurrentUser = user;
    }
}
