using DAL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BLL.interfaces;

namespace DevExtremeAspNetCoreApp1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
       
        }

        // Login GET
        public IActionResult Login()
        {
            return View(new User());
        }

        // Login POST
        [HttpPost]
        public async Task<IActionResult> Login([Bind("Username,Password")] User user)
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
                var result = await _authService.LoginAsync(user.Username,user.Password);
                if (result.Succeeded)
                {

                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserId", result.User.ID.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
