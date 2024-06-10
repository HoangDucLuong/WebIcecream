using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebIcecream_FE_USER;
using WebIcecream_FE_USER.Models;
using WebIcecream_FE_USER.ViewModels;

namespace WebIcecream_FE_USER.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RecipeController(IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _webHostEnvironment = webHostEnvironment;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
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
        public async Task<IActionResult> Index()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://localhost:7018/api/Recipes/GetRecipes";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    List<RecipeViewModel> productList = JsonConvert.DeserializeObject<List<RecipeViewModel>>(responseData);

                    //ViewBag.ApiData = responseData;
                    return View(productList);

                }
                else
                {
                    return View(new List<RecipeViewModel>());

                }
            }
        }

    }
}
