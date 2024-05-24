using Microsoft.AspNetCore.Mvc;

namespace WebIcecream_FE.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
