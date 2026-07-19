using BackLog.Models;

namespace BackLog.ViewModels.SharedViewModels
{
    public class SharedAlbumsViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<Album> Albums { get; set; } = new();
        public string ShareToken { get; set; } = string.Empty;

        //Sorting Feature
        public string? SortBy { get; set; }
        public bool IsDesc { get; set; }

    }
}
