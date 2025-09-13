using CarpetStore.Data;
using CarpetStore.Models;
using CarpetStore.Utility;
using CarpetStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace CarpetStore.Controllers
{
    public class AccountController : Controller
    {
        private CarpetStoreWebDb _dbContext;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public AccountController(CarpetStoreWebDb dbContext, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        private void InitializeAdminUser()
        {

            if (!_dbContext.Users.Any())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "admin",
                    // AccountCreated = DateTime.Now
                };

                var result = _userManager.CreateAsync(adminUser, "Admin123@").Result;

                if (result.Succeeded)
                {
                    var adminRole = "Admin";
                    var roleExist = _roleManager.RoleExistsAsync(adminRole).Result;

                    if (!roleExist)
                    {
                        var roleResult = _roleManager.CreateAsync(new IdentityRole(adminRole)).Result;
                        if (!roleResult.Succeeded)
                        {
                            throw new Exception("Failed to create admin role.");
                        }
                    }

                    var addToRoleResult = _userManager.AddToRoleAsync(adminUser, adminRole).Result;
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception("Failed to add user to admin role.");
                    }
                }
                else
                {
                    throw new Exception("Failed to create admin user.");
                }
            }
        }

        public IActionResult Login()
        {
            InitializeAdminUser();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, false);

            if (result.Succeeded)
            {
                // Update last login time
                // var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                // if (user != null)
                // {
                //     user.LastLogin = DateTime.Now;
                //     await _userManager.UpdateAsync(user);
                // }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid Login");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }

            if (!_roleManager.RoleExistsAsync(Helper.User).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(Helper.User));
            }
            if (!_roleManager.RoleExistsAsync(Helper.Admin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(Helper.Admin));
            }

            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                // AccountCreated = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Helper.User);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("User was not registered", error.Description);
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine("Registration error: " + error.Description);
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> AccountSettings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new AccountSettingsViewModel
            {
                FirstName = user.Name?.Split(' ').FirstOrDefault() ?? "",
                LastName = user.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                AccountCreated = null, // user.AccountCreated,
                LastLogin = null // user.LastLogin
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(AccountSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AccountSettings", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Update user information
            user.Name = $"{model.FirstName} {model.LastName}".Trim();
            user.Email = model.Email;
            user.UserName = model.Email; // Update username to match email
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Reload the view with updated data
            model.AccountCreated = null; // user.AccountCreated;
            model.LastLogin = null; // user.LastLogin;
            return View("AccountSettings", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(AccountSettingsViewModel model)
        {
            if (string.IsNullOrEmpty(model.CurrentPassword) || 
                string.IsNullOrEmpty(model.NewPassword) || 
                string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "All password fields are required.");
                return View("AccountSettings", model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "New password and confirmation password do not match.");
                return View("AccountSettings", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully!";
                // Clear password fields
                model.CurrentPassword = "";
                model.NewPassword = "";
                model.ConfirmPassword = "";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Reload user data
            model.FirstName = user.Name?.Split(' ').FirstOrDefault() ?? "";
            model.LastName = user.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "";
            model.Email = user.Email ?? "";
            model.PhoneNumber = user.PhoneNumber ?? "";
            model.AccountCreated = null; // user.AccountCreated;
            model.LastLogin = null; // user.LastLogin;

            return View("AccountSettings", model);
        }
    }
}


