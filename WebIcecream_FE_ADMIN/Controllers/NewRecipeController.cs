using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebIcecream_FE_ADMIN.Models;
using System.Net.Http.Headers;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class NewRecipeController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7018/api");
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NewRecipeController(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["IsLoggedIn"] = true;
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/NewRecipes/GetNewRecipes");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var recipes = JsonConvert.DeserializeObject<List<NewRecipeViewModel>>(data);
                return View(recipes);
            }
            else
            {
                return View(new List<NewRecipeViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IsLoggedIn"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewRecipeViewModel recipe, IFormFile image)
        {
            try
            {
                if (image != null)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";

                    recipe.ImageUrl = $"{baseUrl}/images/{fileName}";
                }

                recipe.Status = "waiting";
                recipe.SubmissionDate = DateTime.Now;

                var content = new StringContent(JsonConvert.SerializeObject(recipe), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/NewRecipes/PostNewRecipe", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Recipe created successfully. Waiting for admin approval.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create recipe.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/NewRecipes/ApproveRecipe/ApproveRecipe/{id}", null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Recipe approved and published successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to approve recipe.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/NewRecipes/DeleteNewRecipe/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Recipe deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete recipe.";
            }

            return RedirectToAction("Index");
        }
    }
}
