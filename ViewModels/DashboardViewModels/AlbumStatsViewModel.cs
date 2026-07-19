using BackLog.Models;

namespace BackLog.ViewModels.DashboardViewModels
{
    public class AlbumStatsViewModel
    {
        public int TotalAlbums { get; set; }
        public int BackLogCount { get; set; }
        public int VinylCount { get; set; }
        public int CDCount { get; set; }
        public int TapeCount { get; set; }
        public int DigitalCount { get; set; }
        public int AlbumsFinished { get; set; }
        public double FinishPercentage { get; set; }
        public string? FavoriteArtist { get; set; }
        public MusicGenre? FavoriteGenre { get; set; }
        public Format? PreferredFormat { get; set; }
    }
}
