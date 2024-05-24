using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;
using WebIcecream_FE.Models;

namespace WebIcecream_FE.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7018/api");
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            // Lấy UserId từ token
            var userId = GetUserIdFromToken();

            if (userId == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to error page or handle as needed
            }

            // Gửi yêu cầu lấy thông tin người dùng từ API backend
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/User/GetUser/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var userViewModel = JsonConvert.DeserializeObject<UserViewModel>(userJson);
                return View(userViewModel);
            }

            return View("Error");
        }

        private string GetUserIdFromToken()
        {
            // Lấy token từ HttpContext (ví dụ lấy từ cookie hoặc từ các phương thức khác)
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");

            if (token == null)
            {
                return null;
            }

            // Giải mã token để lấy thông tin người dùng
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Lấy UserId từ token
            return jsonToken.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
        }
    }
}
