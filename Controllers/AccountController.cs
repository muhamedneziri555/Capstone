using CarpetStore.Data;
using CarpetStore.Models;
using CarpetStore.Utility;
using CarpetStore.ViewModels;
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
                Name = model.Name
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
    }
}


