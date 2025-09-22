
using myHotel.Areas.SecureAccess.Models;
using myHotel.Infrastructure;

namespace myHotel.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly Hotel_Management_Context _HContext;

        public AuditLogService(Hotel_Management_Context HContext)
        {
            _HContext = HContext;
        }

        public async Task LogAsync(string action, string performedBy, string affectedUser, string details = null)
        {
            var log = new AuditLog
            {
                Action = action,
                PerformedBy = performedBy,
                AffectedUser = affectedUser,
                Details = details
            };

            _HContext.AuditLogs.Add(log);
            await _HContext.SaveChangesAsync();
        }
    }
}
