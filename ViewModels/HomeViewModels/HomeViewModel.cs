using BackLog.DTOs;

namespace BackLog.ViewModels.HomeViewModels
{
    public class HomeViewModel
    {
        public List<HomeTileDto> GameTiles { get; set; } = new();
        public List<HomeTileDto> AlbumTiles { get; set; } = new();
        public List<HomeTileDto> BookTiles { get; set; } = new();
    }
}
