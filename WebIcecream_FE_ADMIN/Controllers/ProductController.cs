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
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Books/GetBook/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var book = JsonConvert.DeserializeObject<ProductViewModel>(data);
                return View(book);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel book, IFormFile image)
        {
            try
            {
                // Lấy lại ImageUrl cũ từ TempData nếu không có hình mới
                if (TempData.ContainsKey("OldImageUrl") && book.KeepCurrentImage)
                {
                    book.ImageUrl = TempData["OldImageUrl"].ToString();
                }

                // Kiểm tra xem có tải lên hình mới không
                if (image != null)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Lưu tên file hình mới vào book.ImageUrl
                    book.ImageUrl = fileName;
                }

                // Nếu không có hình mới và không chọn giữ nguyên hình cũ
                if (image == null && !book.KeepCurrentImage)
                {
                    ModelState.AddModelError(nameof(ProductViewModel.Image), "Please choose an image.");
                    return View("Edit", book); // Trả về view "Edit" với model hiện tại để người dùng chọn hình ảnh
                }

                // Chuẩn bị nội dung cho yêu cầu PUT
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(book.BookId.ToString()), "BookId");
                    content.Add(new StringContent(book.Title), "Title");
                    content.Add(new StringContent(book.Description), "Description");
                    content.Add(new StringContent(book.Price.ToString()), "Price");
                    content.Add(new StringContent(book.ImageUrl), "ImageUrl");

                    // Nếu có tải lên hình mới thì thêm vào content
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

                    // Gửi yêu cầu PUT để cập nhật thông tin sách
                    var apiUrl = $"{_httpClient.BaseAddress}/Books/PutBook/{book.BookId}";
                    var response = await _httpClient.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Book updated successfully.";
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = $"Failed to update book. Status code: {response.StatusCode}, Error: {errorMessage}";
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
