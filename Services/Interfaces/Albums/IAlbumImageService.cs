namespace BackLog.Services.Interfaces.Albums
{
    public interface IAlbumImageService
    {
        Task<string?> GetAlbumImageUrlAsync(string artist, string title);
    }
}