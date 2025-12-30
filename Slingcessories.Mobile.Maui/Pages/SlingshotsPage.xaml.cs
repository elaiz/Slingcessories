using Slingcessories.Mobile.Maui.ViewModels;

namespace Slingcessories.Mobile.Maui.Pages;

public partial class SlingshotsPage : ContentPage
{
    public SlingshotsPage(SlingshotsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is SlingshotsViewModel vm)
        {
            await vm.LoadSlingshotsCommand.ExecuteAsync(null);
        }
    }
}

