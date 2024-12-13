using Microsoft.Maui.Controls;
using System;
using MauiApp1.classes;
using System.Collections.ObjectModel;
using MauiApp1.interfaces;
using Microsoft.Maui.Layouts;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace MauiApp1.pages
{
    public partial class LibraryPage : ContentPage
    {
        public ObservableCollection<Book> Books { get; set; }

        public LibraryPage()
        {
            InitializeComponent();
            Name.Text = App._authClient.User.Info.DisplayName;
            var vm = new LoginVM(App._authClient);
            BindingContext = vm;
            // Добавляем обработчик события для изменения размера окна
            this.SizeChanged += OnPageSizeChanged;
            SortPicker.SelectedIndexChanged += OnSortChanged;
            SortPicker.SelectedItem = "По дате добавления";
            LoadUserBooksAsync();
        }

        private void OnPageSizeChanged(object sender, EventArgs e)
        {
            ScrollMenu.HeightRequest = Window.Height - Panel.Height - SortPicker.Height - SearchBar.Height - 120;
        }
        private async Task LoadUserBooksAsync()
        {
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            string userId = App._authClient.User.Uid;
            try
            {
                var books = await App._firebaseClient
                    .Child("Books")
                    .OrderBy("UserId")
                    .EqualTo(userId)
                    .OnceAsync<Book>();
                BookStack.Children.Clear();
                var bookList = books.Select(firebaseBook =>
                {
                    var book = firebaseBook.Object;
                    book.id = firebaseBook.Key;
                    return book;
                }).ToList();
                Books = new ObservableCollection<Book>(bookList);
                SortBooks(bookList);
                foreach (var book in bookList)
                {
                    AddBookToLibrary(book);
                }
            }
            catch (FirebaseException ex)
            {
                Console.WriteLine($"Ошибка Firebase: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Исключение: {ex}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение: {ex}");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                ScrollMenu.HeightRequest = Window.Height - Panel.Height - SortPicker.Height - SearchBar.Height - 120;
            }
        }
        private void SortBooks(List<Book> books)
        {
            string selectedSortOption = SortPicker.SelectedItem.ToString();
            switch (selectedSortOption)
            {
                case "По названию":
                    books.Sort((b1, b2) => string.Compare(b1.Title, b2.Title, StringComparison.OrdinalIgnoreCase));
                    break;

                case "По автору":
                    books.Sort((b1, b2) =>
                    {
                        int authorComparison = string.Compare(b1.Author, b2.Author, StringComparison.OrdinalIgnoreCase);
                        if (authorComparison == 0)
                        {
                            DateTime date1 = DateTime.ParseExact(b1.AddedDate, "dd.MM.yyyy HH:mm:ss", null);
                            DateTime date2 = DateTime.ParseExact(b2.AddedDate, "dd.MM.yyyy HH:mm:ss", null);
                            return date1.CompareTo(date2);
                        }
                        return authorComparison;
                    });

                    break;

                case "По дате добавления":
                    books.Sort((b1, b2) =>
                    {
                        DateTime date1 = DateTime.ParseExact(b1.AddedDate, "dd.MM.yyyy HH:mm:ss", null);
                        DateTime date2 = DateTime.ParseExact(b2.AddedDate, "dd.MM.yyyy HH:mm:ss", null);
                        return date1.CompareTo(date2);
                    });

                    break;
                default:
                    break;
            }
        }

        private void OnSortChanged(object sender, EventArgs e)
        {
            // После изменения сортировки, перезагружаем книги с применением новой сортировки
            LoadUserBooksAsync();
        }

        private void AddBookToLibrary(Book book)
        {
            var bookGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto }, 
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto } 
                },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto } 
                    },
                Padding = new Thickness(10),
                RowSpacing = 5,
                ColumnSpacing = 10
            };
            var bookCover = new Image
            {
                Source = ImageSource.FromStream(() => new MemoryStream(book.CoverImage)),
                HeightRequest = 150,
                WidthRequest = 100,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };
            bookGrid.Add(bookCover, 0, 0);
            var textContainer = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }, // Название книги
                    new RowDefinition { Height = GridLength.Auto }, // Автор книги
                    new RowDefinition { Height = GridLength.Auto }, // Описание книги
                    new RowDefinition { Height = GridLength.Auto }  // Прогресс чтения
                }
            };
            var bookTitle = new Label
            {
                Text = book.Title.Trim(),
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Colors.Black,
                LineBreakMode = LineBreakMode.WordWrap
            };
            textContainer.Add(bookTitle, 0, 0);
            var bookAuthor = new Label
            {
                Text = book.Author,
                FontSize = 14,
                TextColor = Colors.DarkGray,
                LineBreakMode = LineBreakMode.WordWrap
            };
            textContainer.Add(bookAuthor, 0, 1);
            var bookDescription = new Label
            {
                Text = book.Description.Length > 200
                    ? book.Description.Substring(0, 200) + "..."
                    : book.Description,
                FontSize = 12,
                TextColor = Colors.Gray,
                LineBreakMode = LineBreakMode.WordWrap
            };
            textContainer.Add(bookDescription, 0, 2);
            if (book.Description.Length > 200)
            {
                var toggleButton = new Button
                {
                    Text = "Показать еще",
                    FontSize = 12,
                    HorizontalOptions = LayoutOptions.Start,
                    BackgroundColor = Colors.Transparent, 
                    TextColor = Colors.Blue, 
                    BorderWidth = 0, 
                    Padding = new Thickness(0) 
                };
                toggleButton.Clicked += (s, e) =>
                {
                    if (toggleButton.Text == "Показать еще")
                    {
                        bookDescription.Text = book.Description;
                        toggleButton.Text = "Скрыть";
                    }
                    else
                    {
                        bookDescription.Text = book.Description.Substring(0, 200) + "...";
                        toggleButton.Text = "Показать еще";
                    }
                };

                textContainer.Add(toggleButton, 0, 3);
            }
            bookGrid.Add(textContainer, 1, 0);
            var bookProgress = new Label
            {
                Text = $"Прочитано: {(int)book.ReadingProgress}%",
                FontSize = 12,
                TextColor = Colors.DarkGray,
                HorizontalOptions = LayoutOptions.Start
            };
            textContainer.Add(bookProgress, 0, 4);
            var deleteButton = new Button
            {
                Text = "Удалить",
                HeightRequest = 80,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                Navigation.PushModalAsync(new BookReaderPage(book));
            };
            bookGrid.GestureRecognizers.Add(tapGesture);
            var separator = new BoxView
            {
                Margin = 10,
                Color = Colors.Gray, 
                HeightRequest = 1,    
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            deleteButton.Clicked += async (s, e) =>
            {
                bool confirm = await DisplayAlert("Удалить книгу", "Вы уверены, что хотите удалить эту книгу?", "Да", "Нет");
                if (confirm)
                {
                    await DeleteBookAsync(book);
                    BookStack.Children.Remove(bookGrid);
                    BookStack.Children.Remove(separator);
                }
            };

            bookGrid.Add(deleteButton, 2, 0);
            BookStack.Children.Add(bookGrid);
            BookStack.Children.Add(separator);
            ScrollMenu.HeightRequest = Window.Height - Panel.Height - 100;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue.ToLower();
            var filteredBooks = Books.Where(book =>
                book.Title.ToLower().Contains(searchText) ||
                book.Author.ToLower().Contains(searchText)).ToList();
            BookStack.Children.Clear();
            foreach (var book in filteredBooks)
            {
                AddBookToLibrary(book);
            }
        }
        private async void Load(object sender, EventArgs e)
        {
            await LoadUserBooksAsync();
        }

        private async Task DeleteBookAsync(Book book)
        {
            try
            {
                await App._firebaseClient
                    .Child("Books")
                    .Child(book.id)
                    .DeleteAsync();

                Console.WriteLine($"Книга '{book.Title}' успешно удалена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении книги: {ex.Message}");
                await DisplayAlert("Ошибка", "Не удалось удалить книгу. Попробуйте снова.", "ОК");
            }
        }



        private void OnAddBookClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new AddBookPage());
        }

    }
}
