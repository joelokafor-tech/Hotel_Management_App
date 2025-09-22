using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myHotel.Areas.Admin.ViewModels;
using myHotel.Areas.SecureAccess.Models;
using myHotel.Services;

namespace myHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuditLogService _auditLogService;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuditLogService auditLogService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var userList = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserWithRolesViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userList);
        }

        // GET: /Admin/Users/ManageRoles
        public async Task<IActionResult> ManageRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var model = new ManageRolesViewModel
            {
                UserId = user.Id,
                FullName = user.FullName,
                AssignedRoles = userRoles.ToList(),
                AvailableRoles = allRoles.Select(r => r.Name).ToList()
            };

            return View(model);
        }

        // POST: /Admin/Users/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(ManageRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var rolesToAdd = model.SelectedRoles.Except(userRoles);
            var rolesToRemove = userRoles.Except(model.SelectedRoles);

            await _userManager.AddToRolesAsync(user, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            // Fetch updated roles
            var updatedRoles = await _userManager.GetRolesAsync(user);
            string rolesList = updatedRoles.Any() ? string.Join(", ", updatedRoles) : "No roles";

            // Audit log entry
            await _auditLogService.LogAsync(
                "ManageRoles",
                User.Identity.Name,   // admin performing the action
                user.UserName,        // affected user
                $"Roles updated: {rolesList}"
            );

            TempData["SuccessMessage"] = $"Roles updated for {user.FullName}: {rolesList}";

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };

            return View(model);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // Capture old values before updating
            //user.FullName = model.FullName;
            //user.Email = model.Email;
            //user.UserName = model.Email; // keep username = email for consistency

            // Capture old values BEFORE making changes
            var oldFullName = user.FullName;
            var oldEmail = user.Email;
            var oldUserName = user.UserName;

            // Apply new values
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email; // keep username = email for consistency

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Build a list of changes
                var changes = new List<string>();

                if (oldFullName != user.FullName)
                    changes.Add($"FullName: '{oldFullName}' → '{user.FullName}'");

                if (oldEmail != user.Email)
                    changes.Add($"Email: '{oldEmail}' → '{user.Email}'");

                if (oldUserName != user.UserName)
                    changes.Add($"UserName: '{oldUserName}' → '{user.UserName}'");

                string details = changes.Any()
                    ? "Updated fields: " + string.Join("; ", changes)
                    : "No changes detected.";

                // Audit log entry
                await _auditLogService.LogAsync(
                    "EditUser",
                    User.Identity.Name,   // admin performing the action
                    user.UserName,        // affected user
                    details
                );

                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                // Audit log entry
                await _auditLogService.LogAsync(
                    "DeleteUser",
                    User.Identity.Name,   // admin performing the action
                    user.UserName,        // affected user
                    $"User {user.Email} deleted."
                );

                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
