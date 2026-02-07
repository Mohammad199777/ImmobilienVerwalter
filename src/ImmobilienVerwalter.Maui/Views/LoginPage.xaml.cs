using ImmobilienVerwalter.Maui.ViewModels;

namespace ImmobilienVerwalter.Maui.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
