using System;
using System.IO;

namespace Layla.Desktop.Services
{
    public static class SessionManager
    {
        private static readonly string SessionPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Layla",
            "session.json"
        );

        public static string CurrentToken { get; set; } = string.Empty;
        public static string CurrentEmail { get; set; } = string.Empty;
        public static string CurrentDisplayName { get; set; } = string.Empty;

        public static bool IsAuthenticated => !string.IsNullOrEmpty(CurrentToken);

        public static void SaveSession(string token, string email, string name)
        {
            CurrentToken = token;
            CurrentEmail = email;
            CurrentDisplayName = name;

            try
            {
                var directory = Path.GetDirectoryName(SessionPath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory!);

                var json = System.Text.Json.JsonSerializer.Serialize(new { CurrentToken, CurrentEmail, CurrentDisplayName });
                File.WriteAllText(SessionPath, json);
            }
            catch { /* Redundancy/Logging */ }
        }

        public static void LoadSession()
        {
            if (!File.Exists(SessionPath)) return;

            try
            {
                var json = File.ReadAllText(SessionPath);
                var session = System.Text.Json.JsonSerializer.Deserialize<SessionData>(json);
                if (session != null)
                {
                    CurrentToken = session.CurrentToken;
                    CurrentEmail = session.CurrentEmail;
                    CurrentDisplayName = session.CurrentDisplayName;
                }
            }
            catch { /* Clear corrupted session */ ClearSession(); }
        }

        public static void ClearSession()
        {
            CurrentToken = string.Empty;
            CurrentEmail = string.Empty;
            CurrentDisplayName = string.Empty;
            if (File.Exists(SessionPath)) File.Delete(SessionPath);
        }

        private class SessionData
        {
            public string CurrentToken { get; set; } = string.Empty;
            public string CurrentEmail { get; set; } = string.Empty;
            public string CurrentDisplayName { get; set; } = string.Empty;
        }
    }
}