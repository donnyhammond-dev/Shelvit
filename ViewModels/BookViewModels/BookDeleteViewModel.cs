using BackLog.Models;

namespace BackLog.ViewModels.BookViewModels
{
    public class BookDeleteViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        public int PageNumber { get; set; }
    }
}
