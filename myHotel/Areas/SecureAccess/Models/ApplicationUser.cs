using Microsoft.AspNetCore.Identity;

namespace myHotel.Areas.SecureAccess.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extend IdentityUser with hotel-specific fields
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
