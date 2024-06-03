using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebIcecream.Service
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions);
        void DeleteFile(string fileName);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentNullException(nameof(imageFile), "File is null or empty.");
            }

            var contentPath = _environment.ContentRootPath;
            var uploadPath = Path.Combine(contentPath, "wwwroot", "images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileExtension = Path.GetExtension(imageFile.FileName);
            var isValidExtension = allowedFileExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

            if (!isValidExtension)
            {
                throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed file extensions.");
            }

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return fileName;
        }

        public void DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "File name is null or empty.");
            }

            var contentPath = _environment.ContentRootPath;
            var filePath = Path.Combine(contentPath, "wwwroot", "images", fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found at {filePath}");
            }

            File.Delete(filePath);
        }
    }
}
