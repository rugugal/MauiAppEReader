using Microsoft.Maui.Controls;
using System;
using MauiApp1.classes;
using System.Collections.ObjectModel;

namespace MauiApp1.pages
{
    public partial class LibraryPage : ContentPage
    {
        public ObservableCollection<Book> Books { get; set; }

        public LibraryPage()
        {
            InitializeComponent();
            // Subscribe to receive the added book
            MessagingCenter.Subscribe<AddBookPage, Book>(this, "AddBook", (sender, book) =>
            {
                AddBookToLibrary(book);
            });
        }

        // Method for adding a book to the library
        private void AddBookToLibrary(Book book)
        {
            // Создаем представление книги с обложкой, названием и автором
            var bookLayout = new VerticalStackLayout { Padding = 10 };

            // Обложка (Image)
            var bookCover = new Image
            {
                Source = ImageSource.FromStream(() => new MemoryStream(book.CoverImage)),
                HeightRequest = 150,
                HorizontalOptions = LayoutOptions.Center
            };

            // Название книги (Label)
            var bookTitle = new Label
            {
                Text = book.Title,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            // Автор книги (Label)
            var bookAuthor = new Label
            {
                Text = book.Author,
                HorizontalOptions = LayoutOptions.Center
            };

            // Обработчик нажатия на книгу
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                Navigation.PushModalAsync(new BookReaderPage(book));
            };

            bookLayout.GestureRecognizers.Add(tapGesture);

            // Добавляем элементы в StackLayout
            bookLayout.Children.Add(bookCover);
            bookLayout.Children.Add(bookTitle);
            bookLayout.Children.Add(bookAuthor);

            // Добавляем книгу в StackLayout библиотеки
            BookStack.Children.Add(bookLayout);
        }
        private void OnAddBookClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new AddBookPage());
        }
    }
}
