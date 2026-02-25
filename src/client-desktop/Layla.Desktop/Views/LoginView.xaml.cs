using Layla.Desktop.Models.Authentication;
using System.Windows.Controls;
using System.Windows;
using Layla.Desktop.Services;

namespace Layla.Desktop.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Page
    {
        private readonly IAuthService _authService;

        public LoginView()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private async void LoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ErrorText.Visibility = System.Windows.Visibility.Collapsed;
            LoginButton.IsEnabled = false;
            LoginButton.Content = "Signing in...";

            var request = new LoginRequest
            {
                Email = EmailTextBox.Text,
                Password = PasswordBox.Password
            };

            var response = await _authService.LoginAsync(request);

            if (response.IsSuccess && response.Response != null)
            {
                SessionManager.CurrentToken = response.Response.Token;
                SessionManager.CurrentEmail = response.Response.Email;
                SessionManager.CurrentDisplayName = response.Response.DisplayName;
                NavigationService.Navigate(new MainView());
            }
            else
            {
                ErrorText.Text = response.ErrorMessage ?? "Login failed.";
                ErrorText.Visibility = Visibility.Visible;
                LoginButton.IsEnabled = true;
                LoginButton.Content = "Login";
            }
        }

        private void NavigateToSignUp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignUpView());
        }
    }
}
