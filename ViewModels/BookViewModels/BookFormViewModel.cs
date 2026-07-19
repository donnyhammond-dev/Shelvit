using BackLog.Models;
using BackLog.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BackLog.ViewModels.BookViewModels
{
    public class BookFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters!")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(150, ErrorMessage = "Author cannot exceed 150 characters!")]
        public string Author { get; set; } = string.Empty;

        public int? Year { get; set; }

        [Required]
        public BookGenre? Genre { get; set; }

        [Required]
        public BookFormat? Format { get; set; }

        [Required]
        public CompletionStatus? Status { get; set; }

        public int? Rating { get; set; }

        public List<SelectListItem> GenreList { get; set; } = new();
        public List<SelectListItem> FormatList { get; set; } = new();
        public List<SelectListItem> StatusList { get; set; } = new();

        public List<SelectListItem>? YearList { get; set; }

        public int PageNumber { get; set; }
    }
}