using BackLog.ViewModels.DashboardViewModels;

namespace BackLog.Services.Interfaces.Books
{
    public interface IBookStatsService
    {
        Task<BookStatsViewModel> GetStatsAsync(string userId);
    }
}
