using System;
using System.Reflection.Metadata.Ecma335;
using BackLog.Data;
using BackLog.Helpers;
using BackLog.Models;
using BackLog.Services.Interfaces.Games;
using BackLog.ViewModels.DashboardViewModels;
using BackLog.ViewModels.GameViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackLog.Services.GameServices
{
    public class GameStatsService : IGameStatsService
    {
        private readonly ApplicationDbContext _context;

        public GameStatsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GameStatsViewModel> GetStatsAsync(string userId)
        {
            var gamesQuery = _context.Games.Where(x => x.UserId == userId);
            var gameStats = new GameStatsViewModel
            {
                TotalGames = await gamesQuery.CountAsync(),

                BackLogCount = await gamesQuery.CountAsync(x =>
                     x.Status == Status.Started ||
                     x.Status == Status.NotStarted),

                CompletionListCount = await gamesQuery.CountAsync(x => 
                     x.Status == Status.Completed || 
                     x.Status == Status.FullyCompleted),

                FullyCompletedGames = await gamesQuery.CountAsync(x => 
                     x.Status == Status.FullyCompleted),

                FavoriteGenre = await gamesQuery.GroupBy(x => x.Genre)
                .Where(g => g.Count() >= 2)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Average(x => x.Rating ?? 0))
                .Select(x => x.Key)
                .FirstOrDefaultAsync(),

                PlatformStats = await gamesQuery.GroupBy(x => x.Platform)
                .Select(x => new PlatformGameCountStat
                {
                    Name = x.Key.GetDisplayName(),
                    Count = x.Count()
                })
                .ToListAsync()
            };

            gameStats.CompletionPercentage = gameStats.TotalGames == 0 ? 0
                : (double)gameStats.CompletionListCount / gameStats.TotalGames * 100;

            gameStats.FullyCompletedPercentage = gameStats.TotalGames == 0 ? 0
                : (double)gameStats.FullyCompletedGames / gameStats.TotalGames * 100;

            gameStats.FavoritePlatform = gameStats.PlatformStats
                .OrderByDescending(x => x.Count)
                .Select(x => x.Name)
                .FirstOrDefault();



            return gameStats;
        }

 

    }
}
