namespace BackLog.Helpers
{
    public class ShareTokenHelper
    {
        public static string Generate() => Guid.NewGuid().ToString("N");
    }
}
