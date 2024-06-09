using WebIcecream.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebIcecream.Data.Repositories
{
    public interface IBookRepository
    {
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<Book?> FindBookByIdAsync(int id);
        Task DeleteBookAsync(Book book);
    }

    public class BookRepository : IBookRepository
    {
        private readonly ProjectDak3Context _context;

        public BookRepository(ProjectDak3Context context)
        {
            _context = context;
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task DeleteBookAsync(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<Book?> FindBookByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            return book;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }
    }
}
