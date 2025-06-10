using Microsoft.AspNetCore.Mvc;

namespace LibraryCMS.API.Controllers
{
    public class CategoriesPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
