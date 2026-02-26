using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Layla.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainView : Page
    {
        public MainView()
        {
            InitializeComponent();
            this.Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            try 
            {
                while (NavigationService != null && NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            } 
            catch { }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SettingsView());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Services.SessionManager.ClearSession();
            NavigationService.Navigate(new LoginView());
        }
    }
}
