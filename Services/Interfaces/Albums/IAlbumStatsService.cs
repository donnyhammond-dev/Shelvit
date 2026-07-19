using BackLog.ViewModels.DashboardViewModels;

namespace BackLog.Services.Interfaces.Albums
{
    public interface IAlbumStatsService
    {
        Task<AlbumStatsViewModel> GetStatsAsync(string userId);
    }
}
