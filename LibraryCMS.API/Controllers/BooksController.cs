using AutoMapper;
using LibraryCMS.API.Data;
using LibraryCMS.API.DTOs;
using LibraryCMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryCMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _context.Books.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<BookDTO>>(books));
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return Ok(_mapper.Map<BookDTO>(book));
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookDTO>> CreateBook(CreateBookDTO dto)
        {
            var book = _mapper.Map<Book>(dto);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, _mapper.Map<BookDTO>(book));
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Books/{id}/authors
        [HttpPost("{id}/authors")]
        public async Task<IActionResult> AddAuthorToBook(int id, [FromBody] int authorId)
        {
            var book = await _context.Books.Include(b => b.BookAuthors).FirstOrDefaultAsync(b => b.Id == id);
            var author = await _context.Authors.FindAsync(authorId);

            if (book == null || author == null) return NotFound();

            if (!book.BookAuthors.Any(ba => ba.AuthorId == authorId))
            {
                book.BookAuthors.Add(new BookAuthor { BookId = id, AuthorId = authorId });
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        // GET: api/Books/{id}/authors
        [HttpGet("{id}/authors")]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetBookAuthors(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            var authors = book.BookAuthors.Select(ba => ba.Author);
            return Ok(_mapper.Map<IEnumerable<AuthorDTO>>(authors));
        }
    }
}
