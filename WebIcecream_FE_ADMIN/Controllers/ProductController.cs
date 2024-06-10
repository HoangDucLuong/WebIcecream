using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebIcecream_FE_ADMIN.Models;
using X.PagedList;

namespace WebIcecream_FE_ADMIN.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7018/api");
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int? page, string searchString)
        {
            ViewData["IsLoggedIn"] = true;
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Books/GetBooks");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<ProductViewModel>>(data);

                    // Filter by search string
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        products = products.Where(p => p.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    // Paging logic
                    int pageSize = 5; // Số lượng sản phẩm trên mỗi trang
                    int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là 1 nếu không có giá trị page

                    // Chia nhỏ danh sách sản phẩm thành từng trang
                    var pagedList = products.ToPagedList(pageNumber, pageSize);

                    return View(pagedList);
                }
                else
                {
                    return View(new List<ProductViewModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
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

                    var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Books/PostBook", content);

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
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["IsLoggedIn"] = true;
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Books/GetBook/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ProductViewModel>(data);
                    return View(product);
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to retrieve product. Status code: {response.StatusCode}";
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
        public async Task<IActionResult> Edit(ProductViewModel product, IFormFile image)
        {
            ViewData["IsLoggedIn"] = true;
            try
            {
                // Keep the old ImageUrl if the user chooses to retain the current image
                if (TempData.ContainsKey("OldImageUrl") && product.KeepCurrentImage)
                {
                    product.ImageUrl = TempData["OldImageUrl"].ToString();
                }

                if (image != null)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    product.ImageUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/images/{fileName}";
                }

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(product.BookId.ToString()), "BookId");
                    content.Add(new StringContent(product.Title), "Title");
                    content.Add(new StringContent(product.Description), "Description");
                    content.Add(new StringContent(product.Price.ToString()), "Price");
                    content.Add(new StringContent(product.ImageUrl), "ImageUrl");

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

                    var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Books/PutBook/{product.BookId}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Product updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Failed to update product. Status code: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
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

                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Books/SearchBooksByName?name={searchName}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<MembershipModel>>(data);

                    // Paging logic
                    int pageSize = 5; // Số lượng sản phẩm trên mỗi trang
                    int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là 1 nếu không có giá trị page

                    // Chia nhỏ danh sách sản phẩm thành từng trang
                    var pagedList = products.ToPagedList(pageNumber, pageSize);

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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
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
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
