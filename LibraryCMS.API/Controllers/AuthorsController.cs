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
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AuthorsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            var authors = await _context.Authors.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<AuthorDTO>>(authors));
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDTO>> GetAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();
            return Ok(_mapper.Map<AuthorDTO>(author));
        }

        // POST: api/Authors
        [HttpPost]
        public async Task<ActionResult<AuthorDTO>> CreateAuthor(CreateAuthorDTO dto)
        {
            var author = _mapper.Map<Author>(dto);
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, _mapper.Map<AuthorDTO>(author));
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Authors/{id}/books
        [HttpGet("{id}/books")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByAuthor(int id)
        {
            var author = await _context.Authors
                .Include(a => a.BookAuthors)
                .ThenInclude(ba => ba.Book)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null) return NotFound();

            var books = author.BookAuthors.Select(ba => ba.Book);
            return Ok(_mapper.Map<IEnumerable<BookDTO>>(books));
        }
    }
}
