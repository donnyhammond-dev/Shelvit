using System.ComponentModel.DataAnnotations;
using BackLog.Enums;

namespace BackLog.Models
{
    public enum Format
    {
        Vinyl,
        CD,
        Digital,
        Cassette
    }
    public enum MusicGenre
    {
        Pop,
        Rock,
        HipHop,
        RnB,
        Electronic,
        Dance,
        Jazz,
        Blues,
        Classical,
        Country,
        Metal,
        Punk,
        Indie,
        Alternative,
        Reggae,
        Soul,
        Folk,
        Soundtrack
    }
    public class Album
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
        public string Artist { get; set; } = null!;
        public string SortArtist { get; set; } = null!;
        [Required]
        public MusicGenre Genre { get; set; }
        [Required]
        public Format Format { get; set; }
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
