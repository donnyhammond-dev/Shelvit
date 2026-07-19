using System.ComponentModel.DataAnnotations;
using BackLog.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BackLog.ViewModels.GameViewModels
{
    public class GameFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
        public string Title { get; set; } = null!;

        public int? Year { get; set; }

        [Required]
        public Platform? Platform { get; set; }

        [Required]
        public Genre? Genre { get; set; }

        [Required]
        public Status? Status { get; set; }

        public int? Rating { get; set; }

        public List<SelectListItem>? YearList { get; set; }
        public List<SelectListItem> PlatformList { get; set; } = new();
        public List<SelectListItem> GenreList { get; set; } = new();
        public List<SelectListItem> StatusList { get; set; } = new();

        public int PageNumber { get; set; } = 2;
    }
}
