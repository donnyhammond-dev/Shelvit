using BackLog.Models;
using BackLog.Enums;
using BackLog.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BackLog.ViewModels.AlbumViewModels
{
    public class AlbumIndexViewModel
    {
        public PaginatedList<Album> Albums { get; set; } = default!;
        public string? Search {  get; set; }
        public MusicGenre? Genre { get; set; }
        public Format? Format { get; set; }
        public CompletionStatus? Status { get; set; }
        public List<SelectListItem> GenreList { get; set; } = new();
        public List<SelectListItem> FormatList { get; set; } = new();
        public List<SelectListItem> StatusList { get; set; } = new();
        public int? Year { get; set; }
        public int? Rating { get; set; }
        public string? SortBy { get; set; }
        public bool IsDesc { get; set; }

        public string ShareToken { get; set; } = string.Empty;

        public bool HasAnyAlbums { get; set; }

        public int PageNumber { get; set; }

        public bool IsGridMode { get; set; } = false;

        public bool IsViewAllMode { get; set; } = false;
    }
}
