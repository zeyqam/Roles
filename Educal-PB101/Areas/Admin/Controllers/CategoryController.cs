using Educal_PB101.Data;
using Educal_PB101.Helpers.Extensions;
using Educal_PB101.Models;
using Educal_PB101.Services.Interfaces;
using Educal_PB101.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Educal_PB101.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="SuperAdmin")]
    public class CategoryController : Controller
    {
        public readonly AppDbContext _context;
        public readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context,ICategoryService category, IWebHostEnvironment env)
        {
            _context = context;
            _categoryService = category;
            _env = env;

        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool existCategory = await _categoryService.ExistAsync(category.Name);
            if (existCategory)
            {
                ModelState.AddModelError("Name", "This category already exist");
                return View();
            }
            
            await _categoryService.CreateAsync(new Category { Name = category.Name});
            return RedirectToAction(nameof(Index));
        }

        
    }
}
