using BackLog.Models;
using BackLog.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using BackLog.Services;

namespace BackLog.ViewModels.GameViewModels
{
    public class LibraryViewModel
    {
        public PaginatedList<Game> Games { get; set; } = default!;
        public string? Search {  get; set; }
        public Platform? Platform { get; set; }
        public Genre? Genre { get; set; }
        public Status? Status { get; set; }
        public string? StatusFilter { get; set; }
        public string? StatusGroup { get; set; } // "All", "BackLog", "Completed"
        public List<SelectListItem> PlatformList { get; set; } = new();
        public List<SelectListItem> GenreList { get; set; } = new();
        public List<SelectListItem> StatusList { get; set; } = new();
        public List<SelectListItem> StatusGroupList { get; set; } = new();
        public string? SortBy { get; set; }
        public bool IsDesc{ get; set; }
        public string ShareToken { get; set; } = string.Empty;
        public int? Rating { get; set; }
        public bool HasAnyGames { get; set; }
        public int PageNumber { get; set; }
        public bool IsGridMode { get; set; } = false;

        public bool IsViewAllMode { get; set; } = false;
    }
}
