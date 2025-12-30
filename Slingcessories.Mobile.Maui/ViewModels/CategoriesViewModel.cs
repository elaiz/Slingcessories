using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Models;
using Slingcessories.Mobile.Maui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private List<SubcategoryDto> _allSubcategories = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    public ObservableCollection<CategoryWithSubcategories> Categories { get; } = new();

    public CategoriesViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Debug.WriteLine("CategoriesViewModel created");
    }

    [RelayCommand]
    public async Task LoadCategoriesAsync()
    {
        try
        {
            Debug.WriteLine("LoadCategoriesAsync started");
            IsLoading = true;
            ErrorMessage = null;

            // Load categories
            var categories = await _apiService.GetCategoriesAsync();
            Debug.WriteLine($"Received {categories.Count} categories from API");
            
            // Load ALL subcategories upfront (Blazor approach)
            _allSubcategories = await _apiService.GetAllSubcategoriesAsync();
            Debug.WriteLine($"Received {_allSubcategories.Count} subcategories from API");
            
            Categories.Clear();
            foreach (var category in categories)
            {
                Debug.WriteLine($"Adding category: {category.Name}");
                var categoryWithSubs = new CategoryWithSubcategories
                {
                    Id = category.Id,
                    Name = category.Name
                };
                
                // Pre-populate subcategories for this category
                var categorySubs = _allSubcategories.Where(s => s.CategoryId == category.Id).ToList();
                foreach (var sub in categorySubs)
                {
                    categoryWithSubs.Subcategories.Add(sub);
                }
                Debug.WriteLine($"  Category {category.Name} has {categoryWithSubs.Subcategories.Count} subcategories");
                
                Categories.Add(categoryWithSubs);
            }
            
            Debug.WriteLine($"Categories collection now has {Categories.Count} items");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading categories: {ex}");
            ErrorMessage = $"Failed to load categories: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            Debug.WriteLine($"LoadCategoriesAsync completed. IsLoading={IsLoading}");
        }
    }

    [RelayCommand]
    public void ToggleCategory(CategoryWithSubcategories? category)
    {
        if (category is null)
        {
            Debug.WriteLine("=== ToggleCategory called with null category ===");
            return;
        }
        
        Debug.WriteLine($"=== ToggleCategory START ===");
        Debug.WriteLine($"Category: {category.Name} (ID={category.Id})");
        Debug.WriteLine($"Current IsExpanded: {category.IsExpanded}");
        Debug.WriteLine($"Subcategories Count: {category.Subcategories.Count}");
        
        category.IsExpanded = !category.IsExpanded;
        Debug.WriteLine($"New IsExpanded: {category.IsExpanded}");
        
        Debug.WriteLine($"=== ToggleCategory END ===");
    }
}

public class CategoryWithSubcategories : ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (SetProperty(ref _isExpanded, value))
            {
                Debug.WriteLine($"CategoryWithSubcategories.IsExpanded changed to {value} for {Name}");
            }
        }
    }

    public ObservableCollection<SubcategoryDto> Subcategories { get; } = new();
}
