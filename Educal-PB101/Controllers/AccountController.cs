using Educal_PB101.Helpers.Enums;
using Educal_PB101.Models;
using Educal_PB101.ViewModels.Account;
using Educal_PB101.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Educal_PB101.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<AppUser> userManager, 
                                 SignInManager<AppUser> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task< IActionResult> SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterVM request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = new()
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.UserName
            };
            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(user, nameof(Roles.Memmber));
            return RedirectToAction("Index","Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginVM request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser existUser = await _userManager.FindByEmailAsync(request.EmailorUsername) ?? await _userManager.FindByNameAsync(request.EmailorUsername);
            if (existUser is null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(existUser, request.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid Password");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        //[HttpGet]
        //public async Task<IActionResult> CreateRoles()
        //{
        //    foreach (var role in Enum.GetValues(typeof(Roles)))
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role.ToString()))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole { Name=role.ToString() });  

        //        }
        //    }
        //    return Ok();
        //}
        
        
    }
}
