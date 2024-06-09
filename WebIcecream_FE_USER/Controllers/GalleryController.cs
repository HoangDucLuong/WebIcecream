using Microsoft.AspNetCore.Mvc;

namespace WebIcecream_FE_USER.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
