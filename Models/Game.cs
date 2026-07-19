using System.ComponentModel.DataAnnotations;

namespace BackLog.Models
{
    public enum Status
    {
        [Display(Name = "100%")]
        FullyCompleted,
        Completed,
        [Display(Name = "Incomplete")]
        Started,
        [Display(Name = "Not Started")]
        NotStarted,
        Abandoned

    }

    public enum Platform
    {
        [Display(Name = "PC")]
        PC,

        [Display(Name = "NES")]
        NES,

        [Display(Name = "SNES")]
        SNES,

        [Display(Name = "Sega Master System")]
        MasterSystem,

        [Display(Name = "Sega Genesis")]
        Genesis,

        [Display(Name = "Sega Saturn")]
        Saturn,

        [Display(Name = "Nintendo 64")]
        N64,

        [Display(Name = "PlayStation 1")]
        PS1,

        [Display(Name = "Dreamcast")]
        Dreamcast,

        [Display(Name = "PlayStation 2")]
        PS2,

        [Display(Name = "Nintendo GameCube")]
        GameCube,

        [Display(Name = "Xbox")]
        Xbox,

        [Display(Name = "Xbox 360")]
        Xbox360,

        [Display(Name = "PlayStation 3")]
        PS3,

        [Display(Name = "Wii")]
        Wii,

        [Display(Name = "Wii U")]
        WiiU,

        [Display(Name = "PlayStation 4")]
        PS4,

        [Display(Name = "Xbox One")]
        XboxOne,

        [Display(Name = "Nintendo Switch")]
        Switch,

        [Display(Name = "PlayStation 5")]
        PS5,

        [Display(Name = "Xbox Series S/X")]
        XboxSX,

        [Display(Name = "Nintendo Switch 2")]
        Switch2,

        [Display(Name = "Game Boy")]
        GameBoy,

        [Display(Name = "Game Boy Advance")]
        GBA,

        [Display(Name = "Nintendo DS")]
        DS,

        [Display(Name = "Nintendo 3DS")]
        Nintendo3DS,

        [Display(Name = "Playstation Vita")]
        PSVita,

        [Display(Name = "PlayStation Portable")]
        PSP
    }

    public enum Genre
    {
        Action,
        Adventure,
        RPG,
        Shooter,
        Strategy,
        Simulation,
        Sports,
        Racing,
        Puzzle,
        Platformer,
        Fighting,
        Horror
    }

    public class Game
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = null!;

        //Table Data
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = null!;
        public string SortTitle { get; set; } = null!;
        public int? Year { get; set; }
        [Required]
        public Platform Platform { get; set; }
        [Required]
        public Genre Genre { get; set; }
        [Required]
        public Status Status { get; set; }

        [Range(1,5)]
        public int? Rating { get; set; }

        //Image
        public string? ImageUrl { get; set; }
        public string? CustomImageUrl { get; set; }


    }
}
