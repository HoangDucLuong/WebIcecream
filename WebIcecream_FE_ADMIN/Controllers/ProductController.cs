using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebIcecream_FE_ADMIN.Models;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class ProductController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7018/api");
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["IsLoggedIn"] = true;
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Books/GetBooks");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductViewModel>>(data);
                return View(products);
            }
            else
            {
                return View(new List<ProductViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IsLoggedIn"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel product, IFormFile image)
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

                    // Get the base URL of the application
                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";

                    // Combine the base URL with the relative path to create the full URL
                    product.ImageUrl = $"{baseUrl}/images/{fileName}";
                }

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(product.Title), "Title");
                    content.Add(new StringContent(product.Description), "Description");
                    content.Add(new StringContent(product.Price.ToString()), "Price");
                    content.Add(new StringContent(product.ImageUrl), "ImageUrl");
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

                    var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Books/PostBook", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Product created successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to create product.";
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
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Books/DeleteBook/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete product.";
            }

            return RedirectToAction("Index");
        }

    }
}
