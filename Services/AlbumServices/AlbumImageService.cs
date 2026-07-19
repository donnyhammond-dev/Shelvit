using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BackLog.Services.Interfaces.Albums;
using Microsoft.Extensions.Configuration;

namespace BackLog.Services.AlbumServices
{
    public class AlbumImageService : IAlbumImageService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly SpotifyAuthService _spotifyAuth;

        public AlbumImageService(HttpClient httpClient, IConfiguration config, SpotifyAuthService spotifyAuth)
        {
            _httpClient = httpClient;
            _config = config;
            _spotifyAuth = spotifyAuth;
        }

        public async Task<string?> GetAlbumImageUrlAsync(string artist, string title)
        {
            try
            {
                var token = await _spotifyAuth.GetAccessTokenAsync();

                var query = Uri.EscapeDataString($"{artist} {title}");

                var url = $"https://api.spotify.com/v1/search?q={query}&type=album&limit=5";

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var items = doc.RootElement
                    .GetProperty("albums")
                    .GetProperty("items");

                foreach (var item in items.EnumerateArray())
                {
                    if (!item.TryGetProperty("images", out var images))
                        continue;

                    if (images.GetArrayLength() > 0)
                    {
                        return images[0].GetProperty("url").GetString();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }

        }
    }
}