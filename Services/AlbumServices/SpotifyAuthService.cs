using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BackLog.Services.AlbumServices
{
    
    public class SpotifyAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        private string? _accessToken;
        private DateTime _tokenExpiry;

        public SpotifyAuthService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
                return _accessToken;

            var clientId = _config["Spotify:ClientID"];
            var clientSecret = _config["Spotify:ClientSecret"];

            var auth = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")
            );

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", auth);

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            _accessToken = doc.RootElement.GetProperty("access_token").GetString();

            var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
            _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60);

            return _accessToken!;
        }

    }
}
