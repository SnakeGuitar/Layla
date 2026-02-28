using System.Windows;
using System.Windows.Controls;

namespace Layla.Desktop.Views
{
    public partial class WikiEntityEditorView : Page
    {
        public WikiEntityEditorView()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string aliases = AliasesTextBox.Text;
            string description = DescriptionTextBox.Text;
            string tags = TagsTextBox.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("The Name field is required to save an entity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Wiki Entity '{name}' saved successfully (Local Mock)!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
            NameTextBox.Clear();
            AliasesTextBox.Clear();
            DescriptionTextBox.Clear();
            TagsTextBox.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Clear();
            AliasesTextBox.Clear();
            DescriptionTextBox.Clear();
            TagsTextBox.Clear();
        }
    }
}
