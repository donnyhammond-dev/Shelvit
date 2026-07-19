namespace BackLog.Services.Interfaces.Games
{
    public interface IGameImageService
    {
        Task<string?> GetGameImageUrlAsync(string title);
    }
}
