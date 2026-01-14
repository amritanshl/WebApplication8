using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication8.Controllers
{
    public class BaseController : Controller
    {
       public override void OnActionExecuting(ActionExecutingContext filter)
        {
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                filter.Result = new RedirectToActionResult("Login", "Account", null);
            }
            base.OnActionExecuting(filter);
        }
    }
}
