using Microsoft.AspNetCore.Mvc;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return Redirect("/Identity/Account/Login");
        }
    }
}
