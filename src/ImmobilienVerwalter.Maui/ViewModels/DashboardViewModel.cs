using ImmobilienVerwalter.Maui.Models;
using ImmobilienVerwalter.Maui.Services;
using System.Collections.ObjectModel;

namespace ImmobilienVerwalter.Maui.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly ApiService _api;

    public ObservableCollection<DashboardCard> Cards { get; } = [];

    private DashboardData? _data;
    public DashboardData? Data { get => _data; set => SetProperty(ref _data, value); }

    public RelayCommand LoadCommand { get; }
    public RelayCommand LogoutCommand { get; }

    public DashboardViewModel(ApiService api)
    {
        _api = api;
        Title = "Dashboard";
        LoadCommand = new RelayCommand(LoadAsync);
        LogoutCommand = new RelayCommand(LogoutAsync);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Data = await _api.GetAsync<DashboardData>("dashboard");
            Cards.Clear();
            if (Data != null)
            {
                Cards.Add(new("🏠", "Immobilien", Data.TotalProperties.ToString()));
                Cards.Add(new("🏢", "Einheiten", $"{Data.OccupiedUnits}/{Data.TotalUnits}"));
                Cards.Add(new("📊", "Auslastung", $"{Data.OccupancyRate:F0} %"));
                Cards.Add(new("💰", "Mtl. Einnahmen", $"{Data.MonthlyRentIncome:N2} €"));
                Cards.Add(new("📉", "Mtl. Ausgaben", $"{Data.MonthlyExpenses:N2} €"));
                Cards.Add(new("⚠️", "Überfällig", $"{Data.OverduePayments} ({Data.OverdueAmount:N2} €)"));
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Fehler", ex.Message, "OK");
        }
        finally { IsBusy = false; }
    }

    private async Task LogoutAsync()
    {
        _api.Logout();
        Application.Current!.Windows[0].Page = new NavigationPage(
            App.Current!.Handler!.MauiContext!.Services.GetRequiredService<Views.LoginPage>());
    }
}

public record DashboardCard(string Icon, string Label, string Value);
