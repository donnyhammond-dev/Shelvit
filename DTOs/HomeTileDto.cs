using BackLog.Enums;

namespace BackLog.DTOs
{
    public class HomeTileDto
    {
        public HomeTileDto(string title, string imageUrl, EntryType type)
        {
            Title = title;
            ImageUrl = imageUrl;
            Type = type;
        }

        public string Title { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public EntryType Type { get; set; }
        public string? DetailUrl { get; set; }
        public string? SourceId { get; set; }
    }
}
