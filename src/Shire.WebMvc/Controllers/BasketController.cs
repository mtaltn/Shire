using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Shire.WebMvc.Contexts;
using Shire.WebMvc.Entities;

namespace Shire.WebMvc.Controllers;
[Authorize]
public class BasketController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public BasketController(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index()
    {
        User LoggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;

        return View(new { id = LoggedInUser.Id });
    }

    [HttpGet]
    public IActionResult Index(string userId)
    {
        // Sepet bilgilerini çek
        var basketInfo = _context.Basket
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                Quantity = group.Sum(x => x.Quantity),
                Price = group.Sum(x => x.Price)
            })
            .ToList();

        // Product bilgilerini çek
        var products = _context.Product
            .Where(p => basketInfo.Select(b => b.ProductId).Contains(p.Id))
            .ToList();

        // Sepet bilgileri ile Product bilgilerini birleştir
        var updatedBasket = basketInfo
            .Join(products,
                basketItem => basketItem.ProductId,
                product => product.Id,
                (basketItem, product) => new Basket
                {
                    ProductId = basketItem.ProductId,
                    Quantity = basketItem.Quantity,
                    UserId = userId,
                    Price = basketItem.Price,
                    Product = product
                })
            .ToList();

        // Mevcut sepet bilgilerini temizle ve güncellenmiş sepet bilgilerini ekle
        _context.Basket.RemoveRange(_context.Basket.Where(x => x.UserId == userId));
        _context.Basket.AddRange(updatedBasket);
        _context.SaveChanges();

        return View(updatedBasket);
    }


    public IActionResult Details()
    {

        return View();
    }

    [HttpGet]
    public IActionResult Details(int productId)
    {
        try
        {
            return RedirectToAction("Details", "Product", new { id = productId });
        }
        catch (Exception)
        {
            throw;
        }
    }

    public IActionResult Delete(int id)
    {
        try
        {
            User LoggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;

            var product = _context.Basket.Where(x => x.Id == id).FirstOrDefault();
            _context.Basket.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index", new { userId = LoggedInUser.Id });
        }
        catch (Exception)
        {
            throw;
        }
    }

    public IActionResult Edit(int id)
    {
        try
        {
            var basket = _context.Basket.FirstOrDefault(x => x.Id == id);
            if (basket is null)
            {
                return NotFound();
            }

            return View(basket);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut]
    public IActionResult Edit(int id, [FromBody] Basket updatedBasket)
    {
        try
        {
            var existingBasket = _context.Basket.FirstOrDefault(x => x.Id == id);
            if (existingBasket == null)
            {
                return NotFound();
            }

            existingBasket.Quantity = updatedBasket.Quantity;
            _context.Basket.Update(existingBasket);
            _context.SaveChanges();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    public IActionResult AddBasket(int productId)
    {
        try
        {
            User LoggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;

            var userBasket = _context.Basket
                        .Where(b => b.UserId == LoggedInUser.Id && b.ProductId == productId)
                        .FirstOrDefault();
            var product = _context.Product.Where(x => x.Id == productId).FirstOrDefault();
            if (userBasket != null)
            {
                userBasket.Quantity++;
                _context.Update(userBasket);
            }
            else
            {
                var newBasketItem = new Basket
                {
                    UserId = LoggedInUser.Id,
                    ProductId = productId,
                    Quantity = 1,
                    Price = product.Price,

                };

                _context.Basket.Add(newBasketItem);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", new { userId = LoggedInUser.Id});
        }
        catch (Exception ex)
        {
            // Hata durumunda işlemi geri al
            ModelState.AddModelError("", "Ürün sepete eklenirken bir hata oluştu.");
            return View();
        }

    }
}
