using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using MauiApp1.classes;

namespace MauiApp1.pages
{
    public partial class AddBookPage : ContentPage
    {
        private string selectedFilePath;

        public AddBookPage()
        {
            InitializeComponent();
        }
        private async void OnSelectFileClicked(object sender, EventArgs e)
        {
            try
            {
                var customEpubFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/epub+zip" } },
                    { DevicePlatform.iOS, new[] { "org.idpf.epub-container" } },
                    { DevicePlatform.WinUI, new[] { ".epub" } },
                    { DevicePlatform.MacCatalyst, new[] { "org.idpf.epub-container" } }
                });

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Выберите файл EPUB",
                    FileTypes = customEpubFileType
                });

                if (result != null)
                {
                    selectedFilePath = result.FullPath;
                    SelectedFileLabel.Text = Path.GetFileName(selectedFilePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка при выборе файла: {ex.Message}", "OK");
            }
        }
        private async void OnAddBookClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                await DisplayAlert("Ошибка", "Выберите файл перед добавлением", "OK");
                return;
            }
            Book newBook = new Book(selectedFilePath);
            MessagingCenter.Send(this, "AddBook", newBook);
            await Navigation.PopModalAsync();
        }
    }
}
