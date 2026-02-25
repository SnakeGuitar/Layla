namespace Layla.Desktop.Services
{
    public static class SessionManager
    {
        public static string CurrentToken { get; set; } = string.Empty;
        public static string CurrentEmail { get; set; } = string.Empty;
        public static string CurrentDisplayName { get; set; } = string.Empty;

        public static bool IsAuthenticated => !string.IsNullOrEmpty(CurrentToken);

        public static void ClearSession()
        {
            CurrentToken = string.Empty;
            CurrentEmail = string.Empty;
            CurrentDisplayName = string.Empty;
        }
    }
}