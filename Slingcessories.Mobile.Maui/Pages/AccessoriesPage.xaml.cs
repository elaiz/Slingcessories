using Slingcessories.Mobile.Maui.ViewModels;

namespace Slingcessories.Mobile.Maui.Pages;

public partial class AccessoriesPage : ContentPage
{
    public AccessoriesPage(AccessoriesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AccessoriesViewModel vm)
        {
            await vm.LoadAccessoriesCommand.ExecuteAsync(null);
        }
    }
}

