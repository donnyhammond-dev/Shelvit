using BackLog.Models;

namespace BackLog.DTOs
{
    public class GameDto
    {
        public string Title { get; set; } = "";
        public int? Year { get; set; }
        public List<string> Platforms { get; set; } = [];
    }
}
