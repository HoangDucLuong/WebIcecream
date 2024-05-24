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
        [HttpGet]
        public async Task<IActionResult> Edit()
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
                return View(userViewModel); // Trả về view để chỉnh sửa thông tin người dùng
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel); // Nếu dữ liệu không hợp lệ, trả về lại form để người dùng sửa lại
            }

            var userId = GetUserIdFromToken();

            if (userId == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to error page or handle as needed
            }

            // Gửi yêu cầu PUT lên API backend để cập nhật thông tin người dùng
            var response = await _httpClient.PutAsJsonAsync($"{_httpClient.BaseAddress}/User/PutUser/{userId}", userViewModel);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details"); // Chuyển hướng về trang chi tiết sau khi cập nhật thành công
            }

            // Xử lý lỗi nếu không thành công
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
