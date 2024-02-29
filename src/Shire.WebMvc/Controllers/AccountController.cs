using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Shire.WebMvc.Dtos;
using Shire.WebMvc.Entities;

namespace Shire.WebMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(userLoginDto.Email);

                if (user != null)
                {
                    // Kullanıcı bulundu, parola doğrulamasını yapın
                    var result = await _signInManager.PasswordSignInAsync(user, userLoginDto.Password,
                        userLoginDto.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        // Başarılı giriş durumu
                        return RedirectToAction("Index", "Home");
                    }

                    if (result.IsLockedOut)
                    {
                        // Kullanıcı hesabı kilitli
                        ModelState.AddModelError("", "Hesabınız kilitlendi. Lütfen bir süre sonra tekrar deneyin.");
                    }
                    else
                    {
                        // Giriş başarısız, hata mesajı ekle
                        ModelState.AddModelError("", "E-posta adresiniz veya şifreniz yanlıştır.");
                    }
                }
                else
                {
                    // Kullanıcı bulunamadı
                    ModelState.AddModelError("", "Bu e-posta adresine sahip bir kullanıcı bulunamadı.");
                }

            }

            // ModelState.IsValid false olduğunda buraya gelir
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserAddDto userRegisterDto)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adının veya e-posta adresinin daha önce kullanılıp kullanılmadığını kontrol et
                var existingUser = await _userManager.FindByNameAsync(userRegisterDto.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılmaktadır.");
                    return View();
                }

                existingUser = await _userManager.FindByEmailAsync(userRegisterDto.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu e-posta adresi zaten kullanılmaktadır.");
                    return View();
                }

                // Yukarıdaki kontrollerden geçerse yeni kullanıcı oluştur
                var user = new User
                {
                    UserName = userRegisterDto.UserName,
                    Email = userRegisterDto.Email,
                    PasswordHash = userRegisterDto.Password,
                };

                var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
            }
            else
            {
                return View();
            }
        }


        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            // Kullanıcı bilgilerini al ve profili görüntüle
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;

            if (user == null)
            {
                return NotFound();
            }

            var model = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                // Diğer kullanıcı bilgilerini ekleyebilirsiniz
            };

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

                if (result.Succeeded)
                {
                    // Şifre değiştirme başarılı
                    return RedirectToAction("Profile", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // ModelState.IsValid false olduğunda veya işlem başarısız olduğunda buraya gelir
            return View(changePasswordDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
