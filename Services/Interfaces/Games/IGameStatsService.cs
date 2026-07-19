using BackLog.ViewModels.DashboardViewModels;

namespace BackLog.Services.Interfaces.Games
{
    public interface IGameStatsService
    {
        Task<GameStatsViewModel> GetStatsAsync(string userId);
    }
}
