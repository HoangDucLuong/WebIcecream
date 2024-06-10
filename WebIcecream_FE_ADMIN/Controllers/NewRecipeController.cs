using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebIcecream_FE_ADMIN.Models;
using System.Net.Http.Headers;
using X.PagedList;

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

        public async Task<IActionResult> Index(int? page, string searchString, string statusFilter)
        {
            ViewData["IsLoggedIn"] = true;
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatusFilter"] = statusFilter;

            try
            {
                var response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/NewRecipes/GetNewRecipes");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var recipes = JsonConvert.DeserializeObject<List<NewRecipeViewModel>>(data);

                    // Filter by search string
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        recipes = recipes.Where(r => r.Flavor.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    // Filter by status
                    if (!string.IsNullOrEmpty(statusFilter))
                    {
                        recipes = recipes.Where(r => r.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    int pageSize = 5;
                    int pageNumber = (page ?? 1);

                    // Convert List<NewRecipeViewModel> to IPagedList<NewRecipeViewModel>
                    IPagedList<NewRecipeViewModel> pagedList = recipes.ToPagedList(pageNumber, pageSize);

                    return View(pagedList);
                }
                else
                {
                    return View(new List<NewRecipeViewModel>().ToPagedList(1, 10));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(new List<NewRecipeViewModel>().ToPagedList(1, 10));
            }
        }
        [HttpPost]
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

                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/NewRecipes/SearchNewRecipeByName?name={searchName}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var newrecipes = JsonConvert.DeserializeObject<List<NewRecipeViewModel>>(data);

                    // Paging logic
                    int pageSize = 5; // Số lượng sản phẩm trên mỗi trang
                    int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là 1 nếu không có giá trị page

                    // Chia nhỏ danh sách sản phẩm thành từng trang
                    var pagedList = newrecipes.ToPagedList(pageNumber, pageSize);

                    return View("Index", pagedList);
                }
                else
                {
                    TempData["ErrorMessage"] = "No newrecipes found matching the search criteria.";
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
