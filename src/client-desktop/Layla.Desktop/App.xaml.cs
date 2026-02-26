using System.Configuration;
using System.Data;
using System.Windows;

namespace Layla.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string ConfigPath => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Layla", "theme.txt");
        public string CurrentTheme { get; private set; } = "LightTheme";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string theme = "LightTheme";
            try 
            {
                if (System.IO.File.Exists(ConfigPath))
                    theme = System.IO.File.ReadAllText(ConfigPath).Trim();
            } 
            catch {}
            ChangeTheme(theme);
        }

        public void ChangeTheme(string theme)
        {
            CurrentTheme = theme;
            try 
            {
                var dir = System.IO.Path.GetDirectoryName(ConfigPath);
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir!);
                System.IO.File.WriteAllText(ConfigPath, theme);
            } 
            catch {}

            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new ResourceDictionary
            { 
                Source = new Uri($"Themes/{theme}.xaml", UriKind.Relative) 
            });
        }
    }
}
