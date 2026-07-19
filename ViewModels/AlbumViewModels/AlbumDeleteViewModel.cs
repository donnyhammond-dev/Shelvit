using BackLog.Models;

namespace BackLog.ViewModels.AlbumViewModels
{
    public class AlbumDeleteViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public Format Format { get; set; }

        public int PageNumber { get; set; }
    }
}
