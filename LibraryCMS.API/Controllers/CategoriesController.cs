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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryDTO>>(categories));
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return Ok(_mapper.Map<CategoryDTO>(category));
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CreateCategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, _mapper.Map<CategoryDTO>(category));
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Categories/{id}/books
        [HttpGet("{id}/books")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.BookCategories)
                .ThenInclude(bc => bc.Book)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            var books = category.BookCategories.Select(bc => bc.Book);
            return Ok(_mapper.Map<IEnumerable<BookDTO>>(books));
        }

        // POST: api/Categories/{id}/books
        [HttpPost("{id}/books")]
        public async Task<IActionResult> AddBookToCategory(int id, [FromBody] int bookId)
        {
            var category = await _context.Categories.Include(c => c.BookCategories).FirstOrDefaultAsync(c => c.Id == id);
            var book = await _context.Books.FindAsync(bookId);

            if (category == null || book == null) return NotFound();

            if (!category.BookCategories.Any(bc => bc.BookId == bookId))
            {
                category.BookCategories.Add(new BookCategory { BookId = bookId, CategoryId = id });
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
