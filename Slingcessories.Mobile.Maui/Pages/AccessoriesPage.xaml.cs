using Slingcessories.Mobile.Maui.ViewModels;

namespace Slingcessories.Mobile.Maui.Pages;

public partial class AccessoriesPage : ContentPage
{
    private readonly AccessoriesViewModel _viewModel;

    public AccessoriesPage(AccessoriesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAccessoriesCommand.ExecuteAsync(null);
    }
}
