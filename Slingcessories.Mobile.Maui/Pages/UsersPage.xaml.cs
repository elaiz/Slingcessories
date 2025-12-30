using Slingcessories.Mobile.Maui.ViewModels;

namespace Slingcessories.Mobile.Maui.Pages;

public partial class UsersPage : ContentPage
{
    public UsersPage(UsersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is UsersViewModel vm)
        {
            await vm.LoadUsersCommand.ExecuteAsync(null);
        }
    }
}

