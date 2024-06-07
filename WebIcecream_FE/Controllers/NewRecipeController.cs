using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebIcecream_FE.Models;
using System.IO;

namespace WebIcecream_FE.Controllers
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

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(recipe.UserId.ToString()), "UserId");
                    content.Add(new StringContent(recipe.Flavor), "Flavor");
                    content.Add(new StringContent(recipe.Ingredients), "Ingredients");
                    content.Add(new StringContent(recipe.Procedure), "Procedure");
                    content.Add(new StringContent(recipe.ImageUrl), "ImageUrl");
                    content.Add(new StringContent(recipe.SubmissionDate.ToString("o")), "SubmissionDate");
                    content.Add(new StringContent(recipe.Status), "Status");

                    if (image != null)
                    {
                        var fileContent = new StreamContent(image.OpenReadStream());
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "image",
                            FileName = image.FileName
                        };
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                        content.Add(fileContent);
                    }

                    var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/NewRecipes/PostNewRecipe", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Recipe created successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to create recipe.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/NewRecipes/GetNewRecipe/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var recipe = JsonConvert.DeserializeObject<NewRecipeViewModel>(data);
                return View(recipe);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NewRecipeViewModel recipe, IFormFile image)
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

                    recipe.ImageUrl = fileName;
                }

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(recipe.RecipeId.ToString()), "RecipeId");
                    content.Add(new StringContent(recipe.Flavor), "Flavor");
                    content.Add(new StringContent(recipe.Ingredients), "Ingredients");
                    content.Add(new StringContent(recipe.Procedure), "Procedure");
                    content.Add(new StringContent(recipe.ImageUrl), "ImageUrl");
                    content.Add(new StringContent(recipe.UserId.ToString()), "UserId");
                    content.Add(new StringContent(recipe.SubmissionDate.ToString("o")), "SubmissionDate");
                    content.Add(new StringContent(recipe.Status), "Status");

                    if (image != null)
                    {
                        var fileContent = new StreamContent(image.OpenReadStream());
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "ImageFile",
                            FileName = image.FileName
                        };
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                        content.Add(fileContent);
                    }

                    var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/NewRecipes/PutNewRecipe/{recipe.RecipeId}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Recipe updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update recipe.";
                    }
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
