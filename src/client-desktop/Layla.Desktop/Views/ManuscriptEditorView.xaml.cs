using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Layla.Desktop.Services;

namespace Layla.Desktop.Views
{
    public partial class ManuscriptEditorView : Page
    {
        private readonly LocalCacheManager _cacheManager;
        private readonly string _currentManuscriptId = "temp-manuscript-id";
        private bool _isLoaded = false;

        public ManuscriptEditorView()
        {
            InitializeComponent();
            _cacheManager = new LocalCacheManager();
            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string cachedContent = await _cacheManager.LoadManuscriptAsync(_currentManuscriptId);
                if (!string.IsNullOrEmpty(cachedContent))
                {
                    EditorRichTextBox.Document.Blocks.Clear();
                    EditorRichTextBox.Document.Blocks.Add(new Paragraph(new Run(cachedContent)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load standard cache: {ex.Message}");
            }
            finally
            {
                _isLoaded = true;
            }
        }

        private async void EditorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded) return;

            TextRange textRange = new TextRange(
                EditorRichTextBox.Document.ContentStart,
                EditorRichTextBox.Document.ContentEnd
            );
            
            await _cacheManager.SaveManuscriptAsync(_currentManuscriptId, textRange.Text);
        }
    }
}
