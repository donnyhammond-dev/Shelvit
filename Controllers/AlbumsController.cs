using BackLog.Data;
using BackLog.Enums;
using BackLog.Helpers;
using BackLog.Models;
using BackLog.Services.Interfaces.Albums;
using BackLog.ViewModels.AlbumViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackLog.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAlbumImageService _imageService;
        private readonly IAlbumStatsService _statsService;
        private readonly IAlbumSearchService _searchService;
        private string _userId => _userManager.GetUserId(User)!;

        public AlbumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IAlbumImageService imageService, IAlbumStatsService statsService, IAlbumSearchService searchService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _statsService = statsService;
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(AlbumIndexViewModel vm, int pageNumber = 1)
        {
            //Generate Library Sharing Token
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(user!.AlbumShareToken))
            {
                user!.AlbumShareToken = ShareTokenHelper.Generate();
                await _userManager.UpdateAsync(user);
            }

            //Base Query
            var query = _context.Albums.Where(x => x.UserId == _userId);

            //Empty State Check
            vm.HasAnyAlbums = await query.AnyAsync();

            //Build Library
            if (!string.IsNullOrEmpty(vm.Search))
            {
                query = query.Where(x => x.Title.Contains(vm.Search) || x.Artist.Contains(vm.Search));
            }

            if (vm.Genre.HasValue)
                query = query.Where(x => x.Genre == vm.Genre.Value);

            if (vm.Format.HasValue)
                query = query.Where(x => x.Format == vm.Format.Value);

            if (vm.Status.HasValue)
                query = query.Where(x => x.Status == vm.Status.Value);

            vm.GenreList = await SelectListHelper.GetDistinctEnumSelectListAsync<Album, MusicGenre>(
                query, x => (MusicGenre)x.Genre!, "All Genres");

            vm.FormatList = SelectListHelper.GetEnumSelectList<Format>("All Formats");

            vm.StatusList = SelectListHelper.GetEnumSelectList<CompletionStatus>("All Statuses", false);

            vm.ShareToken = user!.AlbumShareToken;

            query = vm.SortBy switch
            {
                "Artist" => vm.IsDesc ? query.OrderByDescending(x => x.SortArtist).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.SortArtist).ThenBy(x => x.SortTitle),

                "Title" => vm.IsDesc ? query.OrderByDescending(x => x.SortTitle)
                    : query.OrderBy(x => x.SortTitle),

                "Year" => vm.IsDesc ? query.OrderByDescending(x => x.Year).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Year).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle),

                "Format" => vm.IsDesc ? query.OrderByDescending(x => x.Format).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Format).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle),

                "Genre" => vm.IsDesc ? query.OrderByDescending(x => x.Genre.ToString()).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Genre.ToString()).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle),

                "Status" => vm.IsDesc ? query.OrderByDescending(x => x.Status).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle)
                    : query.OrderBy(x => x.Status).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle),

                "Rating" => vm.IsDesc ? query.Where(x => x.Rating.HasValue).OrderByDescending(x => x.Rating).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle)
                    : query.Where(x => x.Rating.HasValue).OrderBy(x => x.Rating).ThenBy(x => x.SortArtist).ThenBy(x => x.SortTitle),

                _ => query.OrderBy(x => x.SortArtist).ThenBy(x => x.SortTitle)
            };

            //Pagintation
            var totalItems = await query.CountAsync();

            var pageSize = vm.IsGridMode ? 36 : 15;

            if (vm.IsViewAllMode) pageSize = totalItems;

            vm.Albums = await PaginatedList<Album>.CreateAsync(query.AsNoTracking(), pageNumber, pageSize);

            vm.PageNumber = pageNumber;

            //Redirct to last occupied page
            if (vm.Albums.TotalPages > 0 && vm.Albums.IsOnEmptyPage)
            {
                return RedirectToAction("Index", new { pageNumber = vm.Albums.TotalPages });
            }

            return View(vm);
        }

        public IActionResult Create()
        {
            var vm = new AlbumFormViewModel
            {
                Status = CompletionStatus.NotStarted
            };
            PopulateAlbumFormViewModel(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlbumFormViewModel vm)
        {
            if (vm.Status == CompletionStatus.NotStarted && vm.Rating.HasValue)
            {
                ModelState.AddModelError(nameof(vm.Rating), "Cannot rate an album that you have not listened too!");
            }
            if (_context.Albums.Any(x => x.UserId == _userId && x.Title == vm.Title && x.Format == vm.Format))
            {
                ModelState.AddModelError(string.Empty, $"{vm.Title} ({vm.Format}) already exists in your Library!");
            }
            if (!ModelState.IsValid)
            {
                PopulateAlbumFormViewModel(vm);
                return View(vm);
            }

            var album = new Album
            {
                Title = vm.Title,
                SortTitle = vm.Title.GetSortableTitle(),
                Artist = vm.Artist,
                SortArtist = vm.Artist.GetSortableTitle(),
                Genre = vm.Genre!.Value,
                Format = vm.Format!.Value,
                Status = vm.Status!.Value,
                Year = vm.Year,
                Rating = vm.Rating,
                UserId = _userId
            };

            album.ImageUrl = await _imageService.GetAlbumImageUrlAsync(album.Artist, album.Title);

            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{album.Title} by {album.Artist} was successfully added";

            if (album.ImageUrl == null)
            {
                TempData["ImageFail"] = "Unable to get a Album Artwork at this time";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id, int pageNumber)
        {
            if (id == null) return NotFound();

            var album = await GetUserAlbumAsync(id.Value);

            if (album == null) return NotFound();

            var vm = new AlbumFormViewModel
            {
                Id = album.Id,
                Title = album.Title,
                Artist = album.Artist,
                Year = album.Year,
                Format = album.Format,
                Genre = album.Genre,
                Status = album.Status,
                Rating = album.Rating,
                PageNumber = pageNumber
            };
            PopulateAlbumFormViewModel(vm);
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AlbumFormViewModel vm)
        {
            if (vm.Status == CompletionStatus.NotStarted && vm.Rating.HasValue)
            {
                ModelState.AddModelError(nameof(vm.Rating), "Cannot rate an album that you have not listened too!");
            }

            if (!ModelState.IsValid)
            {
                PopulateAlbumFormViewModel(vm);
                return View(vm);
            }

            if (vm.Id == null) return NotFound();

            var album = await GetUserAlbumAsync(vm.Id.Value);
            if (album == null) return NotFound();

            album.Title = vm.Title;
            album.Artist = vm.Artist;
            album.Year = vm.Year;
            album.Format = vm.Format!.Value;
            album.Genre = vm.Genre!.Value;
            album.Status = vm.Status!.Value;
            album.Rating = vm.Rating;
            album.ImageUrl = await _imageService.GetAlbumImageUrlAsync(album.Artist, album.Title);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"{album.Title} was successfully updated";

            if (album.ImageUrl == null)
            {
                TempData["ImageFail"] = $"Unable to get Album Artwork at this time";
            }

            return RedirectToAction("Index", new { pageNumber = vm.PageNumber });
        }

        /*
        //Delete operation is currently being used in a Modal
        public async Task<IActionResult> Delete(int? id, int pageNumber)
        {
            if (id == null) return NotFound();

            var album = await GetUserAlbumAsync(id.Value);
            if (album == null) return NotFound();

            var vm = new AlbumDeleteViewModel
            {
                Id = album.Id,
                Title = album.Title,
                Artist = album.Artist,
                Format = album.Format,
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

            var album = await GetUserAlbumAsync(id.Value);

            if (album == null) return NotFound();

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{album.Title} by {album.Artist} successfully deleted";

            return RedirectToAction("Index", new { pageNumber });
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
            var album = await GetUserAlbumAsync(id);
            if (album == null) return NotFound();

            album.CustomImageUrl = string.IsNullOrWhiteSpace(customImageUrl)
                ? null
                : customImageUrl;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Image Url was successfully saved";

            return RedirectToAction("Index", new {pageNumber});
        }

        private async Task<Album?> GetUserAlbumAsync(int id)
        {
            return await _context.Albums.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userId);
        }
        private void PopulateAlbumFormViewModel(AlbumFormViewModel vm)
        {
            vm.YearList = SelectListHelper.GetYearList(1900, "Select Year");
            vm.FormatList = SelectListHelper.GetEnumSelectList<Format>("Select Format");
            vm.GenreList = SelectListHelper.GetEnumSelectList<MusicGenre>("Select Genre");
            vm.StatusList = SelectListHelper.GetEnumSelectList<CompletionStatus>("Select Status", false);
        }

        [HttpGet]
        public async Task<IActionResult> SearchAlbum(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return Json(null);

            var result = await _searchService.AlbumSearchAsync(title);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> AutocompleteAlbum(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Json(new List<string>());

            var results = await _searchService.AlbumAutocompleteAsync(title);

            return Ok(results);
        }
    }
}