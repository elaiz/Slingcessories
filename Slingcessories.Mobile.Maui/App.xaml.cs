using Microsoft.Extensions.DependencyInjection;
using Slingcessories.Mobile.Maui.Services;

namespace Slingcessories.Mobile.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
      var shell = new AppShell();

		var userStateService = IPlatformApplication.Current?.Services.GetService<UserStateService>();
		if (string.IsNullOrWhiteSpace(userStateService?.CurrentUserId))
		{
			MainThread.BeginInvokeOnMainThread(async () => await shell.GoToAsync("//LoginPage"));
		}

		return new Window(shell);
	}
}