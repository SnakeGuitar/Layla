using Layla.Desktop.Models.Authentication;
using Layla.Desktop.Services;
using System.Windows.Controls;
using System.Linq;

namespace Layla.Desktop.Views
{
    /// <summary>
    /// Interaction logic for SignUpView.xaml
    /// </summary>
    public partial class SignUpView : Page
    {
        private readonly IAuthService _authService;
        public SignUpView()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private async void SignUpButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StatusText.Visibility = System.Windows.Visibility.Collapsed;
            SignUpButton.IsEnabled = false;
            SignUpButton.Content = "Creating account...";

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                string.IsNullOrWhiteSpace(DisplayNameTextBox.Text))
            {
                StatusText.Text = "Please fill in all fields.";
                StatusText.Visibility = System.Windows.Visibility.Visible;
                ResetForm();
                return;
            }

            var request = new RegisterRequest
            {
                Email = EmailTextBox.Text,
                Password = PasswordBox.Password,
                DisplayName = DisplayNameTextBox.Text
            };

            var response = await _authService.RegisterAsync(request);

            if (response.IsSuccess && response.Response != null)
            {
                SessionManager.CurrentToken = response.Response.Token;
                SessionManager.CurrentEmail = response.Response.Email;
                SessionManager.CurrentDisplayName = response.Response.DisplayName;
                NavigationService?.Navigate(new ProjectListView());
            }
            else
            {
                var errorMsg = response.ErrorMessage ?? "Failed to create account. Please try again.";
                if (response.ValidationErrors.Count > 0)
                {
                    errorMsg = string.Join("\n", response.ValidationErrors.SelectMany(v => v.Value));
                }
                ShowMessage(errorMsg, false);
                ResetForm();
            }
        }

        private void NavigateToLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginView());
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            StatusText.Text = message;
            StatusText.Foreground = isSuccess ?
                System.Windows.Media.Brushes.MediumSeaGreen :
                System.Windows.Media.Brushes.IndianRed;
            StatusText.Visibility = System.Windows.Visibility.Visible;
        }

        private void ResetForm()
        {
            SignUpButton.IsEnabled = true;
            SignUpButton.Content = "Sign Up";
        }
    }
}
