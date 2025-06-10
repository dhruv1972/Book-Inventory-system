using Microsoft.AspNetCore.Mvc;

namespace LibraryCMS.API.Controllers
{
    public class AuthorsPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
