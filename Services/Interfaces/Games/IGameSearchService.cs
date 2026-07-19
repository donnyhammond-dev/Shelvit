using BackLog.DTOs;

namespace BackLog.Services.Interfaces.Games
{
    public interface IGameSearchService
    {
        Task<GameDto?> GameSearchAsync(string title);

        Task<List<string>> GameAutocompleteAsync(string title);
    }
}
