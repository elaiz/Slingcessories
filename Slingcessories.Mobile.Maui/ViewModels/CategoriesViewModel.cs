using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Models;
using Slingcessories.Mobile.Maui.Services;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private List<CategoryDto> categories = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public CategoriesViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadCategoriesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            Categories = await _apiService.GetCategoriesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load categories: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteCategoryAsync(CategoryDto category)
    {
        try
        {
            await _apiService.DeleteCategoryAsync(category.Id);
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete category: {ex.Message}";
        }
    }
}

