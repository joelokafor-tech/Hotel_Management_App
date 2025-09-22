﻿namespace myHotel.Areas.Admin.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public List<RoleSelection> Roles { get; set; } = new List<RoleSelection>();
    }

    public class RoleSelection
    {
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
