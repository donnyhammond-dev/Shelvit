using BackLog.Data;
using BackLog.ViewModels.DashboardViewModels;
using Microsoft.EntityFrameworkCore;
using BackLog.Models;
using BackLog.Enums;
using BackLog.Services.Interfaces.Albums;

namespace BackLog.Services.AlbumServices
{
    public class AlbumStatsService : IAlbumStatsService
    {
        private readonly ApplicationDbContext _context;

        public AlbumStatsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AlbumStatsViewModel> GetStatsAsync(string userId)
        {
            var albumsQuery = _context.Albums.Where(x => x.UserId == userId);

            var albumStats = new AlbumStatsViewModel
            {
                TotalAlbums = await albumsQuery.CountAsync(),

                VinylCount = await albumsQuery.CountAsync(x => x.Format == Format.Vinyl),

                CDCount = await albumsQuery.CountAsync(x => x.Format == Format.CD),

                TapeCount = await albumsQuery.CountAsync(x => x.Format == Format.Cassette),

                DigitalCount = await albumsQuery.CountAsync(x => x.Format == Format.Digital),

                AlbumsFinished = await albumsQuery.CountAsync(x => x.Status == CompletionStatus.Completed),

                BackLogCount = await albumsQuery.CountAsync(x =>
                    x.Status == CompletionStatus.Incomplete ||
                    x.Status == CompletionStatus.NotStarted),

                PreferredFormat = await albumsQuery.GroupBy(x => x.Format)
                .Where(g => g.Count() >= 2)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync(),

                FavoriteArtist = await albumsQuery.GroupBy(x => x.Artist)
                .Where(g => g.Count() >= 2)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Average(x => x.Rating ?? 0))
                .Select(x => x.Key)
                .FirstOrDefaultAsync(),

                FavoriteGenre = await albumsQuery.GroupBy(x => x.Genre)
                .Where(g => g.Count() >= 2)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Average(x => x.Rating ?? 0))
                .Select(x => x.Key)
                .FirstOrDefaultAsync()
            };
            albumStats.FinishPercentage = albumStats.TotalAlbums == 0 ? 0
                : (double)albumStats.AlbumsFinished / albumStats.TotalAlbums * 100;

            return albumStats;
        }
    }
}
