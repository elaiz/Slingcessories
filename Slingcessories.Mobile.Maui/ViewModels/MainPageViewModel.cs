using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [RelayCommand]
    private async Task NavigateToAccessories()
    {
        await Shell.Current.GoToAsync("//AccessoriesPage");
    }

    [RelayCommand]
    private async Task NavigateToCategories()
    {
        await Shell.Current.GoToAsync("//CategoriesPage");
    }

    [RelayCommand]
    private async Task NavigateToSlingshots()
    {
        await Shell.Current.GoToAsync("//SlingshotsPage");
    }

    [RelayCommand]
    private async Task NavigateToUsers()
    {
        await Shell.Current.GoToAsync("//UsersPage");
    }
}

