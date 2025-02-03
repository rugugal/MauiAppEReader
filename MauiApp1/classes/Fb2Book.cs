using MauiApp1.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document;
using Fb2.Document.Constants;
using Fb2.Document.Models.Base;

namespace MauiApp1.classes
{
    public class Fb2Book : Book
    {
        public string id { get;  set; }
        public string UserId { get;  set; }
        public string FilePath { get;  set; }
        public string Title { get;  set; }
        public string Author { get;  set; }
        public string Description { get;  set; }
        public string AddedDate { get; set; }

        public int currentPage { get; set; }
        public List<Chapter> Chapters { get;  set; } = new List<Chapter>();
        public byte[] CoverImage { get;  set; }


        public Fb2Book(string filePath)
        {
            FilePath = filePath;
            if (!IsValidFb2File(filePath))
            {
                // Вывод предупреждения и пропуск файла
                Application.Current.MainPage.DisplayAlert("Ошибка", $"Файл \"{Path.GetFileName(filePath)}\" не является корректным FB2.", "OK");
                return;
            }
            Fb2Document fb2Document;
            currentPage = 0;
            // Загружаем содержимое файла
            string fileContent = File.ReadAllText(filePath);
            fb2Document = new Fb2Document();
            fb2Document.Load(fileContent);
            UserId = App._authClient.User.Info.Uid;
            // Инициализация полей
            Author = fb2Document.Title.Content[1].ToString().Replace($"{Environment.NewLine}", " ");
            Title = fb2Document.Title.Content[2].ToString();
            Description = fb2Document.Title.Content[3].ToString();
            AddedDate = DateTime.Now.ToString();
            // Обработка глав
            foreach (var page in fb2Document.Bodies[0].Content)
            {
                ProcessNode(page, Chapters);
            }
            Chapters.Add(new Chapter("Конец", "Конец"));
            string base64Image = fb2Document.BinaryImages[0].ToString().Replace("data:image/jpeg;base64,", "");

            CoverImage = Convert.FromBase64String(base64Image);
        }

        private void ProcessNode(Fb2Node node, List<Chapter> chapters)
        {
            string title = "";
            string content = node.ToString();
            if (node is Fb2Container fb2Cont)
            {
                var titleNode = fb2Cont.GetDescendants(ElementNames.Title).FirstOrDefault();
                if (titleNode != null)
                {
                    title = titleNode.ToString().Trim();
                }
                if (fb2Cont.GetDescendants(ElementNames.BookBodySection).FirstOrDefault() != null)
                {
                    foreach (var childNode in fb2Cont.Content)
                    {
                        ProcessNode(childNode, chapters);
                    }
                }
                else
                {
                    chapters.Add(new Chapter(title, content));
                }
            }
            else
            {
                chapters.Add(new Chapter(title, content));
            }
        }
        // Метод для проверки валидности FB2 файла
        private bool IsValidFb2File(string filePath)
        {
            try
            {
                // Попытка загрузить документ FB2
                string fileContent = File.ReadAllText(filePath);
                Fb2Document fb2Document = new Fb2Document();
                fb2Document.Load(fileContent);

                // Проверяем, есть ли тела (Bodies) и заголовок
                return fb2Document.Bodies.Any() && fb2Document.Title != null;
            }
            catch
            {
                // Если произошла ошибка, файл считается невалидным
                return false;
            }
        }

    }
}
