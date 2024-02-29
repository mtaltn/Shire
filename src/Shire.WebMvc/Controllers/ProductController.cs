using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shire.WebMvc.Contexts;
using Shire.WebMvc.Entities;

namespace Shire.WebMvc.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(string searchString)
    {        
        // Kullanıcı giriş yapmışsa işlemlere devam et
        var product = from m in _context.Product
                      select m;

        if (!String.IsNullOrEmpty(searchString))
        {
            product = product.Where(s => s.Name.Contains(searchString));
        }

        return View(product);
    }


    public IActionResult Details()
    {

        return View();
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        try
        {
            var category = _context.Product.Where(x => x.Id == id).FirstOrDefault();
            return View(category);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        try
        {
            // Yetki gerektiren işlemler
            return View();
        }
        catch (Exception)
        {
            // Hata durumunda yetki hatası sayfasına yönlendir
            return RedirectToAction("AccessDenied", "Account");
        }
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(Product product)
    {
        _context.Product.Add(product);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        try
        {
            var product = _context.Product.Where(x => x.Id == id).FirstOrDefault();
            _context.Product.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction("AccessDenied", "Account");
        }
    }

    [HttpGet]
    public IActionResult FilterProducts(int? categoryId)
    {
        var productList = _context.Product.ToList();

        if (categoryId.HasValue)
        {
            productList = productList.Where(p => p.CategoryId == categoryId.ToString()).ToList();
        }

        ViewBag.Categories = _context.Categories.ToList();

        return PartialView("_ProductListPartial", productList);
    }


}
