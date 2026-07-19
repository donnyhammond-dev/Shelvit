namespace BackLog.Helpers
{
    public static class StringExtensions
    {
        public static string GetSortableTitle(this string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            string[] articles = { "The ", "An ", "A " };

            foreach (var article in articles)
            {
                if (title.StartsWith(article, StringComparison.OrdinalIgnoreCase))
                {
                    return title.Substring(article.Length);
                }
            }

            return title;
        }
    }
}
