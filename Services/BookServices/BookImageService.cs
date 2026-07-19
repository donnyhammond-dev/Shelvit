using System.Text.Json;
using BackLog.Services.Interfaces.Books;

namespace BackLog.Services.BookServices
{
    public class BookImageService : IBookImageService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookImageService> _logger;

        public BookImageService(HttpClient httpClient, ILogger<BookImageService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string?> GetBookImageUrlAsync(string title, string author)
        {
            var url = $"https://openlibrary.org/search.json?title={Uri.EscapeDataString(title)}&author={Uri.EscapeDataString(author)}";

            try
            {
                _logger.LogInformation("Searching Open Library for cover images for {Title} by {Author}", title, author);

                var response = await _httpClient.GetStringAsync(url);

                using var json = JsonDocument.Parse(response);

                var docs = json.RootElement.GetProperty("docs");

                foreach (var doc in docs.EnumerateArray())
                {
                    //Title verification if needed
                    //var docTitle = doc.GetProperty("title").GetString();
                    //if (!string.Equals(title, docTitle, StringComparison.OrdinalIgnoreCase)) continue;

                    if (doc.TryGetProperty("cover_i", out var coverProp))
                    {
                        var coverId = coverProp.GetInt32();

                        _logger.LogInformation("Cover image found for {Title} by {Author}", title, author);

                        return $"https://covers.openlibrary.org/b/id/{coverId}-M.jpg";
                    }
                }

                _logger.LogWarning("No cover image found for {Title} by {Author}", title, author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve cover image for {Title} by {Author}", title, author);

                return null;
            }


            return null;

 
        }
    }
}
