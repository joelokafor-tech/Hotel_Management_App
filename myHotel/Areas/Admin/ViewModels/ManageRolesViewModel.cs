namespace myHotel.Areas.Admin.ViewModels
{
    public class ManageRolesViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public List<string> AssignedRoles { get; set; } = new();
        public List<string> AvailableRoles { get; set; } = new();

        // For form binding
        public List<string> SelectedRoles { get; set; } = new();
    }
}
