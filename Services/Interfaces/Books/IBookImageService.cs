namespace BackLog.Services.Interfaces.Books
{
    public interface IBookImageService 
    {
        Task<string?> GetBookImageUrlAsync(string title, string author);
    }
}
