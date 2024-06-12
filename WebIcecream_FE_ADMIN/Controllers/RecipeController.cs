﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebIcecream_FE_ADMIN.Models;
using X.PagedList;
using System.IO;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class RecipeController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7018/api");
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RecipeController(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int? page, string searchString)
        {
            ViewData["IsLoggedIn"] = true;
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Recipes/GetRecipes");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var recipes = JsonConvert.DeserializeObject<List<RecipeViewModel>>(data);

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        recipes = recipes.Where(p => p.Flavor.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    int pageSize = 5; 
                    int pageNumber = (page ?? 1);

                    var pagedList = recipes.ToPagedList(pageNumber, pageSize);

                    return View(pagedList);
                }
                else
                {
                    return View(new List<RecipeViewModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(new List<ProductViewModel>());
            }
        }
        public async Task<IActionResult> Search(string searchName, int? page)
        {
            try
            {
                ViewData["IsLoggedIn"] = true;

                // Validate input
                if (string.IsNullOrEmpty(searchName))
                {
                    return RedirectToAction("Index");
                }

                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Books/SearchBooksByName?name={searchName}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var recipes = JsonConvert.DeserializeObject<List<RecipeViewModel>>(data);

                    int pageSize = 5; 
                    int pageNumber = (page ?? 1); 

                    
                    var pagedList = recipes.ToPagedList(pageNumber, pageSize);

                    return View("Index", pagedList);
                }
                else
                {
                    TempData["ErrorMessage"] = "No products found matching the search criteria.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index");
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

                    var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Recipes/PostRecipe", content);

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
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Recipes/GetRecipe/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var recipe = JsonConvert.DeserializeObject<RecipeViewModel>(data);
                return View(recipe);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RecipeViewModel recipe, IFormFile image)
        {
            try
            {
                string oldImageUrl = recipe.ImageUrl;

                if (image == null)
                {
                    ModelState.AddModelError(nameof(RecipeViewModel.Image), "Please choose an image.");
                    return View("Edit", recipe); 
                }

                var fileName = Path.GetFileName(image.FileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                recipe.ImageUrl = $"{baseUrl}/images/{fileName}";

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(recipe.RecipeId.ToString()), "RecipeId");
                    content.Add(new StringContent(recipe.Flavor), "Flavor");
                    content.Add(new StringContent(recipe.Ingredients), "Ingredients");
                    content.Add(new StringContent(recipe.Procedure), "Procedure");
                    content.Add(new StringContent(recipe.ImageUrl), "ImageUrl");

                    var fileContent = new StreamContent(image.OpenReadStream());
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "ImageFile",
                        FileName = image.FileName
                    };
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                    content.Add(fileContent);

                    var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Recipes/PutRecipe/{recipe.RecipeId}", content);

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
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Recipes/DeleteRecipe/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }
            else
            {
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}
