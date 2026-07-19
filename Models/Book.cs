using System.ComponentModel.DataAnnotations;
using BackLog.Enums;

namespace BackLog.Models
{
    public enum BookFormat
    {
        [Display(Name = "Hard Cover")]
        Hardcover,
        [Display(Name = "Soft Cover")]
        Softcover,
        Digital
    }

    public enum BookGenre
    {
        Fiction,
        NonFiction,
        Biography,
        Fantasy,
        History,
        Mystery,
        Romance,
        Science,
        SciFi,
        SelfHelp,
        Other
    }
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = null!;

        //Table Data
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!; 
        public string SortTitle { get; set; } = null!;
        [Required]
        [MaxLength(150)]
        public string Author { get; set; } = null!;
        public string SortAuthor { get; set; } = null!;
        [Required]
        public BookGenre Genre { get; set; }
        [Required]
        public BookFormat Format { get; set; }
        [Required]
        public CompletionStatus Status { get; set; }
        public int? Year { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }

        //Image
        public string? ImageUrl { get; set; }
        public string? CustomImageUrl { get; set; }
    }
}
