using System.Text.Json;
using BackLog.DTOs;
using BackLog.Services.Interfaces.Games;
using BackLog.Models;
using System.Text;

namespace BackLog.Services.GameServices
{
    public class GameSearchService : IGameSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public GameSearchService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<GameDto?> GameSearchAsync(string title)
        {
            var apiKey = _config["Rawg:ApiKey"];

            var query = Uri.EscapeDataString(title);

            var url = $"https://api.rawg.io/api/games?key={apiKey}&search={query}";

            var response = await _httpClient.GetStringAsync(url);

            return ParseResponse(response, title);
        }

        public async Task<List<string>> GameAutocompleteAsync(string title)
        {
            var apiKey = _config["Rawg:ApiKey"];
            var query = Uri.EscapeDataString(title);

            var url = $"https://api.rawg.io/api/games?key={apiKey}&search={query}&page_size=5";

            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);

            if (!doc.RootElement.TryGetProperty("results", out var results))
                return new List<string>();

            var titles = new List<string>();

            foreach (var game in results.EnumerateArray())
            {
                if (!game.TryGetProperty("name", out var nameProp))
                    continue;

                var name = nameProp.GetString();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    titles.Add(System.Text.RegularExpressions.Regex.Replace(name, @"\s+", " ").Trim());
                }
            }

            return titles
                .Distinct()
                .ToList();
        }

        private static GameDto? ParseResponse(string json, string title)
        {
            using var doc = JsonDocument.Parse(json);

            var results = doc.RootElement.GetProperty("results");

            JsonElement? firstGame = null;

            foreach (var game in results.EnumerateArray())
            {
                // Save first result as fallback
                firstGame ??= game;

                if (!game.TryGetProperty("name", out var nameProp))
                    continue;

                var resultName = nameProp.GetString();

                if (!string.Equals(resultName, title, StringComparison.OrdinalIgnoreCase))
                    continue;


                return new GameDto
                {
                    Title = title,
                    Year = GetYear(game),
                };
            }

            // Fallback to first result if no exact match found
            if (firstGame.HasValue)
            {
                var game = firstGame.Value;

                return new GameDto
                {
                    Title = title,
                    Year = GetYear(game),
                  
                };
            }

            return null;
        }

        private static int? GetYear(JsonElement game)
        {
            if (game.TryGetProperty("released", out var dateProp) &&
                DateTime.TryParse(dateProp.GetString(), out var date))
            {
                return date.Year;
            }

            return null;
        }

        /*
        private static List<string> GetPlatforms(JsonElement game) =>
       game.GetProperty("platforms").EnumerateArray()
                .Select(x => x.GetProperty("platform").GetProperty("name").GetString())
                .Where(x => !string.IsNullOrEmpty(x)).Select(x =>x!).ToList();
        */





    }
}