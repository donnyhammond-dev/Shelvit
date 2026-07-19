using BackLog.Data;
using BackLog.Enums;
using BackLog.Models;
using BackLog.Services.Interfaces.Books;
using BackLog.ViewModels.DashboardViewModels;
using Microsoft.EntityFrameworkCore;

namespace BackLog.Services.BookServices
{
    public class BookStatsService : IBookStatsService
    {
        private readonly ApplicationDbContext _context;

        public BookStatsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookStatsViewModel> GetStatsAsync(string userId)
        {
            var booksQuery = _context.Books.Where(x => x.UserId == userId);

            var bookStats = new BookStatsViewModel
            {
                TotalBooks = await booksQuery.CountAsync(),

                PhysicalCount = await booksQuery.CountAsync(x => x.Format != BookFormat.Digital),

                DigitalCount = await booksQuery.CountAsync(x => x.Format == BookFormat.Digital),

                CompletionCount = await booksQuery.CountAsync(x => x.Status == CompletionStatus.Completed),

                BackLogCount = await booksQuery.CountAsync(x =>
                    x.Status == CompletionStatus.Incomplete &&
                    x.Status == CompletionStatus.NotStarted),

                PreferredFormat = await booksQuery.GroupBy(x => x.Format)
                .Where(g => g.Count() >= 2)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync(),

                FavoriteAuthor = await booksQuery.GroupBy(x => x.Author)
                .Where(g => g.Count() >= 2)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Average(x => x.Rating ?? 0))
                .Select(x => x.Key)
                .FirstOrDefaultAsync(),

                FavoriteGenre = await booksQuery.GroupBy (x => x.Genre)
                .Where(g => g.Count() >= 2)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Average(x => x.Rating ?? 0))
                .Select(x => x.Key)
                .FirstOrDefaultAsync()

            };
            bookStats.CompletionRate = bookStats.TotalBooks == 0 ? 0
                : (double)bookStats.CompletionCount / bookStats.TotalBooks * 100;

            return bookStats;
        }
    }
}
