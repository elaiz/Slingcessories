using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Slingcessories.Mobile.Maui.Services;
using Slingcessories.Mobile.Maui.ViewModels;
using Slingcessories.Mobile.Maui.Pages;
using System.Net.Security;

namespace Slingcessories.Mobile.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Load configuration from appsettings.json
		var assembly = typeof(MauiProgram).Assembly;
		using var stream = assembly.GetManifestResourceStream("Slingcessories.Mobile.Maui.appsettings.json");
		if (stream != null)
		{
			var config = new ConfigurationBuilder()
				.AddJsonStream(stream)
				.Build();
			builder.Configuration.AddConfiguration(config);
		}

		// Get API base URL from configuration
		var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] 
			?? "https://localhost:7289/api/";

		// Register UserStateService as singleton (shared across app)
		builder.Services.AddSingleton<UserStateService>();

		// Register HttpClient and ApiService
		builder.Services.AddHttpClient<ApiService>(client =>
		{
			client.BaseAddress = new Uri(apiBaseUrl);
		})
			.ConfigurePrimaryHttpMessageHandler(() =>
			{
				var handler = new HttpClientHandler();
#if DEBUG
				// Bypass SSL certificate validation for localhost in development
				handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
				{
					if (message.RequestUri?.Host == "localhost" || message.RequestUri?.Host == "127.0.0.1")
						return true;
					return errors == SslPolicyErrors.None;
				};
#endif
				return handler;
			});

		// Register ViewModels
		builder.Services.AddTransient<AccessoriesViewModel>();
		builder.Services.AddTransient<CategoriesViewModel>();
		builder.Services.AddTransient<SlingshotsViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		// Register Pages
		builder.Services.AddTransient<AccessoriesPage>();
		builder.Services.AddTransient<CategoriesPage>();
		builder.Services.AddTransient<SlingshotsPage>();
		builder.Services.AddTransient<SettingsPage>();

		return builder.Build();
	}
}

