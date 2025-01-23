using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.Models.GravityBookstore;



namespace WebApplication1.Controllers
{
    public class BooksController : Controller
    {
        private readonly GravityContext _context;

        public BooksController(GravityContext context)
        {
            _context = context;
        }

       
        public IActionResult Index(int page = 1, int size = 20)
        {
            return View(model: PagingListAsync<BookViewModel>.Create(
                data: (p, s) => _context.Book
                    .OrderBy(b => b.Title)
                    .Skip((p - 1) * s)
                    .Take(s)
                    .Select(b => new BookViewModel
                    {
                        BookId = b.BookId,
                        Title = b.Title,
                        Isbn13 = b.Isbn13,
                        NumPages = b.NumPages ?? 0,
                        PublicationDate = b.PublicationDate,
                        PublisherId = b.PublisherId, 
                        LanguageId = b.LanguageId, 
                        AuthorsCount = b.Authors.Count(),
                        SoldCopies = b.OrderLines.Count()
                    })
                    .AsAsyncEnumerable(),
                totalItems: _context.Book.Count(),
                number: page,
                size: size
            ));
        }

        
        
        [Authorize]
        public IActionResult Edit(int id)
        {
            
            var book = _context.Book
                .Include(b => b.Language) 
                .FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            
            var model = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Isbn13 = book.Isbn13,
                NumPages = book.NumPages,
                PublicationDate = book.PublicationDate,
                LanguageId = book.LanguageId,
                PublisherId = book.PublisherId
            };

            
            ViewBag.LanguageId = new SelectList(_context.BookLanguages, "LanguageId", "LanguageName", model.LanguageId);
            ViewBag.PublisherId = new SelectList(_context.Publishers, "PublisherId", "PublisherName", model.PublisherId);

            return View(model);
        }

        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, BookViewModel model)
        {
            if (id != model.BookId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    var book = _context.Book.FirstOrDefault(b => b.BookId == id);

                    if (book == null)
                    {
                        return NotFound();
                    }

                    
                    book.Title = model.Title;
                    book.Isbn13 = model.Isbn13;
                    book.NumPages = model.NumPages;
                    book.PublicationDate = model.PublicationDate;

                    
                    _context.Update(book);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
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
            }

            return View(model);
        }
        public IActionResult Authors()
        {
            var authors = _context.Authors
                .Include(a => a.Books) 
                .Select(a => new AuthorViewModel
                {
                    AuthorId = a.AuthorId,
                    AuthorName = a.AuthorName,
                    BookTitles = a.Books.Select(b => new BookViewModel
                    {
                        BookId = b.BookId,
                        Title = b.Title
                    }).ToList()
                })
                .ToList();

            if (!authors.Any())
            {
                return NotFound("No authors found.");
            }

            return View(authors);
        }
        
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var book = new Book
                {
                    Title = model.Title,
                    Isbn13 = model.Isbn13,
                    NumPages = model.NumPages,
                    PublicationDate = model.PublicationDate,
                    LanguageId = model.LanguageId,
                    PublisherId = model.PublisherId
                };

                _context.Book.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.LanguageId = new SelectList(_context.Languages, "LanguageId", "LanguageName", model.LanguageId);
            ViewBag.PublisherId = new SelectList(_context.Publishers, "PublisherId", "PublisherName", model.PublisherId);
            return View(model);
        }
        
    }
}