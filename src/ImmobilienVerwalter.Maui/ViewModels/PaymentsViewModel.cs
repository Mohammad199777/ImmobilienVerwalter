using ImmobilienVerwalter.Maui.Models;
using ImmobilienVerwalter.Maui.Services;
using System.Collections.ObjectModel;

namespace ImmobilienVerwalter.Maui.ViewModels;

public class PaymentsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    public ObservableCollection<PaymentDto> Payments { get; } = [];
    public RelayCommand LoadCommand { get; }

    private int _selectedYear;
    public int SelectedYear
    {
        get => _selectedYear;
        set { if (SetProperty(ref _selectedYear, value)) _ = LoadAsync(); }
    }

    private int _selectedMonth;
    public int SelectedMonth
    {
        get => _selectedMonth;
        set { if (SetProperty(ref _selectedMonth, value)) _ = LoadAsync(); }
    }

    public List<int> Years { get; } = Enumerable.Range(DateTime.Now.Year - 2, 5).ToList();
    public List<string> MonthNames { get; } = new()
    {
        "Januar", "Februar", "März", "April", "Mai", "Juni",
        "Juli", "August", "September", "Oktober", "November", "Dezember"
    };

    public PaymentsViewModel(ApiService api)
    {
        _api = api;
        Title = "Zahlungen";
        _selectedYear = DateTime.Now.Year;
        _selectedMonth = DateTime.Now.Month;
        LoadCommand = new RelayCommand(LoadAsync);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var items = await _api.GetListAsync<PaymentDto>(
                $"payments/month/{SelectedYear}/{SelectedMonth}");
            Payments.Clear();
            foreach (var item in items) Payments.Add(item);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Fehler", ex.Message, "OK");
        }
        finally { IsBusy = false; }
    }
}
