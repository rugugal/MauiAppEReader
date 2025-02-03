using Firebase.Database.Query;
using MauiApp1.interfaces;

namespace MauiApp1.classes
{
    public static class BookFactory
    {
        public static async Task<Book> CreateBookAsync(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            Book book = extension switch
            {
                ".epub" => new EpubBook(filePath),
                ".fb2" => new Fb2Book(filePath),
                _ => throw new NotSupportedException($"Формат файла {extension} не поддерживается.")
            };
            await SaveBookToDBAsync(book);

            return book;
        }
        private static async Task SaveBookToDBAsync(Book book)
        {
            var firebaseClient = App._firebaseClient;
            book.id = Guid.NewGuid().ToString();
            await firebaseClient
                .Child("Books")
                .Child(book.id)
                .PutAsync(book);
        }
    }
}
