using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;
using MauiApp1.classes;
using MauiApp1.interfaces;
using Firebase.Auth;
using System.Windows.Input;


namespace MauiApp1.pages
{
    public partial class BookReaderPage : ContentPage
    {
        private readonly Book _book;
        public ObservableCollection<Chapter> Chapters => new(_book.Chapters);
        public ObservableCollection<Chapter> FilteredChapters
        {
            get
            {
                return new ObservableCollection<Chapter>(_book.Chapters.Where(c => !string.IsNullOrWhiteSpace(c.Title)));
            }
        }

        public string CurrentPageText { get; private set; } = string.Empty;

        private double _fontSize = 20;
        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged(nameof(FontSize));
                    DisplayCurrentChapter(); // Обновляем отображение текста при изменении шрифта
                }
            }
        }

        public ObservableCollection<string> Quotes { get; private set; } = new ObservableCollection<string>();

        public BookReaderPage(Book book)
        {
            InitializeComponent();
            _book = book;
            BackgroundPicker.Items.Add("Белый");
            BackgroundPicker.Items.Add("Черный");
            BackgroundPicker.Items.Add("Сепия");
            BindingContext = this;
            BackgroundPicker.SelectedIndex = 0;
            this.SizeChanged += OnSizeChanged;

        }
        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (PageView.Width > 0 && PageView.Height > 0)
            {
                DisplayCurrentChapter();
            }
        }

        private async void DisplayCurrentChapter()
        {
            if (_book.currentPage >= 0 && _book.currentPage < _book.Chapters.Count)
            {
                var currentChapter = _book.Chapters[_book.currentPage];
                CurrentPageText = currentChapter.Content;
                OnPropertyChanged(nameof(CurrentPageText));
                PageView.HeightRequest = Window.Height - SettingsPanel.Height - NavigationPanel.Height - 100;
                await _book.UpdateCurrentPageAsync();
                await PageView.ScrollToAsync(0, 0, true);
            }
        }
        private void OnFontSizeSliderChanged(object sender, ValueChangedEventArgs e)
        {
            FontSize = e.NewValue;
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var colors = BackgroundPicker.SelectedIndex switch
            {
                0 => (Colors.White, Colors.Black),
                1 => (Colors.Black, Colors.White),
                2 => (Color.FromArgb("#F5DEB3"), Colors.Black),
                _ => (Colors.White, Colors.Black)
            };

            BackgroundColor = colors.Item1;
            PageLabel.TextColor = colors.Item2;
            BackgroundPicker.TextColor = colors.Item2;
            FontSizeSlider.MinimumTrackColor = colors.Item2;
            FontSizeSlider.ThumbColor = colors.Item2;
            BackgroundPicker.BackgroundColor = colors.Item1;
        }
        private void OnTableOfContentsClicked(object sender, EventArgs e)
        {
            TableOfContentsView.IsVisible = !TableOfContentsView.IsVisible;
            TableOfContentsView.HeightRequest = Window.Height - SettingsPanel.Height;
        }

        private void OnChapterSelected(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedChapter = _book.Chapters.FirstOrDefault(c => c.Title == button.Text);
            if (selectedChapter != null)
            {
                _book.currentPage = _book.Chapters.IndexOf(selectedChapter);
                TableOfContentsView.IsVisible = false; 
                DisplayCurrentChapter();
            }
        }
        private void OnPreviousPageClicked(object sender, EventArgs e)
        {
            if (_book.currentPage > 0)
            {
                _book.currentPage--;
                DisplayCurrentChapter();
            }
        }
        private void OnNextPageClicked(object sender, EventArgs e)
        {
            if (_book.currentPage < _book.Chapters.Count - 1)
            {
                _book.currentPage++;
                DisplayCurrentChapter();
                
            }
        }
        private async void OnBackClicked(object sender, EventArgs e)
        {

            await Shell.Current.GoToAsync("//Library");
        }
    }
}
