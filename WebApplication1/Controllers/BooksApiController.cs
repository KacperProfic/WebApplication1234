using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.GravityBookstore;

namespace WebApplication1.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly GravityContext _context;

        public BooksApiController(GravityContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = _context.Book
                .Include(b => b.Authors)
                .Include(b => b.OrderLines)
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    b.Isbn13,
                    b.NumPages,
                    b.PublicationDate,
                    b.LanguageId,
                    b.PublisherId,
                    AuthorsCount = b.Authors.Count(),
                    SoldCopies = b.OrderLines.Count()
                })
                .ToList();

            return Ok(books);
        }

        
        [HttpGet("{id}")]
        public IActionResult GetBook(int id)
        {
            var book = _context.Book
                .Include(b => b.Authors)
                .Include(b => b.OrderLines)
                .Where(b => b.BookId == id)
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    b.Isbn13,
                    b.NumPages,
                    b.PublicationDate,
                    b.LanguageId,
                    b.PublisherId,
                    AuthorsCount = b.Authors.Count(),
                    SoldCopies = b.OrderLines.Count()
                })
                .FirstOrDefault();

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Book.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Book.Any(b => b.BookId == id))
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
        
        [HttpGet("authors")]
        public IActionResult GetAuthors()
        {
            var authors = _context.Authors
                .Include(a => a.Books)
                .Select(a => new
                {
                    a.AuthorId,
                    a.AuthorName,
                    Books = a.Books.Select(b => new { b.BookId, b.Title }).ToList()
                })
                .ToList();

            return Ok(authors);
        }
        
    }
}