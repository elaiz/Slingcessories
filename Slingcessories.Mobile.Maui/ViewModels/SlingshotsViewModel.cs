using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Models;
using Slingcessories.Mobile.Maui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class SlingshotsViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly UserStateService _userStateService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string _newYear = DateTime.Now.Year.ToString();

    [ObservableProperty]
    private string _newModel = string.Empty;

    [ObservableProperty]
    private string _newColor = string.Empty;

    public ObservableCollection<SlinghotDto> Slingshots { get; } = new();

    public SlingshotsViewModel(ApiService apiService, UserStateService userStateService)
    {
        _apiService = apiService;
        _userStateService = userStateService;
        Debug.WriteLine("SlingshotsViewModel created");
    }

    [RelayCommand]
    public async Task LoadSlingshotsAsync()
    {
        try
        {
            Debug.WriteLine($"LoadSlingshotsAsync started. UserId={_userStateService.CurrentUserId}");
            IsLoading = true;
            ErrorMessage = null;

            var items = await _apiService.GetSlingshotsAsync(_userStateService.CurrentUserId);
            
            Debug.WriteLine($"Received {items.Count} slingshots from API");
            
            Slingshots.Clear();
            foreach (var item in items)
            {
                Debug.WriteLine($"Adding slingshot: {item.Year} {item.Model} ({item.Color})");
                Slingshots.Add(item);
            }
            
            Debug.WriteLine($"Slingshots collection now has {Slingshots.Count} items");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading slingshots: {ex}");
            ErrorMessage = $"Failed to load slingshots: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            Debug.WriteLine($"LoadSlingshotsAsync completed. IsLoading={IsLoading}");
        }
    }

    [RelayCommand]
    public async Task CreateSlingshotAsync()
    {
        if (string.IsNullOrWhiteSpace(NewModel) || string.IsNullOrWhiteSpace(NewColor))
        {
            ErrorMessage = "Model and Color are required";
            return;
        }

        if (string.IsNullOrEmpty(_userStateService.CurrentUserId))
        {
            ErrorMessage = "Please select a user first";
            return;
        }

        try
        {
            Debug.WriteLine($"Creating slingshot: {NewYear} {NewModel} ({NewColor})");
            
            var dto = new CreateSlingshotDto
            {
                Year = int.Parse(NewYear),
                Model = NewModel.Trim(),
                Color = NewColor.Trim(),
                UserId = _userStateService.CurrentUserId
            };

            var created = await _apiService.CreateSlingshotAsync(dto);
            if (created != null)
            {
                Debug.WriteLine($"Slingshot created successfully: {created.Id}");
                Slingshots.Add(created);
                NewModel = string.Empty;
                NewColor = string.Empty;
                NewYear = DateTime.Now.Year.ToString();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating slingshot: {ex}");
            ErrorMessage = $"Error creating slingshot: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task DeleteSlingshotAsync(int id)
    {
        try
        {
            Debug.WriteLine($"Deleting slingshot {id}");
            var success = await _apiService.DeleteSlingshotAsync(id);
            if (success)
            {
                var item = Slingshots.FirstOrDefault(s => s.Id == id);
                if (item != null)
                {
                    Slingshots.Remove(item);
                    Debug.WriteLine($"Removed slingshot {id}");
                }
            }
            else
            {
                ErrorMessage = "Failed to delete slingshot";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting slingshot: {ex}");
            ErrorMessage = $"Error deleting slingshot: {ex.Message}";
        }
    }
}
