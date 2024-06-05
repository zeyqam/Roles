using Educal_PB101.Models;
using Educal_PB101.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Educal_PB101.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var users = _userManager.Users.ToList();


            List<UserRoleVM> userWithRoles = new();
            foreach (var item in users)
            {
                var roles = await _userManager.GetRolesAsync(item);
                string userRoles = String.Join(",", roles.ToArray());
                userWithRoles.Add(new UserRoleVM
                {
                    FullName = item.FullName,
                    Username = item.UserName,
                    Email = item.Email,
                    Roles = userRoles

                });
            }

            return View(userWithRoles);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Users = users;
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError(string.Empty, "User and Role are required.");
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View();
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                ModelState.AddModelError(string.Empty, "Role not found.");
                return View();
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to add role to user.");
                return View();
            }

            return RedirectToAction("Index");
        }
    }
}
