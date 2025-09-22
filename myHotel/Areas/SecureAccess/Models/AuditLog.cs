namespace myHotel.Areas.SecureAccess.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string Action { get; set; } // e.g. "CreateUser", "DeleteRole"
        public string PerformedBy { get; set; } // Admin username/email
        public string AffectedUser { get; set; } // User on whom action was taken
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Details { get; set; } // Optional JSON/details of change
    }
}
