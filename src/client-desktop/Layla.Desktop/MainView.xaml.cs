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
            while (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }
    }
}
