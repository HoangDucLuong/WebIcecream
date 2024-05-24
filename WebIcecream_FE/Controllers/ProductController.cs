using Microsoft.AspNetCore.Mvc;

namespace WebIcecream_FE.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
