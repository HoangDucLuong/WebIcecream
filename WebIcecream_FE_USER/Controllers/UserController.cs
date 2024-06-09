using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebIcecream_FE_USER.Models;

namespace WebIcecream_FE_USER.Controllers
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
        public async Task<IActionResult> RenewMembership()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/MembershipPackages/GetMembershipPackages");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var memberships = JsonConvert.DeserializeObject<List<MembershipPackageModel>>(data);
                    ViewData["Membership"] = memberships;

                    return View(memberships); // Pass memberships to the view
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve Membership packages.";
                    return View(); // Return the view even if data retrieval fails
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving Membership packages: {ex.Message}";
                return View(); // Handle any exceptions and return the view
            }
        }


        [HttpPost]
        public async Task<IActionResult> RenewMembership(RenewMembershipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetUserIdFromToken();

            if (userId == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to error page or handle as needed
            }

            // Call API to renew membership
            var response = await _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}/User/RenewMembership/{userId}", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details"); // Redirect to details page after successful renewal
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Renewal failed: {errorResponse}");
                return View(model);
            }
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
                return RedirectToAction("Details");
            }

            // Xử lý lỗi nếu không thành công
            return View("Error");
        }

        private string GetUserIdFromToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");

            if (token == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            return jsonToken.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
        }
    }
}
