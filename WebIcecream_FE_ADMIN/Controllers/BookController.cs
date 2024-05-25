using Microsoft.AspNetCore.Mvc;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
