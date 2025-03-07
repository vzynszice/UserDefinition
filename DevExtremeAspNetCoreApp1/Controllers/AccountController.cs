using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SystemManager.Abstractions.Auth;
using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace DevExtremeAspNetCoreApp1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationManager _authManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAuthenticationManager authManager,
            ILogger<AccountController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View(new UserModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("Username,Password")] UserModel user)
        {
            foreach (var key in ModelState.Keys.ToList())
            {
                if (key != "Username" && key != "Password")
                {
                    ModelState.Remove(key);
                }
            }

            if (ModelState.IsValid)
            {
                // AuthenticationManager zaten gerekli ValidationException 
                // ve BusinessException'ları fırlatacak ve middleware yakalayacak
                var result = await _authManager.LoginAsync(user.Username, user.Password);

                if (result.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim("UserId", result.User.ID.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal);

                    return RedirectToAction("Index", "Home");
                }

                // Başarısız login durumunda ValidationException fırlat
                throw new ValidationException("Invalid login attempt.");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}