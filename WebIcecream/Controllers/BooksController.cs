using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIcecream.Models;
using WebIcecream.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using WebIcecream.Data.Repositories;
using WebIcecream.Service;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IBookRepository _bookRepo;
        private readonly ILogger<BooksController> _logger;
        private readonly ProjectDak3Context _context;

        public BooksController(IFileService fileService, IBookRepository bookRepo, ILogger<BooksController> logger, ProjectDak3Context context)
        {
            _fileService = fileService;
            _bookRepo = bookRepo;
            _logger = logger;
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _context.Books.Select(b => new BookDTO
            {
                BookId = b.BookId,
                Title = b.Title,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                Price = b.Price
            }).ToListAsync();

            return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _context.Books.Select(b => new BookDTO
            {
                BookId = b.BookId,
                Title = b.Title,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                Price = b.Price
            }).FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookDTO>> PostBook([FromForm] BookDTO bookDTO, [FromForm] IFormFile image)
        {
            if (bookDTO == null)
            {
                return BadRequest("Invalid book data");
            }

            var book = new Book
            {
                Title = bookDTO.Title,
                Description = bookDTO.Description,
                Price = bookDTO.Price
            };

            if (image != null && image.Length > 0)
            {
                book.ImageUrl = await SaveImageAsync(image);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            bookDTO.BookId = book.BookId;
            bookDTO.ImageUrl = book.ImageUrl;

            return CreatedAtAction(nameof(GetBook), new { id = bookDTO.BookId }, bookDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromForm] UpdateBookDTO updatebookDTO)
        {
            try
            {
                if (id != updatebookDTO.BookId)
                {
                    return BadRequest("ID in URL and form body does not match.");
                }

                var existingBook = await _bookRepo.FindBookByIdAsync(id);
                if (existingBook == null)
                {
                    return NotFound($"Book with ID: {id} not found");
                }

                string oldImage = existingBook.ImageUrl;
                if (updatebookDTO.ImageFile != null)
                {
                    if (updatebookDTO.ImageFile.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest("File size should not exceed 5 MB");
                    }

                    string[] allowedFileExtensions = { ".jpg", ".jpeg", ".png" };
                    string createdImageName = await _fileService.SaveFileAsync(updatebookDTO.ImageFile, allowedFileExtensions);
                    updatebookDTO.ImageUrl = createdImageName;

                    existingBook.ImageUrl = createdImageName;
                }
                else
                {
                    updatebookDTO.ImageUrl = existingBook.ImageUrl;
                }

                existingBook.Title = updatebookDTO.Title;
                existingBook.Description = updatebookDTO.Description;
                existingBook.Price = updatebookDTO.Price;

                var updatedBook = await _bookRepo.UpdateBookAsync(existingBook);

                if (updatebookDTO.ImageFile != null)
                    _fileService.DeleteFile(oldImage);

                return Ok(updatedBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return null;
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            // Create directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var imageUrl = $"{baseUrl}/images/{fileName}";

            return imageUrl;
        }

    }
}
