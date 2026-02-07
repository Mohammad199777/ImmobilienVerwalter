using ImmobilienVerwalter.Maui.Services;
using ImmobilienVerwalter.Maui.ViewModels;
using ImmobilienVerwalter.Maui.Views;
using Microsoft.Extensions.Logging;

namespace ImmobilienVerwalter.Maui;

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

		// Services
		builder.Services.AddSingleton<ApiService>();

		// ViewModels
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<PropertiesViewModel>();
		builder.Services.AddTransient<TenantsViewModel>();
		builder.Services.AddTransient<PaymentsViewModel>();

		// Pages
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<PropertiesPage>();
		builder.Services.AddTransient<TenantsPage>();
		builder.Services.AddTransient<PaymentsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
