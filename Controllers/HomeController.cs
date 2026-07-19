using System.Diagnostics;
using BackLog.Data;
using BackLog.DTOs;
using BackLog.Enums;
using BackLog.Models;
using Microsoft.EntityFrameworkCore;
using BackLog.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackLog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var vm = new HomeViewModel();

            var userGames = user == null ? new List<HomeTileDto>() : await GetUserGameTiles(user.Id);
            vm.GameTiles = userGames.Count >= 3 ? userGames : GetDefaultGameTiles();

            var userAlbums = user == null ? new List<HomeTileDto>() : await GetUserAlbumTiles(user.Id);
            vm.AlbumTiles = userAlbums.Count >= 3 ? userAlbums : GetDefaultAlbumTiles();

            var userBooks = user == null ? new List<HomeTileDto>() : await GetUserBookTiles(user.Id);
            vm.BookTiles = userBooks.Count >= 3 ? userBooks : GetDefaultBookTiles();

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<HomeTileDto> GetDefaultGameTiles()
        {
            return new List<HomeTileDto>
            {
                  new HomeTileDto("Forza 6", "/images/default/games/Game0.jpg", EntryType.Game),
                  new HomeTileDto("Dark Souls 3", "/images/default/games/Game1.jpg", EntryType.Game),
                  new HomeTileDto("Super Mario Galaxy", "/images/default/games/Game2.jpg", EntryType.Game),
                  new HomeTileDto("Metroid Prime", "/images/default/games/Game3.jpg", EntryType.Game),
                  new HomeTileDto("Hollow Knight Silksong", "/images/default/games/Game4.jpg", EntryType.Game),
                  new HomeTileDto("Final Fantasy X", "/images/default/games/Game5.jpg", EntryType.Game),
                  new HomeTileDto("GTA V", "/images/default/games/Game6.jpg", EntryType.Game),
                  new HomeTileDto("Resident Evil Requiem", "/images/default/games/Game7.jpg", EntryType.Game),
                  new HomeTileDto("The Legend of Zelda Twilight Princess", "/images/default/games/Game8.jpg", EntryType.Game),
                  new HomeTileDto("Halo 3", "/images/default/games/Game9.jpg", EntryType.Game),
                  new HomeTileDto("Hades II", "/images/default/games/Game10.jpg", EntryType.Game),
                  new HomeTileDto("Celeste", "/images/default/games/Game11.jpg", EntryType.Game),
                  new HomeTileDto("Starfox", "/images/default/games/Game12.jpg", EntryType.Game),
                  new HomeTileDto("Castlevania Symphony of the Night", "/images/default/games/Game13.jpg", EntryType.Game),
            };
        }

        private List<HomeTileDto> GetDefaultAlbumTiles()
        {
            return new List<HomeTileDto>
            {
                 new HomeTileDto("Bitches Brew", "/images/default/albums/Album0.jpg", EntryType.Album),
                 new HomeTileDto("OK Computer", "/images/default/albums/Album1.jpg", EntryType.Album),
                 new HomeTileDto("Dark Side of the Moon", "/images/default/albums/Album2.jpg", EntryType.Album),
                 new HomeTileDto("To Pimp a Butterfly", "/images/default/albums/Album3.jpg", EntryType.Album),
                 new HomeTileDto("Demon Days", "/images/default/albums/Album4.jpg", EntryType.Album),
                 new HomeTileDto("Rumours", "/images/default/albums/Album5.jpg", EntryType.Album),
                 new HomeTileDto("Toxicity", "/images/default/albums/Album6.jpg", EntryType.Album),
                 new HomeTileDto("Currents", "/images/default/albums/Album7.jpg", EntryType.Album),
                 new HomeTileDto("The Velvet Underground & Nico", "/images/default/albums/Album8.png", EntryType.Album),
                 new HomeTileDto("Man on the Moon", "/images/default/albums/Album9.jpg", EntryType.Album),
                 new HomeTileDto("Energy", "/images/default/albums/Album10.jpg", EntryType.Album),
                 new HomeTileDto("Madvillainy", "/images/default/albums/Album11.jpg", EntryType.Album),
                 new HomeTileDto("Harvest", "/images/default/albums/Album12.jpg", EntryType.Album),
                 new HomeTileDto("Merriweather Post Pavilion", "/images/default/albums/Album13.jpg", EntryType.Album)

            };
        }

        private List<HomeTileDto> GetDefaultBookTiles()
        {
            return new List<HomeTileDto>
            {
                 new HomeTileDto("Bride", "/images/default/books/Book0.jpg", EntryType.Book),
                 new HomeTileDto("The Stand", "/images/default/books/Book1.jpg", EntryType.Book),
                 new HomeTileDto("Fourth Wing", "/images/default/books/Book2.jpg", EntryType.Book),
                 new HomeTileDto("Project Hail Mary", "/images/default/books/Book3.jpg", EntryType.Book),
                 new HomeTileDto("Theo of Golden", "/images/default/books/Book4.jpg", EntryType.Book),
                 new HomeTileDto("The Hobbit", "/images/default/books/Book5.jpg", EntryType.Book),
                 new HomeTileDto("East of Eden", "/images/default/books/Book6.jpg", EntryType.Book),
                 new HomeTileDto("A Court of Thorns and Roses", "/images/default/books/Book7.jpg", EntryType.Book),
                 new HomeTileDto("Fighting Words", "/images/default/books/Book9.jpg", EntryType.Book),
                 new HomeTileDto("The Invisible Life of Addie LaRue", "/images/default/books/Book10.jpg", EntryType.Book),
                 new HomeTileDto("The Hunger Games", "/images/default/books/Book11.jpg", EntryType.Book),
                 new HomeTileDto("Beta Vulgaris", "/images/default/books/Book12.jpg", EntryType.Book),
                 new HomeTileDto("The Devil in the White City", "/images/default/books/Book13.jpg", EntryType.Book)
            };
        }

        private async Task<List<HomeTileDto>> GetUserGameTiles(string userId)
        {
            return await _context.Games.Where(x => x.UserId == userId && (x.ImageUrl != null || x.CustomImageUrl != null))
                .OrderByDescending(x => x.Rating.HasValue)
                .ThenByDescending(x => x.Rating)
                .Take(20)
                .Select(x => new HomeTileDto(
                    x.Title,
                    x.CustomImageUrl ?? x.ImageUrl!,
                    EntryType.Game))
                .ToListAsync();
        }

        private async Task<List<HomeTileDto>> GetUserAlbumTiles(string userId)
        {
            return await _context.Albums.Where(x => x.UserId == userId && (x.ImageUrl != null || x.CustomImageUrl != null))
                .OrderByDescending(x => x.Rating.HasValue)
                .ThenByDescending(x => x.Rating)
                .Take(20)
                .Select(x => new HomeTileDto(
                    x.Title,
                    x.CustomImageUrl ?? x.ImageUrl!,
                    EntryType.Album))
                .ToListAsync();
        }

        private async Task<List<HomeTileDto>> GetUserBookTiles(string userId)
        {
            return await _context.Books.Where(x => x.UserId == userId && (x.ImageUrl != null || x.CustomImageUrl != null))
                .OrderByDescending(x => x.Rating.HasValue)
                .ThenByDescending(x => x.Rating)
                .Take(20)
                .Select(x => new HomeTileDto(
                    x.Title,
                    x.CustomImageUrl ?? x.ImageUrl!,
                    EntryType.Book))
                .ToListAsync();
        }
    }
}
