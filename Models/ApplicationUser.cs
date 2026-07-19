using Microsoft.AspNetCore.Identity;

namespace BackLog.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? GameShareToken { get; set; }
        public string? AlbumShareToken { get; set; }
        public string? BookShareToken { get; set; }
    }
}
