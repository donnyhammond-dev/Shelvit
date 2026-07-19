using BackLog.DTOs;

namespace BackLog.Services.Interfaces.Albums
{
    public interface IAlbumSearchService
    {
        Task<AlbumDto?> AlbumSearchAsync(string title);

        Task<List<string>> AlbumAutocompleteAsync(string title);
    }
}
