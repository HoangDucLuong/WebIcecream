using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebIcecream_FE.Models;
using WebIcecream_FE.Models;

namespace WebIcecream_FE.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7018/api/Auth/Login/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
                HttpContext.Session.SetString("Token", tokenResponse.Token);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7018/api/Auth/Register/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Auth");
            }

            ModelState.AddModelError("", "Registration failed");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Login", "Auth");
        }
    }
}
