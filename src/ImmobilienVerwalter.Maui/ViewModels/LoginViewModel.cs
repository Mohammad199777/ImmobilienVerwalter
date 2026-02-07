using ImmobilienVerwalter.Maui.Services;

namespace ImmobilienVerwalter.Maui.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly ApiService _api;

    private string _email = "";
    public string Email { get => _email; set => SetProperty(ref _email, value); }

    private string _password = "";
    public string Password { get => _password; set => SetProperty(ref _password, value); }

    private string _errorMessage = "";
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public RelayCommand LoginCommand { get; }

    public LoginViewModel(ApiService api)
    {
        _api = api;
        Title = "Anmelden";
        LoginCommand = new RelayCommand(LoginAsync);
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Bitte E-Mail und Passwort eingeben.";
            return;
        }

        IsBusy = true;
        ErrorMessage = "";

        try
        {
            var success = await _api.LoginAsync(Email, Password);
            if (success)
            {
                // Zur Hauptseite navigieren
                Application.Current!.Windows[0].Page = new AppShell();
            }
            else
            {
                ErrorMessage = "Anmeldung fehlgeschlagen. Bitte prüfen Sie Ihre Zugangsdaten.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Verbindungsfehler: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
