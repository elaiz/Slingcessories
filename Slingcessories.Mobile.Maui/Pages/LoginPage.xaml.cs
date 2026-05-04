using Slingcessories.Mobile.Maui.ViewModels;

namespace Slingcessories.Mobile.Maui.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
