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

                var userRoleId = GetUserRoleIdFromToken(token.Token);

                if (userRoleId == 2)
                    HttpContext.Session.SetString("Token", token.Token);

                if (userRoleId == 2)
                {

                    ViewData["IsLoggedIn"] = true; 

                    return RedirectToAction("Index", "User"); 
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
            HttpResponseMessage response = await _httpClient.PostAsync("Logout/logout", null);

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.Remove("Token");
            }

            return RedirectToAction("Login");
        }
        private int? GetUserRoleIdFromToken(string token)
        {

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
