using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Layla.Desktop.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Page
    {
        public SettingsView()
        {
            InitializeComponent();
            InitializeThemeSelection();
        }

        private void InitializeThemeSelection()
        {
            if (Application.Current is App app)
            {
                var theme = app.CurrentTheme;
                for (int i = 0; i < ThemeComboBox.Items.Count; i++)
                {
                    if (ThemeComboBox.Items[i] is ComboBoxItem item && item.Tag?.ToString() == theme)
                    {
                        ThemeComboBox.SelectedIndex = i;
                        return;
                    }
                }
            }
            ThemeComboBox.SelectedIndex = 0; 
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var themeName = selectedItem.Tag.ToString();
                if (!string.IsNullOrEmpty(themeName))
                {
                    (Application.Current as App)?.ChangeTheme(themeName);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
