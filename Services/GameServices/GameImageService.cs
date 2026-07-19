using System.Text;
using System.Text.Json;
using BackLog.Services.Interfaces.Games;

namespace BackLog.Services.GameServices
{
    public class GameImageService : IGameImageService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IgdbAuthService _authService;

        public GameImageService(
            HttpClient httpClient,
            IConfiguration config,
            IgdbAuthService authService)
        {
            _httpClient = httpClient;
            _config = config;
            _authService = authService;
        }

        public async Task<string?> GetGameImageUrlAsync(string title)
        {
            try
            {
                var games = await SearchGameAsync(title);

                // First pass: look for exact title match
                foreach (var game in games.EnumerateArray())
                {
                    if (!game.TryGetProperty("name", out var nameProp))
                        continue;

                    var gameName = nameProp.GetString();

                    if (string.IsNullOrWhiteSpace(gameName))
                        continue;

                    if (NormalizeTitle(gameName) == NormalizeTitle(title))
                    {
                        if (game.TryGetProperty("cover", out var coverObj) &&
                            coverObj.TryGetProperty("image_id", out var imageIdProp))
                        {
                            var imageId = imageIdProp.GetString();

                            if (!string.IsNullOrWhiteSpace(imageId))
                            {
                                return BuildCoverUrl(imageId);
                            }
                        }
                    }
                }


                // Second pass/fallback
                foreach (var game in games.EnumerateArray())
                {
                    if (game.TryGetProperty("cover", out var coverObj) &&
                        coverObj.TryGetProperty("image_id", out var imageIdProp))
                    {
                        var imageId = imageIdProp.GetString();

                        if (!string.IsNullOrWhiteSpace(imageId))
                        {
                            return BuildCoverUrl(imageId);
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // IGDB SEARCH
        private async Task<JsonElement> SearchGameAsync(string title)
        {
            var token = await _authService.GetAccessTokenAsync();

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.igdb.com/v4/games");

            request.Headers.Add("Client-ID", _config["Igdb:ClientId"]);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            request.Content = new StringContent(
                $"search \"{title}\";" +
                "fields name,cover.image_id;" +
                "limit 10;",
                Encoding.UTF8,
                "text/plain");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }

        // COVER URL BUILDER
        private static string BuildCoverUrl(string imageId)
        {
            return
                $"https://images.igdb.com/igdb/image/upload/t_cover_big/{imageId}.jpg";
        }

        private static string NormalizeTitle(string title)
        {
            return title
                .ToLower()
                .Replace(":", "")
                .Replace("-", "")
                .Replace("'", "")
                .Trim();
        }
    }
}