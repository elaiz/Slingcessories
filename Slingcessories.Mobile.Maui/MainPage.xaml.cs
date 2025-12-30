using Slingcessories.Mobile.Maui.ViewModels;

namespace Slingcessories.Mobile.Maui;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
