using BackLog.Models;

namespace BackLog.ViewModels.SharedViewModels
{
    public class SharedGamesViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<Game> Games { get; set; } = new();
        public string ShareToken { get; set; } = string.Empty;

        //Sorting Feature
        public string? SortBy { get; set; }
        public bool IsDesc { get; set; }
    }
}
