using System.Text.Json;

namespace BackLog.Services.GameServices
{
    public class IgdbAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        private string? _token;
        private DateTime _expiresAt;

        public IgdbAuthService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token) && DateTime.UtcNow < _expiresAt)
                return _token;

            var clientId = _config["Igdb:ClientId"];
            var clientSecret = _config["Igdb:ClientSecret"];

            var url =
                $"https://id.twitch.tv/oauth2/token" +
                $"?client_id={clientId}" +
                $"&client_secret={clientSecret}" +
                $"&grant_type=client_credentials";

            var response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            _token = json.GetProperty("access_token").GetString();
            var expiresIn = json.GetProperty("expires_in").GetInt32();

            _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 60);

            return _token!;
        }
    }
}
