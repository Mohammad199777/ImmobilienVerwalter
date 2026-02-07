using ImmobilienVerwalter.Maui.ViewModels;

namespace ImmobilienVerwalter.Maui.Views;

public partial class TenantsPage : ContentPage
{
    private readonly TenantsViewModel _vm;

    public TenantsPage(TenantsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
