using Microsoft.AspNetCore.Mvc;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (model.Email == "admin@company.com" && model.Password == "Admin@123")
            {
                HttpContext.Session.SetString("UserEmail", model.Email);
                return RedirectToAction("Index", "Students");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

    }
}
