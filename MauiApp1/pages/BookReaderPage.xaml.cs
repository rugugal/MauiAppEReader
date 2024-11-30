using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;
using System.Text;
using MauiApp1.classes;

namespace MauiApp1.pages
{
    public partial class BookReaderPage : ContentPage
    {
        private readonly EpubBook _book;
        private readonly List<string> _pages = new();
        private int _currentPage = 0;

        public ObservableCollection<Chapter> Chapters => new(_book.Chapters);
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
                    ReloadPages(); // Обновляем страницы при изменении шрифта
                }
            }
        }

        public BookReaderPage(EpubBook book)
        {
            InitializeComponent();
            _book = book;

            // Инициализация выбора фона
            BackgroundPicker.Items.Add("Белый");
            BackgroundPicker.Items.Add("Черный");
            BackgroundPicker.Items.Add("Светло-коричневый");

            BindingContext = this;

            this.SizeChanged += OnSizeChanged;


        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (_pages.Count == 0 && PageView.Width > 0 && PageView.Height > 0)
            {
                ReloadPages();
            }
        }

        private void ReloadPages()
        {
            if (PageView.Width <= 0 || PageView.Height <= 0) return;

            var currentPageBeforeReload = _currentPage; 
            LoadPages(FontSize);

            if (currentPageBeforeReload < _pages.Count)
            {

                _currentPage = currentPageBeforeReload;
            }
            else
            {
                _currentPage = 0;  
            }

            DisplayCurrentPage();
        }

        private void LoadPages(double fontSize)
        {
            _pages.Clear();
            _currentPage = 0;

            double width = PageView.Width;
            double height = PageView.Height;

            if (double.IsNaN(width) || double.IsNaN(height)) return;

            foreach (var chapter in _book.Chapters)
            {
                var textToProcess = $"{chapter.Title}\n\n{chapter.Content}";

                var pageBuilder = new StringBuilder();

                // Разбиваем текст на строки, где каждая строка — это часть текста, ограниченная длиной
                foreach (var line in textToProcess.Split('\n'))
                {
                    // Очищаем строку от лишних пробелов
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                    // Пробуем добавить текущую строку на страницу
                    var testText = pageBuilder.Length > 0
                        ? $"{pageBuilder}\n{trimmedLine}"
                        : trimmedLine;

                    // Создаем временный Label для оценки размера текста
                    var label = new Label
                    {
                        Text = testText,
                        FontSize = fontSize,
                        LineBreakMode = LineBreakMode.WordWrap
                    };

                    var sizeRequest = label.Measure(width, height);

                    // Если текст превышает высоту страницы, сохраняем текущую страницу
                    if (sizeRequest.Request.Height > height)
                    {
                        _pages.Add(pageBuilder.ToString().TrimEnd());
                        pageBuilder.Clear();
                        pageBuilder.Append(trimmedLine);
                    }
                    else
                    {
                        // Текст помещается, продолжаем добавлять
                        pageBuilder.Clear();
                        pageBuilder.Append(testText);
                    }
                }

                // Добавляем оставшийся текст из главы
                if (pageBuilder.Length > 0)
                {
                    _pages.Add(pageBuilder.ToString().TrimEnd());
                }
            }

            DisplayCurrentPage();
        }








        private void DisplayCurrentPage()
        {
            if (_currentPage >= 0 && _currentPage < _pages.Count)
            {
                CurrentPageText = _pages[_currentPage];
                OnPropertyChanged(nameof(CurrentPageText));
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
        }

        private void OnTableOfContentsClicked(object sender, EventArgs e)
        {
            TableOfContentsView.IsVisible = !TableOfContentsView.IsVisible;
        }

        private void OnChapterSelected(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedChapter = _book.Chapters.FirstOrDefault(c => c.Title == button.Text);
            if (selectedChapter != null)
            {
                _currentPage = _pages.FindIndex(p => p.StartsWith(selectedChapter.Title, StringComparison.OrdinalIgnoreCase));

                if (_currentPage == -1) _currentPage = 0; // Если не нашли, начинаем с начала
                TableOfContentsView.IsVisible = false; // Скрываем оглавление
                DisplayCurrentPage();
            }
        }

        private void OnPreviousPageClicked(object sender, EventArgs e)
        {
            GoToPreviousPage();
        }

        private void OnNextPageClicked(object sender, EventArgs e)
        {
            GoToNextPage();
        }

        private async void GoToPreviousPage()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                DisplayCurrentPage();
                await PageView.ScrollToAsync(0, 0, true);
            }
        }

        private async void GoToNextPage()
        {
            if (_currentPage < _pages.Count - 1)
            {
                _currentPage++;
                DisplayCurrentPage();
                await PageView.ScrollToAsync(0, 0, true);
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new LibraryPage());
        }
    }
}
