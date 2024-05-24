using Microsoft.AspNetCore.Mvc;

namespace WebIcecream_FE.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
