using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using MauiApp1.classes;
using MauiApp1.interfaces;
using Firebase.Database;
using System.Collections.ObjectModel;
using System.Text;

namespace MauiApp1.pages
{
    
    public partial class AddBookPage : ContentPage
    {
        private ObservableCollection<string> selectedFilePaths = new ObservableCollection<string>();

        public AddBookPage()
        {
            InitializeComponent();
            SelectedFilesList.ItemsSource = selectedFilePaths;
        }

        private async void OnSelectFilesClicked(object sender, EventArgs e)
        {
            try
            {
                var customFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/epub+zip", "application/x-fictionbook+xml" } },
                { DevicePlatform.iOS, new[] { "org.idpf.epub-container", "application/x-fictionbook+xml" } },
                { DevicePlatform.WinUI, new[] { ".epub", ".fb2" } },
                { DevicePlatform.MacCatalyst, new[] { "org.idpf.epub-container", "application/x-fictionbook+xml" } }
            });

                var results = await FilePicker.Default.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "�������� ����� ����",
                    FileTypes = customFileTypes
                });

                if (results != null)
                {
                    foreach (var result in results)
                    {
                        if (!selectedFilePaths.Contains(result.FullPath))
                        {
                            selectedFilePaths.Add(result.FullPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", $"������ ��� ������ ������: {ex.Message}", "OK");
            }
        }

        private async void OnAddBooksClicked(object sender, EventArgs e)
        {
            if (selectedFilePaths.Count == 0)
            {
                await DisplayAlert("������", "�������� ���� �� ���� ���� ����� �����������", "OK");
                return;
            }

            var successfullyAdded = new List<string>();
            var failedToAdd = new Dictionary<string, string>(); // �������: ���� -> ������� ������

            foreach (var filePath in selectedFilePaths)
            {
                try
                {
                    Book newBook = await BookFactory.CreateBookAsync(filePath);
                    successfullyAdded.Add(Path.GetFileName(filePath));
                }
                catch (Exception ex)
                {
                    failedToAdd[filePath] = ex.Message;
                }
            }

            // ��������� �������� ���������
            var resultMessage = new StringBuilder();
            if (successfullyAdded.Count > 0)
            {
                resultMessage.AppendLine("������� ��������� ��������� �����:");
                foreach (var book in successfullyAdded)
                {
                    resultMessage.AppendLine($"- {book}");
                }
            }

            if (failedToAdd.Count > 0)
            {
                resultMessage.AppendLine();
                resultMessage.AppendLine("�� ������� �������� ��������� �����:");
                foreach (var failed in failedToAdd)
                {
                    resultMessage.AppendLine($"- {Path.GetFileName(failed.Key)}: {failed.Value}");
                }
            }

            await DisplayAlert("���������", resultMessage.ToString(), "OK");

            // ���� ���� �� ���� ����� ���� ������� ���������, ��������� � �������� ����������
            if (successfullyAdded.Count > 0)
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new LibraryPage());
            }
            else
            {
                await DisplayAlert("������", "�� ����� ����� �� ������� ��������. ��������� ����� � ���������� �����.", "OK");
            }
        }


        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Library");
        }
    }

}
