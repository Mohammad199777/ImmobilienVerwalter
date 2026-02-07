using ImmobilienVerwalter.Maui.Models;
using ImmobilienVerwalter.Maui.Services;
using System.Collections.ObjectModel;

namespace ImmobilienVerwalter.Maui.ViewModels;

public class TenantsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    public ObservableCollection<TenantDto> Tenants { get; } = [];
    public RelayCommand LoadCommand { get; }

    public TenantsViewModel(ApiService api)
    {
        _api = api;
        Title = "Mieter";
        LoadCommand = new RelayCommand(LoadAsync);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var items = await _api.GetListAsync<TenantDto>("tenants");
            Tenants.Clear();
            foreach (var item in items) Tenants.Add(item);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Fehler", ex.Message, "OK");
        }
        finally { IsBusy = false; }
    }
}
