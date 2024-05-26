using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
                string data = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<TokenResponse>(data);

                // Lưu token vào session hoặc cookie
                HttpContext.Session.SetString("Token", token.Token);
                HttpContext.Session.SetString("Username", model.Username);

                // Kiểm tra vai trò của người dùng
                var userRoleId = GetUserRoleIdFromToken();

                // Kiểm tra xem vai trò có phù hợp không
                if (userRoleId == 1)
                {
                    // Cập nhật trạng thái đăng nhập sau khi đăng nhập thành công
                    ViewData["IsLoggedIn"] = true;

                    return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính sau khi đăng nhập thành công
                }
                else
                {
                    TempData["ErrorMessage"] = "Access denied. You do not have permission to access this page.";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                ModelState.AddModelError("Password", "Invalid password.");
                return View(model);
            }
  
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
        private int? GetUserRoleIdFromToken()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var roleIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");

                if (roleIdClaim != null && int.TryParse(roleIdClaim.Value, out int roleId))
                {
                    return roleId;
                }
            }

            return null;
        }
    }
}
