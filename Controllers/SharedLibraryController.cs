using BackLog.Data;
using BackLog.Models;
using BackLog.ViewModels.SharedViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace BackLog.Controllers
{
    [AllowAnonymous]
    public class SharedLibraryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SharedLibraryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GamesTable(string id, SharedGamesViewModel vm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.GameShareToken == id);

            if (user == null) return NotFound();

            var query = _context.Games.Where(x => x.UserId == user.Id);

            query = vm.SortBy switch
            {
                "Title" => vm.IsDesc
                    ? query.OrderByDescending(x => x.SortTitle)
                    : query.OrderBy(x => x.SortTitle),

                "Year" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Year).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Year).ThenBy(x => x.SortTitle),

                "Platform" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Platform.ToString()).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Platform.ToString()).ThenBy(x => x.SortTitle),

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

            vm.UserName = user.UserName!;
            vm.Games = await query.ToListAsync();
            vm.ShareToken = user.GameShareToken!;

            return View(vm);
        }

        public async Task<IActionResult> GamesGrid(string id, SharedGamesViewModel vm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.GameShareToken == id);

            if (user == null) return NotFound();

            vm.UserName = user.UserName!;
            vm.Games = await _context.Games.Where(x => x.UserId == user.Id)
                .OrderBy(x => x.SortTitle)
                .ToListAsync();
            vm.ShareToken = user.GameShareToken!;

            return View(vm);
        }

        public async Task<IActionResult> AlbumsTable(string id, SharedAlbumsViewModel vm)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.AlbumShareToken == id);

            if (user == null)
                return NotFound();

            var query = _context.Albums
                .Where(x => x.UserId == user.Id);

            query = vm.SortBy switch
            {
                "Artist" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Artist).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Artist)
                        .ThenBy(x => x.Title),

                "Title" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Title)
                    : query.OrderBy(x => x.Title),

                "Year" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Year).ThenBy(x => x.Artist).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Year).ThenBy(x => x.Artist).ThenBy(x => x.Title),

                "Format" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Format).ThenBy(x => x.Artist).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Format).ThenBy(x => x.Artist).ThenBy(x => x.Title),

                "Genre" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Genre).ThenBy(x => x.Artist).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Genre).ThenBy(x => x.Artist).ThenBy(x => x.Title),

                "Status" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Status).ThenBy(x => x.Artist).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Status).ThenBy(x => x.Artist).ThenBy(x => x.Title),

                "Rating" => vm.IsDesc
                    ? query.Where(x => x.Rating.HasValue).OrderByDescending(x => x.Rating).ThenBy(x => x.Artist).ThenBy(x => x.Title)
                    : query.Where(x => x.Rating.HasValue).OrderBy(x => x.Rating).ThenBy(x => x.Artist).ThenBy(x => x.Title),

                _ => query.OrderBy(x => x.Artist)
                    .ThenBy(x => x.Title)
            };

            vm.UserName = user.UserName!;
            vm.Albums = await query.ToListAsync();
            vm.ShareToken = user.AlbumShareToken!;

            return View(vm);
        }

        public async Task<IActionResult> AlbumsGrid(string id, SharedAlbumsViewModel vm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.AlbumShareToken == id);

            if (user == null) return NotFound();

            vm.UserName = user.UserName!;
            vm.Albums = await _context.Albums.Where(x => x.UserId == user.Id)
                .OrderBy(x => x.SortArtist)
                .ThenBy(x => x.SortTitle)
                .ToListAsync();
            vm.ShareToken = user.AlbumShareToken!;

            return View(vm);
        }

        public async Task<IActionResult> BooksTable(string id, SharedBooksViewModel vm)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.BookShareToken == id);

            if (user == null)
                return NotFound();

            var query = _context.Books
                .Where(x => x.UserId == user.Id);

            query = vm.SortBy switch
            {
                "Author" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Author).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Author).ThenBy(x => x.Title),

                "Title" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Title)
                    : query.OrderBy(x => x.Title),

                "Year" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Year).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Year).ThenBy(x => x.Title),

                "Format" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Format).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Format).ThenBy(x => x.Title),

                "Genre" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Genre).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Genre).ThenBy(x => x.Title),

                "Status" => vm.IsDesc
                    ? query.OrderByDescending(x => x.Status).ThenBy(x => x.Title)
                    : query.OrderBy(x => x.Status).ThenBy(x => x.Title),

                "Rating" => vm.IsDesc
                    ? query.Where(x => x.Rating.HasValue).OrderByDescending(x => x.Rating).ThenBy(x => x.Title)
                    : query.Where(x => x.Rating.HasValue).OrderBy(x => x.Rating).ThenBy(x => x.Title),

                _ => query.OrderBy(x => x.Title)
            };

            vm.UserName = user.UserName!;
            vm.Books = await query.ToListAsync();
            vm.ShareToken = user.BookShareToken!;

            return View(vm);
        }

        public async Task<IActionResult> BooksGrid(string id, SharedBooksViewModel vm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.BookShareToken == id);

            if (user == null) return NotFound();

            vm.UserName = user.UserName!;
            vm.Books = await _context.Books.Where(x => x.UserId == user.Id)
                .OrderBy(x => x.SortAuthor)
                .ThenBy(x => x.SortTitle)
                .ToListAsync();
            vm.ShareToken = user.BookShareToken!;

            return View(vm);
        }
    }
}