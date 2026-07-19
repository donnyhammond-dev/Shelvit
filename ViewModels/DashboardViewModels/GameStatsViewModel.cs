using BackLog.Models;
using BackLog.ViewModels.GameViewModels;

namespace BackLog.ViewModels.DashboardViewModels
{
    public class GameStatsViewModel
    {
        public int TotalGames { get; set; }
        public int BackLogCount { get; set; }
        public int CompletionListCount { get; set; }
        public int FullyCompletedGames { get; set; }
        public double CompletionPercentage { get; set; }
        public double FullyCompletedPercentage { get; set; }
        public List<PlatformGameCountStat> PlatformStats { get; set; } = new();

        public Genre FavoriteGenre { get; set; }
        public string? FavoritePlatform { get; set; }

    }
}
