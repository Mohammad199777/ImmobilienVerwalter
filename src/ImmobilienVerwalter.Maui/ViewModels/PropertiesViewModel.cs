using ImmobilienVerwalter.Maui.Models;
using ImmobilienVerwalter.Maui.Services;
using System.Collections.ObjectModel;

namespace ImmobilienVerwalter.Maui.ViewModels;

public class PropertiesViewModel : BaseViewModel
{
    private readonly ApiService _api;
    public ObservableCollection<PropertyDto> Properties { get; } = [];
    public RelayCommand LoadCommand { get; }

    public PropertiesViewModel(ApiService api)
    {
        _api = api;
        Title = "Immobilien";
        LoadCommand = new RelayCommand(LoadAsync);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var items = await _api.GetListAsync<PropertyDto>("properties");
            Properties.Clear();
            foreach (var item in items) Properties.Add(item);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Fehler", ex.Message, "OK");
        }
        finally { IsBusy = false; }
    }
}
