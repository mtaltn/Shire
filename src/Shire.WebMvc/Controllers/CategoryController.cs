using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shire.WebMvc.Contexts;
using Shire.WebMvc.Entities;
using System.Linq;

namespace Shire.WebMvc.Controllers;

public class CategoryController : Controller
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(string searchString)
    {
        try
        {
            var categories = from m in _context.Categories
                             select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(s => s.Name.Contains(searchString));
            }

            ViewBag.Categories = categories.ToList();

            return View(categories.ToList());
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Veriler alınırken bir hata oluştu.";
            return View(new List<Category>()); 
        }
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
            var category = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
            return View(category);
        }
        catch (Exception)
        {
            throw;
        }
    }
        
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        _context.Categories.Add(category);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        try
        {
            var category = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            throw;
        }
    }


}
