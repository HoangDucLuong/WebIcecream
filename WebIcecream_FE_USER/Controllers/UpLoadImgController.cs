using Microsoft.AspNetCore.Mvc;

namespace WebIcecream_FE_USER.Controllers
{
    public class UpLoadImgController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

		public UpLoadImgController(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpPost]
		public async Task<IActionResult> UploadImages(IFormFile upload)
		{
			if (upload != null && upload.Length > 0)
			{
				// Lấy tên file và đường dẫn lưu file
				var fileName = Path.GetFileName(upload.FileName);
				var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

				// Tạo folder nếu chưa tồn tại
				if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "images")))
				{
					Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "images"));
				}

				// Lưu file vào đường dẫn đã chỉ định
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await upload.CopyToAsync(stream);
				}

				// Lấy base URL của ứng dụng
				var request = HttpContext.Request;
				var baseUrl = $"{request.Scheme}://{request.Host}";

				// Kết hợp base URL với đường dẫn tương đối để tạo URL đầy đủ
				var imageUrl = $"{baseUrl}/images/{fileName}";

				return Json(new { url = imageUrl });
			}

			return Json(new { error = "Upload failed" });
		}
    }
}
