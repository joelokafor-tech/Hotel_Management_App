namespace myHotel.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string action, string performedBy, string affectedUser, string details = null);
    }
}
