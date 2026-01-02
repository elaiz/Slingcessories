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

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private bool _isCreating;

    [ObservableProperty]
    private string _newCategoryName = string.Empty;

    [ObservableProperty]
    private int? _addingSubcategoryCategoryId;

    [ObservableProperty]
    private string _newSubcategoryName = string.Empty;

    [ObservableProperty]
    private int? _confirmDeleteSubcategoryId;

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
                    categoryWithSubs.Subcategories.Add(new EditableSubcategory(sub.Id, sub.Name, sub.CategoryId));
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

    [RelayCommand]
    public void BeginEditCategory(CategoryWithSubcategories? category)
    {
        if (category is null || IsSaving) return;

        Debug.WriteLine($"Beginning edit for category: {category.Name}");
        
        // Cancel any other category edits
        foreach (var cat in Categories.Where(c => c.Id != category.Id))
        {
            cat.IsEditing = false;
        }
        
        // Cancel all subcategory edits
        foreach (var cat in Categories)
        {
            foreach (var sub in cat.Subcategories)
            {
                sub.IsEditing = false;
            }
        }

        category.EditName = category.Name;
        category.IsEditing = true;
    }

    [RelayCommand]
    public void CancelEditCategory(CategoryWithSubcategories? category)
    {
        if (category is null) return;

        Debug.WriteLine($"Canceling edit for category: {category.Name}");
        category.IsEditing = false;
        category.EditName = string.Empty;
    }

    [RelayCommand]
    public async Task SaveCategoryAsync(CategoryWithSubcategories? category)
    {
        if (category is null || string.IsNullOrWhiteSpace(category.EditName)) return;

        try
        {
            IsSaving = true;
            ErrorMessage = null;

            Debug.WriteLine($"Saving category {category.Id}: {category.EditName}");
            
            var dto = new CategoryDto(category.Id, category.EditName.Trim());
            var success = await _apiService.UpdateCategoryAsync(category.Id, dto);

            if (success)
            {
                category.Name = category.EditName.Trim();
                category.IsEditing = false;
                category.EditName = string.Empty;
                Debug.WriteLine($"Category {category.Id} updated successfully");
            }
            else
            {
                ErrorMessage = "Failed to update category";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating category: {ex}");
            ErrorMessage = $"Error updating category: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    public void BeginEditSubcategory(EditableSubcategory? subcategory)
    {
        if (subcategory is null || IsSaving) return;

        Debug.WriteLine($"Beginning edit for subcategory: {subcategory.Name}");
        
        // Cancel any category edits
        foreach (var cat in Categories)
        {
            cat.IsEditing = false;
        }

        // Cancel other subcategory edits
        foreach (var cat in Categories)
        {
            foreach (var sub in cat.Subcategories)
            {
                if (sub.Id != subcategory.Id)
                {
                    sub.IsEditing = false;
                }
            }
        }

        subcategory.EditName = subcategory.Name;
        subcategory.IsEditing = true;
    }

    [RelayCommand]
    public void CancelEditSubcategory(EditableSubcategory? subcategory)
    {
        if (subcategory is null) return;

        Debug.WriteLine($"Canceling edit for subcategory: {subcategory.Name}");
        subcategory.IsEditing = false;
        subcategory.EditName = string.Empty;
    }

    [RelayCommand]
    public async Task SaveSubcategoryAsync(EditableSubcategory? subcategory)
    {
        if (subcategory is null || string.IsNullOrWhiteSpace(subcategory.EditName)) return;

        try
        {
            IsSaving = true;
            ErrorMessage = null;

            Debug.WriteLine($"Saving subcategory {subcategory.Id}: {subcategory.EditName}");
            
            var dto = new SubcategoryDto(subcategory.Id, subcategory.EditName.Trim(), subcategory.CategoryId);
            var success = await _apiService.UpdateSubcategoryAsync(subcategory.Id, dto);

            if (success)
            {
                subcategory.Name = subcategory.EditName.Trim();
                subcategory.IsEditing = false;
                subcategory.EditName = string.Empty;
                Debug.WriteLine($"Subcategory {subcategory.Id} updated successfully");
            }
            else
            {
                ErrorMessage = "Failed to update subcategory";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating subcategory: {ex}");
            ErrorMessage = $"Error updating subcategory: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    public async Task DeleteSubcategoryAsync(EditableSubcategory? subcategory)
    {
        if (subcategory is null) return;

        try
        {
            IsSaving = true;
            ErrorMessage = null;

            Debug.WriteLine($"Deleting subcategory {subcategory.Id}: {subcategory.Name}");
            
            var success = await _apiService.DeleteSubcategoryAsync(subcategory.Id);

            if (success)
            {
                // Find the category that contains this subcategory and remove it
                foreach (var category in Categories)
                {
                    var subToRemove = category.Subcategories.FirstOrDefault(s => s.Id == subcategory.Id);
                    if (subToRemove != null)
                    {
                        category.Subcategories.Remove(subToRemove);
                        Debug.WriteLine($"Subcategory {subcategory.Id} deleted successfully from category {category.Name}");
                        break;
                    }
                }
                
                // Clear confirmation state
                ConfirmDeleteSubcategoryId = null;
            }
            else
            {
                ErrorMessage = "Failed to delete subcategory";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting subcategory: {ex}");
            ErrorMessage = $"Error deleting subcategory: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    public void BeginDeleteSubcategory(EditableSubcategory? subcategory)
    {
        if (subcategory is null) return;
        ConfirmDeleteSubcategoryId = subcategory.Id;
    }

    [RelayCommand]
    public void CancelDeleteSubcategory()
    {
        ConfirmDeleteSubcategoryId = null;
    }

    [RelayCommand]
    public void BeginCreateCategory()
    {
        if (IsCreating || IsSaving) return;

        Debug.WriteLine("Beginning create category");
        
        // Cancel any edits
        foreach (var cat in Categories)
        {
            cat.IsEditing = false;
            foreach (var sub in cat.Subcategories)
            {
                sub.IsEditing = false;
            }
        }

        NewCategoryName = string.Empty;
        IsCreating = true;
    }

    [RelayCommand]
    public void CancelCreateCategory()
    {
        Debug.WriteLine("Canceling create category");
        IsCreating = false;
        NewCategoryName = string.Empty;
    }

    [RelayCommand]
    public async Task SaveNewCategoryAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName)) return;

        try
        {
            IsSaving = true;
            ErrorMessage = null;

            Debug.WriteLine($"Creating new category: {NewCategoryName}");
            
            var dto = new CreateCategoryDto(NewCategoryName.Trim());
            var created = await _apiService.CreateCategoryAsync(dto);

            if (created != null)
            {
                var newCategory = new CategoryWithSubcategories
                {
                    Id = created.Id,
                    Name = created.Name
                };
                
                Categories.Add(newCategory);
                
                // Sort categories by name
                var sorted = Categories.OrderBy(c => c.Name).ToList();
                Categories.Clear();
                foreach (var cat in sorted)
                {
                    Categories.Add(cat);
                }
                
                IsCreating = false;
                NewCategoryName = string.Empty;
                Debug.WriteLine($"Category created successfully: {created.Name}");
            }
            else
            {
                ErrorMessage = "Failed to create category";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating category: {ex}");
            ErrorMessage = $"Error creating category: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    public void BeginAddSubcategory(CategoryWithSubcategories? category)
    {
        if (category is null || IsSaving) return;
        AddingSubcategoryCategoryId = category.Id;
        NewSubcategoryName = string.Empty;
    }

    [RelayCommand]
    public void CancelAddSubcategory()
    {
        AddingSubcategoryCategoryId = null;
        NewSubcategoryName = string.Empty;
    }

    [RelayCommand]
    public async Task SaveNewSubcategoryAsync(CategoryWithSubcategories? category)
    {
        if (category is null || string.IsNullOrWhiteSpace(NewSubcategoryName)) return;
        try
        {
            IsSaving = true;
            ErrorMessage = null;
            var dto = new CreateSubcategoryDto(NewSubcategoryName.Trim(), category.Id);
            var created = await _apiService.CreateSubcategoryAsync(dto);
            if (created != null)
            {
                category.Subcategories.Add(new EditableSubcategory(created.Id, created.Name, created.CategoryId));
                // Sort subcategories by name
                var sorted = category.Subcategories.OrderBy(s => s.Name).ToList();
                category.Subcategories.Clear();
                foreach (var sub in sorted)
                    category.Subcategories.Add(sub);
                AddingSubcategoryCategoryId = null;
                NewSubcategoryName = string.Empty;
            }
            else
            {
                ErrorMessage = "Failed to create subcategory";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error creating subcategory: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }
}

public partial class CategoryWithSubcategories : ObservableObject
{
    public int Id { get; set; }
    
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _editName = string.Empty;

    public ObservableCollection<EditableSubcategory> Subcategories { get; } = new();
}

public partial class EditableSubcategory : ObservableObject
{
    public int Id { get; set; }
    
    [ObservableProperty]
    private string _name = string.Empty;
    
    public int CategoryId { get; set; }

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _editName = string.Empty;

    public EditableSubcategory(int id, string name, int categoryId)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
    }
}
