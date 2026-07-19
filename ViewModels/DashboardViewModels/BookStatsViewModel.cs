using BackLog.Models;

namespace BackLog.ViewModels.DashboardViewModels
{
    public class BookStatsViewModel
    {
        public int TotalBooks { get; set; }
        public int PhysicalCount { get; set; }
        public int DigitalCount { get; set; }
        public int BackLogCount { get; set; }
        public int CompletionCount { get; set; }
        public double CompletionRate { get; set; }

        public string? FavoriteAuthor { get; set; }

        public BookGenre? FavoriteGenre { get; set; }
        
        public BookFormat? PreferredFormat { get; set; }



    }
}
