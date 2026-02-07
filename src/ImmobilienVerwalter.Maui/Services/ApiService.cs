using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ImmobilienVerwalter.Maui.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private string? _token;
    private const string TokenKey = "auth_token";
    
    // Konfigurierbare API-URL
    public string BaseUrl { get; set; } = DeviceInfo.Platform == DevicePlatform.Android
        ? "https://10.0.2.2:5001/api"   // Android-Emulator
        : "https://localhost:5001/api";   // Windows / iOS Simulator

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

    public ApiService()
    {
        var handler = new HttpClientHandler
        {
            // Für Entwicklung: Selbstsignierte Zertifikate erlauben
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _http = new HttpClient(handler);

        // Gespeicherten Token laden
        _token = SecureStorage.Default.GetAsync(TokenKey).Result;
        if (!string.IsNullOrEmpty(_token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // === Auth ===
    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/auth/login", new { email, password });
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
            if (result?.Token == null) return false;

            _token = result.Token;
            await SecureStorage.Default.SetAsync(TokenKey, _token);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return true;
        }
        catch { return false; }
    }

    public void Logout()
    {
        _token = null;
        SecureStorage.Default.Remove(TokenKey);
        _http.DefaultRequestHeaders.Authorization = null;
    }

    // === GET Requests ===
    public async Task<List<T>> GetListAsync<T>(string endpoint)
    {
        var response = await _http.GetAsync($"{BaseUrl}/{endpoint}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<T>>(JsonOptions) ?? [];
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _http.GetAsync($"{BaseUrl}/{endpoint}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    // === POST ===
    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        var response = await _http.PostAsJsonAsync($"{BaseUrl}/{endpoint}", data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    // === DELETE ===
    public async Task DeleteAsync(string endpoint)
    {
        var response = await _http.DeleteAsync($"{BaseUrl}/{endpoint}");
        response.EnsureSuccessStatusCode();
    }
}

// Auth-Response
public record AuthResponse(string Token, DateTime Expiration);
