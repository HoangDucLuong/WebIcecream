using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIcecream.Models;
using WebIcecream.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ProjectDak3Context _context;

        public BooksController(ProjectDak3Context context)
        {
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

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookDTO bookDTO)
        {
            if (id != bookDTO.BookId)
            {
                return BadRequest();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.Title = bookDTO.Title;
            book.Description = bookDTO.Description;
            book.ImageUrl = bookDTO.ImageUrl;
            book.Price = bookDTO.Price;

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookDTO>> PostBook(BookDTO bookDTO)
        {
            var book = new Book
            {
                Title = bookDTO.Title,
                Description = bookDTO.Description,
                ImageUrl = bookDTO.ImageUrl,
                Price = bookDTO.Price
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            bookDTO.BookId = book.BookId;

            return CreatedAtAction(nameof(GetBook), new { id = bookDTO.BookId }, bookDTO);
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
    }
}
