using BackLog.Data;
using BackLog.Helpers;
using BackLog.Models;
using BackLog.Services.AlbumServices;
using BackLog.Services.BookServices;
using BackLog.Services.GameServices;
using BackLog.Services.Interfaces.Albums;
using BackLog.Services.Interfaces.Books;
using BackLog.Services.Interfaces.Games;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpClient<IAlbumImageService, AlbumImageService>(x => x.Timeout = TimeSpan.FromSeconds(8));
builder.Services.AddHttpClient<IGameImageService, GameImageService>(x => x.Timeout = TimeSpan.FromSeconds(8));
builder.Services.AddHttpClient<IBookImageService, BookImageService>(x => x.Timeout = TimeSpan.FromSeconds(8));
builder.Services.AddHttpClient<IGameSearchService, GameSearchService>(x => x.Timeout = TimeSpan.FromSeconds(8));
builder.Services.AddHttpClient<IAlbumSearchService, AlbumSearchService>(x => x.Timeout = TimeSpan.FromSeconds(8));
builder.Services.AddHttpClient<IBookSearchService, BookSearchService>(x => x.Timeout = TimeSpan.FromSeconds(8));
builder.Services.AddScoped<IGameStatsService, GameStatsService>();
builder.Services.AddScoped<IAlbumStatsService, AlbumStatsService>();
builder.Services.AddScoped<IBookStatsService, BookStatsService>();
builder.Services.AddHttpClient<SpotifyAuthService>();
builder.Services.AddScoped<IgdbAuthService, IgdbAuthService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Seed(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
