using Microsoft.Extensions.Logging;
using Slingcessories.Mobile.Maui.Services;
using Slingcessories.Mobile.Maui.ViewModels;
using Slingcessories.Mobile.Maui.Pages;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

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

		// Register HttpClient and Services
		builder.Services.AddHttpClient<ApiService>(client =>
		{
			client.BaseAddress = new Uri("https://localhost:7289/api/");
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
		builder.Services.AddSingleton<ApiService>();

		// Register ViewModels
		builder.Services.AddTransient<MainPageViewModel>();
		builder.Services.AddTransient<AccessoriesViewModel>();
		builder.Services.AddTransient<AccessoryDetailViewModel>();
		builder.Services.AddTransient<CategoriesViewModel>();
		builder.Services.AddTransient<SlingshotsViewModel>();
		builder.Services.AddTransient<UsersViewModel>();

		// Register Pages
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<AccessoriesPage>();
		builder.Services.AddTransient<CategoriesPage>();
		builder.Services.AddTransient<SlingshotsPage>();
		builder.Services.AddTransient<UsersPage>();

		return builder.Build();
	}
}

