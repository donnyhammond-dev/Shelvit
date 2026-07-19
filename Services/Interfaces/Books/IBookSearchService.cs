using BackLog.DTOs;

namespace BackLog.Services.Interfaces.Books
{
    public interface IBookSearchService
    {
        Task<BookDto?> BookSearchAsync(string title);

        Task<List<string>> BookAutocompleteAsync(string title);
    }
}
