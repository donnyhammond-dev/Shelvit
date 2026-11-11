using BackLog.Data;
using BackLog.Enums;
using BackLog.Helpers;
using BackLog.Models;
using BackLog.Services.Interfaces.Books;
using BackLog.ViewModels.BookViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackLog.ControllersS
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookImageService _imageService;
        private readonly IBookStatsService _statsService;
        private readonly IBookSearchService _searchService;

        private string _userId => _userManager.GetUserId(User)!;

        public BooksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            IBookImageService imageService, IBookStatsService statsService, IBookSearchService searchService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _statsService = statsService;
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(BookIndexViewModel vm, int pageNumber = 1)
        {
            //Generate Library Sharing Token
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(user!.BookShareToken))
            {
                user!.BookShareToken = ShareTokenHelper.Generate();
                await _userManager.UpdateAsync(user);
            }

            //Base Query
            var query = _context.Books.Where(x => x.UserId == _userId);

            //Empty State Check
            vm.HasAnyBooks = await query.AnyAsync();   

            //Query Filters
            if (!string.IsNullOrEmpty(vm.Search))
            {
                query = query.Where(x => x.Title.Contains(vm.Search) || x.Author.Contains(vm.Search));
            }

            if (vm.Genre.HasValue)
                query = query.Where(x => x.Genre == vm.Genre.Value);

            if (vm.Format.HasValue)
                query = query.Where(x => x.Format == vm.Format.Value);

            if (vm.Status.HasValue)
                query = query.Where(x => x.Status == vm.Status.Value);

            //Build Dropdown Lists & Share Token
            vm.GenreList = SelectListHelper.GetEnumSelectList<BookGenre>("All Genres", false);

            vm.FormatList = SelectListHelper.GetEnumSelectList<BookFormat>("All Formats");

            vm.StatusList = SelectListHelper.GetEnumSelectList<CompletionStatus>("All Statuses", false);

            vm.ShareToken = user!.BookShareToken;

            //Query Sorting
            query = vm.SortBy switch
            {
                "Author" => vm.IsDesc
                    ? query.OrderByDescending(x => x.SortAuthor).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.SortAuthor).ThenBy(x => x.SortTitle),

                "Title" => vm.IsDesc
                    ? query.OrderByDescending(x => x.SortTitle)
                    : query.OrderBy(x => x.SortTitle),

                "Year" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Year).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Year).ThenBy(x => x.SortTitle),

                "Format" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Format).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Format).ThenBy(x => x.SortTitle),

                "Genre" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Genre.ToString()).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Genre.ToString()).ThenBy(x => x.SortTitle),

                "Status" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Status).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Status).ThenBy(x => x.SortTitle),

                "Rating" => vm.IsDesc
                    ? query.Where(x => x.Rating.HasValue).OrderByDescending(x => x.Rating).ThenBy(x => x.SortTitle)
                    : query.Where(x => x.Rating.HasValue).OrderBy(x => x.Rating).ThenBy(x => x.SortTitle),

                _ => query.OrderBy(x => x.SortTitle)
            };

            //Pagination
            var totalItems = await query.CountAsync();

            var pageSize = vm.IsGridMode ? 36 : 15;

            if (vm.IsViewAllMode) pageSize = totalItems;

            vm.Books = await PaginatedList<Book>.CreateAsync(query.AsNoTracking(), pageNumber, pageSize);

            vm.PageNumber = pageNumber;

            //Redirct to last occupied page
            if (vm.Books.TotalPages > 0 && vm.Books.IsOnEmptyPage)
            {
                return RedirectToAction("Index", new {pageNumber = vm.Books.TotalPages});
            }

            return View(vm);
        }

        public IActionResult Create()
        {
            var vm = new BookFormViewModel
            {
                Status = CompletionStatus.NotStarted
            };

            PopulateBookFormViewModel(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel vm)
        {
            //Model States
            if (vm.Status == CompletionStatus.NotStarted && vm.Rating.HasValue)
            {
                ModelState.AddModelError(nameof(vm.Rating), "Cannot rate a book that you have not read!");
            }
            if (_context.Books.Any(x => x.UserId == _userId && x.Title == vm.Title && x.Format == vm.Format))
            {
                ModelState.AddModelError(string.Empty, $"{vm.Title} ({vm.Format}) already exists in your Library!");
            }
            if (!ModelState.IsValid)
            {
                PopulateBookFormViewModel(vm);
                return View(vm);
            }

            var book = new Book
            {
                Title = vm.Title,
                SortTitle = vm.Title.GetSortableTitle(),
                Author = vm.Author,
                SortAuthor = vm.Author.GetSortableTitle(),
                Genre = vm.Genre!.Value,
                Format = vm.Format!.Value,
                Status = vm.Status!.Value,
                Year = vm.Year,
                Rating = vm.Rating,
                UserId = _userId
            };

            book.ImageUrl = await _imageService.GetBookImageUrlAsync(book.Title, book.Author);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{book.Title} was successfully added";

            if (book.ImageUrl == null)
            {
                TempData["ImageFail"] = $"Unable to get a Cover for {book.Title} at this time";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id, int pageNumber)
        {
            if (id == null) return NotFound();

            var book = await GetUserBookAsync(id.Value);
            if (book == null) return NotFound();

            var vm = new BookFormViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Year = book.Year,
                Format = book.Format,
                Genre = book.Genre,
                Status = book.Status,
                Rating= book.Rating,
                PageNumber = pageNumber
            };

            PopulateBookFormViewModel(vm);

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookFormViewModel vm)
        {
            if (vm.Status == CompletionStatus.NotStarted && vm.Rating.HasValue)
            {
                ModelState.AddModelError(nameof(vm.Rating), "Cannot rate a book that you have not read!");
            }

            if (!ModelState.IsValid)
            {
                PopulateBookFormViewModel(vm);
                return View(vm);
            }

            if (vm.Id == null) return NotFound();

            var book = await GetUserBookAsync(vm.Id.Value);
            if (book == null) return NotFound();
            
            book.Title = vm.Title;
            book.Author = vm.Author;
            book.Year = vm.Year;
            book.Format = vm.Format!.Value;
            book.Genre = vm.Genre!.Value;
            book.Status = vm.Status!.Value;
            book.Rating = vm.Rating;
            book.ImageUrl = await _imageService.GetBookImageUrlAsync(book.Title, book.Author);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"{book.Title} was successfully updated";

            if (book.ImageUrl == null)
            {
                TempData["ImageFail"] = $"Unable to get a Cover for {book.Title} at this time";
            }

            return RedirectToAction("Index", new {pageNumber = vm.PageNumber});
        }

        /*
        //Delete operation is currently being used in a Modal
         
        public async Task<IActionResult> Delete(int? id, int pageNumber)
        {
            if (id == null) return NotFound();

            var book = await GetUserBookAsync(id.Value);
            if (book == null) return NotFound();

            var vm = new BookDeleteViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PageNumber = pageNumber
            };

            return View(vm);
        }
        */

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id, int pageNumber)
        {
            if (id == null) return NotFound();

            var book = await GetUserBookAsync(id.Value);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{book.Title} was successfully deleted";

            return RedirectToAction("Index", new {pageNumber});
        }

        public async Task<IActionResult> Stats()
        {
            var stats = await _statsService.GetStatsAsync(_userId);

            return View(stats);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateImage(int id, string? customImageUrl, int pageNumber)
        {
            var book = await GetUserBookAsync(id);
            if (book == null) return NotFound();

            book.CustomImageUrl = string.IsNullOrWhiteSpace(customImageUrl)
                ? null
                : customImageUrl;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Image Url was successfully saved";

            return RedirectToAction("Index", new {pageNumber});
        }

        private async Task<Book?> GetUserBookAsync(int id) =>
            await _context.Books.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userId);

        private void PopulateBookFormViewModel(BookFormViewModel vm)
        {
            vm.YearList = SelectListHelper.GetYearList(1700, "Select Year");
            vm.GenreList = SelectListHelper.GetEnumSelectList<BookGenre>("Select Genre", false);
            vm.FormatList = SelectListHelper.GetEnumSelectList<BookFormat>("Select Format");
            vm.StatusList = SelectListHelper.GetEnumSelectList<CompletionStatus>("Select Status", false);
        }

        public async Task<IActionResult> SearchBook(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return Json(null);

            var data = await _searchService.BookSearchAsync(title);

            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> AutocompleteBook(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Json(new List<string>());

            var results = await _searchService.BookAutocompleteAsync(title);

            return Ok(results);
        }
    }
}