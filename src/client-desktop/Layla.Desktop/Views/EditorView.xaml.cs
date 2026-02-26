using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Layla.Desktop.Models;
using Layla.Desktop.Services;

namespace Layla.Desktop.Views
{
    public partial class EditorView : Page
    {
        private readonly ProjectDto? _projectDto;
        private readonly IManuscriptApiService _manuscriptApiService;

        public EditorView()
        {
            InitializeComponent();
            _manuscriptApiService = new ManuscriptApiService();
        }

        public EditorView(ProjectDto projectDto)
        {
            InitializeComponent();
            _projectDto = projectDto;
            _manuscriptApiService = new ManuscriptApiService();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_projectDto == null) return;

            try
            {
                var manuscripts = await _manuscriptApiService.GetManuscriptsByProjectIdAsync(_projectDto.Id);
                ManuscriptsListView.ItemsSource = manuscripts;

                if (manuscripts.Any())
                {
                    ManuscriptsListView.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading manuscripts: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManuscriptsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ManuscriptsListView.SelectedItem is ManuscriptDto selectedManuscript)
            {
                EditorRichTextBox.Document.Blocks.Clear();
                EditorRichTextBox.Document.Blocks.Add(new Paragraph(new Run(selectedManuscript.Content)));
            }
            else
            {
                EditorRichTextBox.Document.Blocks.Clear();
                EditorRichTextBox.Document.Blocks.Add(new Paragraph(new Run("Select a manuscript to start writing...")));
            }
        }
    }
}
