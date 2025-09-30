using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myHotel.Areas.SecureAccess.Models;
using myHotel.Areas.SecureAccess.ViewModels;

namespace myHotel.Areas.SecureAccess.Controllers
{
    [Area("SecureAccess")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        // GET: /SecureAccess/Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /SecureAccess/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Prevent open redirect attacks
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Default redirect (for now) → Home page
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        //        if (result.Succeeded)
        //        {
        //            return Redirect(returnUrl ?? Url.Action("Index", "Home", new { area = "" })!);
        //        }

        //        ModelState.AddModelError("", "Invalid login attempt.");
        //    }
        //    return View(model);
        //}

        // GET: /SecureAccess/Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /SecureAccess/Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    UserName = model.Email,
                    Email = model.Email
                    //EmailConfirmed = true // optional for dev
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    // Instead, just redirect to login
                    TempData["SuccessMessage"] = "Registration successful. Please log in with your new account.";
                    return RedirectToAction("Login", "Account", new { area = "SecureAccess" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser
        //        {
        //            UserName = model.Email,
        //            Email = model.Email,
        //            FullName = model.FullName
        //        };

        //        var result = await _userManager.CreateAsync(user, model.Password);

        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(user, "Customer");
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            return RedirectToAction("Index", "Home", new { area = "" });
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //    }
        //    return View(model);
        //}

        // GET: /SecureAccess/Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "SecureAccess" });
        }
    }
}
