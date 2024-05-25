using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebIcecream_FE_ADMIN.Models;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7018/api/Auth/");
        }

        // Hiển thị form đăng nhập
        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("Login/login", content);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<TokenResponse>(data);

                // Lưu token vào session hoặc cookie
                HttpContext.Session.SetString("Token", token.Token);

                // Kiểm tra vai trò của người dùng
                var userRoleId = GetUserRoleIdFromToken();

                // Kiểm tra xem vai trò có phù hợp không
                if (userRoleId == 2)
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
                return View(login);
            }
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Gửi yêu cầu đến endpoint 'logout' của API backend để đăng xuất
            HttpResponseMessage response = await _httpClient.PostAsync("Logout/logout", null);

            if (response.IsSuccessStatusCode)
            {
                // Xóa token từ session hoặc cookie
                HttpContext.Session.Remove("Token");
            }

            return RedirectToAction("Login");
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
