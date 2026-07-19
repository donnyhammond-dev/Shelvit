using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BackLog.DTOs;
using BackLog.Services.Interfaces.Albums;

namespace BackLog.Services.AlbumServices
{
    public class AlbumSearchService : IAlbumSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly SpotifyAuthService _spotifyAuth;

        public AlbumSearchService(HttpClient httpClient, IConfiguration config, SpotifyAuthService spotifyAuth)
        {
            _httpClient = httpClient;
            _config = config;
            _spotifyAuth = spotifyAuth;
        }

        public async Task<AlbumDto?> AlbumSearchAsync(string title)
        {
            var token = await _spotifyAuth.GetAccessTokenAsync();

            var query = Uri.EscapeDataString(title);

            var url =
                $"https://api.spotify.com/v1/search?q={query}&type=album&limit=5";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return ParseResponse(json, title);
        }

        private static AlbumDto? ParseResponse(string json, string title)
        {
            using var doc = JsonDocument.Parse(json);

            var results = doc.RootElement
                .GetProperty("albums")
                .GetProperty("items");

            JsonElement? firstAlbum = null;

            foreach (var album in results.EnumerateArray())
            {
                // Save first result as fallback
                firstAlbum ??= album;

                if (!album.TryGetProperty("name", out var nameProp))
                    continue;

                var resultName = nameProp.GetString();

                if (!string.Equals(resultName, title, StringComparison.OrdinalIgnoreCase))
                    continue;

                return new AlbumDto
                {
                    Title = resultName!,
                    Artist = GetArtist(album),
                    Year = GetYear(album)
                };
            }

            // Fallback to first result if no exact match found
            if (firstAlbum.HasValue)
            {
                var album = firstAlbum.Value;

                return new AlbumDto
                {
                    Title = album.GetProperty("name").GetString()!,
                    Artist = GetArtist(album),
                    Year = GetYear(album)
                };
            }

            return null;
        }

        private static string? GetArtist(JsonElement album)
        {
            if (album.TryGetProperty("artists", out var artists))
            {
                var firstArtist = artists.EnumerateArray().FirstOrDefault();

                if (firstArtist.ValueKind != JsonValueKind.Undefined &&
                    firstArtist.TryGetProperty("name", out var nameProp))
                {
                    return nameProp.GetString();
                }
            }

            return null;
        }

        private static int? GetYear(JsonElement album)
        {
            if (album.TryGetProperty("release_date", out var dateProp) &&
                DateTime.TryParse(dateProp.GetString(), out var date))
            {
                return date.Year;
            }

            return null;
        }

        public async Task<List<string>> AlbumAutocompleteAsync(string title)
        {
            var token = await _spotifyAuth.GetAccessTokenAsync();

            var query = Uri.EscapeDataString(title);

            var url =
                $"https://api.spotify.com/v1/search?q={query}&type=album&limit=5";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            var results = doc.RootElement
                .GetProperty("albums")
                .GetProperty("items");

            var list = new List<string>();

            foreach (var album in results.EnumerateArray())
            {
                if (album.TryGetProperty("name", out var nameProp))
                {
                    var name = nameProp.GetString();
                    if (!string.IsNullOrWhiteSpace(name))
                        list.Add(name);
                }
            }

            return list;
        }
    }
}