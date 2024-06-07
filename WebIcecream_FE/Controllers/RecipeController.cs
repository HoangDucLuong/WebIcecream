using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebIcecream_FE.Models;

namespace WebIcecream_FE.Controllers
{
    public class RecipeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RecipeController(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
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
