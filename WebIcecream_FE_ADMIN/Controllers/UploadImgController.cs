using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace WebIcecream_FE_ADMIN.Controllers
{
	public class UploadImgController : Controller
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public UploadImgController(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpPost]
		public async Task<IActionResult> UploadImages(IFormFile upload)
		{
			if (upload != null && upload.Length > 0)
			{
				
				var fileName = Path.GetFileName(upload.FileName);
				var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

				if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "images")))
				{
					Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "images"));
				}

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await upload.CopyToAsync(stream);
				}

				var request = HttpContext.Request;
				var baseUrl = $"{request.Scheme}://{request.Host}";

				var imageUrl = $"{baseUrl}/images/{fileName}";

				return Json(new { url = imageUrl });
			}

			return Json(new { error = "Upload failed" });
		}
	}
}
