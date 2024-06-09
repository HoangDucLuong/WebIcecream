using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebIcecream_FE_USER.Models;

namespace WebIcecream_FE_USER.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangePasswordController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7018/api");
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUsernameFromToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name");

                if (usernameClaim != null)
                {
                    return usernameClaim.Value;
                }
            }

            return null;
        }
        private int? GetUserIdFromToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }

            return null;
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewData["IsLoggedIn"] = true;
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var username = GetUsernameFromToken();
            ViewBag.Username = username; // Gán tên người dùng vào ViewBag
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ViewData["IsLoggedIn"] = true;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                var username = GetUsernameFromToken();

                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login", "Auth");
                }

                model.Username = username; // Thêm username vào model

                // Prepare the request payload
                var jsonModel = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

                // Send the request to the API endpoint
                HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Account/ChangePasswordByUsername", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "User"); // Redirect to member details page after successful password change
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["errorMessage"] = "Failed to change password.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View("Error");
            }
        }
    }
}
