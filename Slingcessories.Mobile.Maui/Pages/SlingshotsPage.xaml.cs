using Slingcessories.Mobile.Maui.ViewModels;

namespace Slingcessories.Mobile.Maui.Pages;

public partial class SlingshotsPage : ContentPage
{
    private readonly SlingshotsViewModel _viewModel;

    public SlingshotsPage(SlingshotsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSlingshotsCommand.ExecuteAsync(null);
    }
}
