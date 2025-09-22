using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using myHotel.Areas.SecureAccess.Models;

namespace myHotel.Infrastructure
{
    public class Hotel_Management_Context : IdentityDbContext<ApplicationUser>
    {
        public Hotel_Management_Context(DbContextOptions<Hotel_Management_Context> options) : base(options) { }


        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}
