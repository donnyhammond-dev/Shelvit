using System.Text.Json;
using BackLog.DTOs;
using BackLog.Services.Interfaces.Books;

namespace BackLog.Services.BookServices
{
    public class BookSearchService : IBookSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookSearchService> _logger;

        public BookSearchService(HttpClient httpClient, ILogger<BookSearchService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BookDto?> BookSearchAsync(string title)
        {
            try
            {
                _logger.LogInformation("Searching Open Library for data on {Title}", title);

                using var json = await GetJsonDocAsync(title);

                var docs = json.RootElement.GetProperty("docs");

                JsonElement? firstBook = null;

                foreach (var doc in docs.EnumerateArray())
                {
                    firstBook ??= doc;

                    if (!doc.TryGetProperty("title", out var titleProp))
                        continue;

                    var docTitle = titleProp.GetString();

                    if (!string.Equals(
                            docTitle?.Trim(),
                            title.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    _logger.LogInformation("Exact match found for {Title}, returning result", title);

                    return CreateBookDto(doc, docs, title);
                }

                // Fallback to first result if no exact title match found
                if (firstBook.HasValue)
                {
                    _logger.LogInformation("No exact match was found for {Title}, returning first search result", title);

                    return CreateBookDto(firstBook.Value, docs, title);
                }

                _logger.LogWarning("No match or data was found for {Title}", title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve data for {Title}", title);

                return null;
            }
            return null;
           
        }

        public async Task<List<string>> BookAutocompleteAsync(string title)
        {
            try
            {
                _logger.LogInformation("Searching Open Library for title suggestions matching \"{Title}\"", title);

                using var json = await GetJsonDocAsync(title);

                var docs = json.RootElement.GetProperty("docs");

                var titles = new List<string>();

                foreach (var doc in docs.EnumerateArray().Take(5))
                {
                    if (doc.TryGetProperty("title", out var titleProp))
                    {
                        var bookTitle = titleProp.GetString();

                        if (!string.IsNullOrWhiteSpace(bookTitle))
                        {
                            titles.Add(bookTitle);
                        }
                    }
                }

                _logger.LogInformation("Returned {Count} title suggestions for \"{Title}\"", titles.Count, title);

                return titles.Distinct().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve Title Suggestions for \"{Title}\"", title);

                return new List<string>();
            }

        }

        private static BookDto CreateBookDto(JsonElement book, JsonElement docs, string fallbackTitle)
        {
            string? author = null;

            if (book.TryGetProperty("author_name", out var authors)
                && authors.ValueKind == JsonValueKind.Array
                && authors.GetArrayLength() > 0)
            {
                author = authors[0].GetString();
            }

            string finalTitle = fallbackTitle;

            if (book.TryGetProperty("title", out var titleProp))
            {
                finalTitle = titleProp.GetString() ?? fallbackTitle;
            }

            int? year = docs.EnumerateArray()
                .Where(doc =>
                {
                    if (!doc.TryGetProperty("title", out var docTitle))
                        return false;

                    if (!string.Equals(
                            docTitle.GetString()?.Trim(),
                            finalTitle.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(author))
                        return false;

                    if (!doc.TryGetProperty("author_name", out var docAuthors)
                        || docAuthors.ValueKind != JsonValueKind.Array
                        || docAuthors.GetArrayLength() == 0)
                    {
                        return false;
                    }

                    return string.Equals(
                        docAuthors[0].GetString(),
                        author,
                        StringComparison.OrdinalIgnoreCase);
                })
                .Where(doc =>
                    doc.TryGetProperty("first_publish_year", out var yearProp)
                    && yearProp.ValueKind == JsonValueKind.Number)
                .Select(doc => doc.GetProperty("first_publish_year").GetInt32())
                .DefaultIfEmpty()
                .Min();

            return new BookDto
            {
                Title = finalTitle,
                Author = author,
                Year = year == 0 ? null : year
            };
        }

        private async Task<JsonDocument> GetJsonDocAsync(string title)
        {
            var query = Uri.EscapeDataString(title);

            var url = $"https://openlibrary.org/search.json?title={query}";

            var response = await _httpClient.GetStringAsync(url);

            return JsonDocument.Parse(response);
        }
    }
}