using ImmobilienVerwalter.Maui.Services;
using ImmobilienVerwalter.Maui.Views;

namespace ImmobilienVerwalter.Maui;

public partial class App : Application
{
	private readonly IServiceProvider _services;

	public App(IServiceProvider services)
	{
		InitializeComponent();
		_services = services;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var api = _services.GetRequiredService<ApiService>();

		// Wenn bereits authentifiziert, direkt zur Shell; sonst Login
		if (api.IsAuthenticated)
		{
			return new Window(new AppShell());
		}

		var loginPage = _services.GetRequiredService<LoginPage>();
		return new Window(new NavigationPage(loginPage));
	}
}