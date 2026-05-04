using Microsoft.Extensions.DependencyInjection;
using Slingcessories.Mobile.Maui.Services;

namespace Slingcessories.Mobile.Maui;

public partial class AppShell : Shell
{
    private readonly UserStateService? _userStateService;

    public AppShell()
    {
        InitializeComponent();

        _userStateService = IPlatformApplication.Current?.Services.GetService<UserStateService>();
        if (_userStateService != null)
        {
            _userStateService.OnUserChanged += OnUserChanged;
            UpdateFlyoutBehavior();
        }
    }

    private void OnUserChanged()
    {
        MainThread.BeginInvokeOnMainThread(UpdateFlyoutBehavior);
    }

    private void UpdateFlyoutBehavior()
    {
        var isLoggedIn = !string.IsNullOrWhiteSpace(_userStateService?.CurrentUserId);
        FlyoutBehavior = isLoggedIn ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;
    }

    protected override void OnDisappearing()
    {
        if (_userStateService != null)
        {
            _userStateService.OnUserChanged -= OnUserChanged;
        }

        base.OnDisappearing();
    }
}
