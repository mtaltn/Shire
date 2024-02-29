using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shire.WebMvc.Contexts;
using Shire.WebMvc.Entities;
using Shire.WebMvc.Models;
using System.Diagnostics;

namespace Shire.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var productList = _context.Product.ToList();

            var categoryList = _context.Categories.ToList();

            ViewBag.Categories = categoryList;

            return View(productList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Category(int categoryId)
        {
            // categoryId'ye göre ürünleri filtrele
            var productsInCategory = _context.Product.Where(p => p.CategoryId == categoryId.ToString()).ToList();

            // Burada ViewBag ya da baþka bir yöntemle kategori adýný view'e geçirebilirsiniz.
            // ViewBag.CategoryName = _context.Categories.FirstOrDefault(c => c.Id == categoryId)?.Name;

            return View(productsInCategory);
        }

        // HomeController.cs

        public IActionResult FilterProducts(int? categoryId)
        {
            var productList = _context.Product.ToList();

            if (categoryId.HasValue)
            {
                productList = productList.Where(p => p.CategoryId == categoryId.ToString()).ToList();
            }

            return PartialView("_ProductListPartial", productList);
        }

        public IActionResult ClearFilters()
        {
            var productList = _context.Product.ToList();
            return PartialView("_ProductListPartial", productList);
        }

        public IActionResult SearchProducts(string searchString)
        {
            var productList = _context.Product.ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                productList = productList.Where(p => p.Name.Contains(searchString)).ToList();
            }

            return PartialView("_ProductListPartial", productList);
        }
    }
}

