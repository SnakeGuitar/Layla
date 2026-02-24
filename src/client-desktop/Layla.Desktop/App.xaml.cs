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
        public void ChangeTheme(string theme)
        {
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new ResourceDictionary
            { 
                Source = new Uri($"Themes/{theme}.xaml", UriKind.Relative) 
            });
        }
    }
}
