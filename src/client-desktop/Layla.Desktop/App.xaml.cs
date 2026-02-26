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

            this.Dispatcher.InvokeAsync(() =>
            {
                if (this.MainWindow != null)
                {
                    this.MainWindow.KeyDown += MainWindow_KeyDown;
                }
            });
        }

        private bool _isFullscreen = false;
        private WindowStyle _previousWindowStyle = WindowStyle.SingleBorderWindow;
        private WindowState _previousWindowState = WindowState.Normal;

        public bool IsFullscreen => _isFullscreen;

        public void SetFullscreen(bool isFullscreen)
        {
            if (this.MainWindow == null) return;
            if (_isFullscreen == isFullscreen) return;

            if (isFullscreen)
            {
                _previousWindowStyle = this.MainWindow.WindowStyle;
                _previousWindowState = this.MainWindow.WindowState;

                this.MainWindow.WindowStyle = WindowStyle.None;
                this.MainWindow.WindowState = WindowState.Maximized;
                _isFullscreen = true;
            }
            else
            {
                this.MainWindow.WindowStyle = _previousWindowStyle;
                this.MainWindow.WindowState = _previousWindowState;
                _isFullscreen = false;
            }
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.MainWindow == null) return;

            if (e.Key == System.Windows.Input.Key.F11)
            {
                SetFullscreen(!_isFullscreen);
            }
            else if (e.Key == System.Windows.Input.Key.Escape && _isFullscreen)
            {
                SetFullscreen(false);
            }
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
