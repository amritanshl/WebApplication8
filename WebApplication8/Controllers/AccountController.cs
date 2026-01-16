using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class AccountController : Controller
    {
        private readonly Amrit01132026Context _context;
        private readonly IConfiguration _configuration;

        public AccountController(Amrit01132026Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // 1. Check Database
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                // 2. Create Claims (User info + ROLE)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role) // This is where we save "Read", "Write", etc.
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecretKey1234567890_VerySecureKeyNeeded"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "https://localhost",
                    audience: "https://localhost",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // 3. Store Token in Cookie (HttpOnly for security)
                Response.Cookies.Append("jwtCookie", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Use only in HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddMinutes(30)
                });

                return RedirectToAction("Index", "Students");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            // Use TempData to pass a message to the next request
            TempData["AuthErrorMessage"] = "You do not have permission to perform this action.";

            // Redirect back to Home or use Request.Headers["Referer"] to go back
            string returnUrl = Request.Headers["Referer"].ToString();
            return !string.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtCookie");
            return RedirectToAction("Login");
        }
    }
}