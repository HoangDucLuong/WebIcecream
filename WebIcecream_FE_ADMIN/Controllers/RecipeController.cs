using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WebIcecream_FE_ADMIN.Models;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class RecipeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RecipeController(IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7018/api");
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["IsLoggedIn"] = true;
            try
            {
                var response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Recipes/GetRecipes");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var recipes = JsonConvert.DeserializeObject<List<RecipeViewModel>>(data);
                    return View(recipes);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to fetch recipes.";
                    return View(new List<RecipeViewModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(new List<RecipeViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IsLoggedIn"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RecipeViewModel recipe, IFormFile image)
        {
            try
            {
                if (recipe.Image != null)
                {
                    var fileName = Path.GetFileName(recipe.Image.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await recipe.Image.CopyToAsync(stream);
                    }

                            var request = HttpContext.Request;
                            var baseUrl = $"{request.Scheme}://{request.Host}";

                            recipe.ImageUrl = $"{baseUrl}/images/{fileName}";
                }
                else
                {
                    // If no image is uploaded, set ImageUrl to an empty string
                    recipe.ImageUrl = "";
                }

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(recipe.Flavor), "Flavor");
                    content.Add(new StringContent(recipe.Ingredients), "Ingredients");
                    content.Add(new StringContent(recipe.Procedure), "Procedure");
                    content.Add(new StringContent(recipe.ImageUrl), "ImageUrl");

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

                    var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Recipes/PostRecipes", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Recipe created successfully.";
                        return RedirectToAction("Index");
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
            ViewData["IsLoggedIn"] = true;
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Recipes/GetRecipe/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var recipe = JsonConvert.DeserializeObject<RecipeViewModel>(data);
                    return View(recipe);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to fetch recipe.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RecipeViewModel recipe, IFormFile image)
        {
            try
            {
                // Fetch the existing recipe
                var existingRecipe = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Recipes/GetRecipe/{recipe.RecipeId}");
                if (!existingRecipe.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to fetch the recipe.";
                    return View(recipe);
                }

                // Get existing recipe data
                var existingRecipeData = JsonConvert.DeserializeObject<RecipeViewModel>(await existingRecipe.Content.ReadAsStringAsync());

                // Update fields
                existingRecipeData.Flavor = recipe.Flavor;
                existingRecipeData.Ingredients = recipe.Ingredients;
                existingRecipeData.Procedure = recipe.Procedure;

                // Update image if a new one is uploaded
                if (image != null && image.Length > 0)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    existingRecipeData.ImageUrl = $"{baseUrl}/images/{fileName}";
                }

                // Send updated data to the API
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(existingRecipeData.RecipeId.ToString()), "RecipeId");
                    content.Add(new StringContent(existingRecipeData.Flavor), "Flavor");
                    content.Add(new StringContent(existingRecipeData.Ingredients), "Ingredients");
                    content.Add(new StringContent(existingRecipeData.Procedure), "Procedure");
                    content.Add(new StringContent(existingRecipeData.ImageUrl), "ImageUrl");

                    if (image != null)
                    {
                        var fileContent = new StreamContent(image.OpenReadStream());
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "Image",
                            FileName = image.FileName
                        };
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                        content.Add(fileContent);
                    }

                    var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Recipes/PutRecipes/{recipe.RecipeId}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Recipe updated successfully.";
                        return RedirectToAction("Index");
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

            return View(recipe);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Recipes/DeleteRecipes/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Recipe deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete recipe.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
