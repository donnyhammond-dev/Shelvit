using BackLog.Data;
using BackLog.Helpers;
using BackLog.Models;
using BackLog.Services.Interfaces.Games;
using BackLog.ViewModels.GameViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BackLog.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGameImageService _imageService;
        private readonly IGameSearchService _searchService;
        private readonly IGameStatsService _statsService;
        private string userId => _userManager.GetUserId(User)!;

        public LibraryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            IGameImageService imageService, IGameStatsService statsService, IGameSearchService searchService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _statsService = statsService;
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(LibraryViewModel vm, int pageNumber = 1)
        {
            pageNumber = Math.Max(pageNumber, 1);

            //Generate Library Sharing Token
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(user!.GameShareToken))
            {
                user!.GameShareToken = ShareTokenHelper.Generate();
                await _userManager.UpdateAsync(user);
            }

            //Base Query
            var query = _context.Games.Where(x => x.UserId == userId);

            //Empty state check
            vm.HasAnyGames = await query.AnyAsync();

            //Build Library
            if (!string.IsNullOrEmpty(vm.Search))
            {
                query = query.Where(x => x.Title.Contains(vm.Search));
            }

            //Possible null issues!
            vm.PlatformList = await SelectListHelper.GetDistinctEnumSelectListAsync<Game, Platform>(
                query, x => (Platform)x.Platform!, "All Platforms");

            vm.GenreList = await SelectListHelper.GetDistinctEnumSelectListAsync<Game, Genre>(
                query, x => (Genre)x.Genre!, "All Genres");

            vm.StatusList = SelectListHelper.GetEnumSelectList<Status>("All Statuses", false);       

            vm.StatusGroupList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "My Games"},
                new SelectListItem { Value = "BackLog", Text = "BackLog"},
                new SelectListItem { Value = "CompletionList", Text = "Completion List"},
            };

            vm.ShareToken = user!.GameShareToken;

            if (!string.IsNullOrEmpty(vm.StatusGroup))
            {
                switch (vm.StatusGroup)
                {
                    case "BackLog":
                        query = query.Where(x => x.Status == Status.Started || x.Status == Status.NotStarted);
                        break;
                    case "CompletionList":
                        query = query.Where(x => x.Status == Status.Completed || x.Status == Status.FullyCompleted);
                        break;
                    case "":
                    default:
                        break;
                }
            }

            if (vm.Platform.HasValue)
            {
                query = query.Where(x => x.Platform == vm.Platform.Value);
            }

            if (vm.Genre.HasValue)
            {
                query = query.Where(x => x.Genre == vm.Genre.Value);
            }

            if (vm.Status.HasValue)
            {
                query = query.Where(x => x.Status == vm.Status.Value);
            }

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


            //Pagination
            var totalItems = await query.CountAsync();

            var pageSize = vm.IsGridMode ? 36 : 15;

            if (vm.IsViewAllMode) pageSize = totalItems;

            vm.Games = await PaginatedList<Game>.CreateAsync(query.AsNoTracking(), pageNumber, pageSize);

            vm.PageNumber = pageNumber;

            //Redirct to last occupied page
            if (vm.Games.TotalPages > 0 && vm.Games.IsOnEmptyPage)
            {
                return RedirectToAction("Index", new { pageNumber = vm.Games.TotalPages });
            }

            return View(vm);
        }

        public IActionResult Create()
        {
            var vm = new GameFormViewModel();
            PopulateGameFormViewModel(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameFormViewModel vm)
        {
            //Model Error States
            if (vm.Status == Status.NotStarted && vm.Rating.HasValue)
            {
                ModelState.AddModelError("Rating", "Cannot rate a game that you have not played!");
            }
            if (_context.Games.Any(x => x.UserId == userId && x.Title == vm.Title && x.Platform == vm.Platform))
            {
                ModelState.AddModelError(string.Empty, $"{vm.Title} ({vm.Platform.ToString()}) already exists in your Library!");
            }
            if (!ModelState.IsValid)
            {
                PopulateGameFormViewModel(vm);
                return View(vm);
            }

            var game = new Game
            {
                Title = vm.Title,
                SortTitle = vm.Title.GetSortableTitle(),
                Year = vm.Year,
                Platform = vm.Platform ?? 0,
                Genre = vm.Genre ?? 0,
                Status = vm.Status ?? 0,
                Rating = vm.Rating,
                UserId = userId
            };


            var imageUrl = await _imageService.GetGameImageUrlAsync(game.Title);
            game.ImageUrl = imageUrl;

            _context.Games.Add(game);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"{game.Title} added successfully!";

            if (game.ImageUrl == null)
            {
                TempData["ImageFail"] = $"Unable to get a Cover for {game.Title} at this time";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id, int pageNumber)
        {
            if (id == null) return NotFound();

            var game = await GetUserGameAsync(id.Value);

            if (game == null) return NotFound();

            var vm = new GameFormViewModel
            {
                Id = game.Id,
                Title = game.Title,
                Year = game.Year,
                Platform = game.Platform,
                Genre = game.Genre,
                Status = game.Status,
                Rating = game.Rating,
                PageNumber = pageNumber
            };
            PopulateGameFormViewModel(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GameFormViewModel vm)
        {
            //Model Error States
            if (vm.Status == Status.NotStarted && vm.Rating.HasValue)
            {
                ModelState.AddModelError(nameof(vm.Rating), "Cannot rate a game that you have not played!");
            }

            if (!ModelState.IsValid)
            {
                PopulateGameFormViewModel(vm);
                return View(vm);
            }

            if (vm.Id == null) return NotFound();

            var game = await GetUserGameAsync(vm.Id.Value);

            if (game == null) return NotFound();

            game.Title = vm.Title;
            game.Year = vm.Year;
            game.Platform = vm.Platform ?? 0;
            game.Genre = vm.Genre ?? 0;
            game.Status = vm.Status ?? 0;
            game.Rating = vm.Rating;

            var imageUrl = await _imageService.GetGameImageUrlAsync(game.Title);
            game.ImageUrl = imageUrl;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"{game.Title} updated successfully!";

            if (game.ImageUrl == null)
            {
                TempData["ImageFail"] = $"Unable to get a Cover for {game.Title} at this time";
            }

            return RedirectToAction("Index", new { pageNumber = vm.PageNumber});
        }

        /*
        //Delete operation is currently being used in a Modal
        public async Task<IActionResult> Delete(int? id, int pageNumber)
        {
            if (id == null) return NotFound();

            var game = await GetUserGameAsync(id.Value);

            if (game == null) return NotFound();

            var vm = new GameDeleteViewModel
            {
                Id = game.Id,
                Title = game.Title,
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
            var game = await GetUserGameAsync(id.Value);
            if (game == null) return NotFound();
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{game.Title} deleted successfully";

            return RedirectToAction("Index", new {pageNumber});
        }

        public async Task<IActionResult> Stats()
        {
            var stats = await _statsService.GetStatsAsync(userId);

            return View(stats);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateImage(int id, string? customImageUrl, int pageNumber)
        {
            var game = await GetUserGameAsync(id);
            if (game == null) return NotFound();

            game.CustomImageUrl = string.IsNullOrWhiteSpace(customImageUrl)
                ? null
                : customImageUrl;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Image Url was successfully saved";

            return RedirectToAction("Index", new {pageNumber});
        }

        private async Task<Game?> GetUserGameAsync(int id)
        {
            return await _context.Games.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        private void PopulateGameFormViewModel(GameFormViewModel vm)
        {
            vm.YearList = SelectListHelper.GetYearList(1980, "Select Year");
            vm.PlatformList = SelectListHelper.GetEnumSelectList<Platform>("Select Platform");
            vm.GenreList = SelectListHelper.GetEnumSelectList<Genre>("Select Genre");
            vm.StatusList = SelectListHelper.GetEnumSelectList<Status>("Select Status", false);
        }

        [HttpGet]
        public async Task<IActionResult> SearchGame(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return Json(null);

            var result = await _searchService.GameSearchAsync(title);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> AutocompleteGame(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Json(new List<string>());

            var result = await _searchService.GameAutocompleteAsync(title);

            return Ok(result);
        }

    }
}