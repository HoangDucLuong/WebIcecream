using Microsoft.AspNetCore.Mvc;

namespace WebIcecream_FE.Controllers
{
    public class RecipeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
