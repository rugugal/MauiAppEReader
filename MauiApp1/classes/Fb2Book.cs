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
    public class Fb2Book : IBook
    {
        public string FilePath { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public List<Chapter> Chapters { get; private set; } = new List<Chapter>();
        public byte[] CoverImage { get; private set; }
        private Fb2Document fb2Document;

        public Fb2Book(string filePath)
        {
            FilePath = filePath;

            // Загружаем содержимое файла
            string fileContent = File.ReadAllText(filePath);
            fb2Document = new Fb2Document();
            fb2Document.Load(fileContent);

            // Инициализация полей
            Title = fb2Document.Title.Content[1].ToString().Replace($"{Environment.NewLine}", " ");
            Author = fb2Document.Title.Content[2].ToString();
            Description = fb2Document.Title.Content[3].ToString();

            // Обработка глав
            foreach (var page in fb2Document.Bodies[0].Content)
            {
                ProcessNode(page, Chapters);
            }
            string base64Image = fb2Document.BinaryImages[0].ToString().Replace("data:image/jpeg;base64,", "");

            CoverImage = Convert.FromBase64String(base64Image);
        }

        private void ProcessNode(Fb2Node node, List<Chapter> chapters)
        {
            string title = "";
            string content = node.ToString();

            // Если текущий узел является контейнером
            if (node is Fb2Container fb2Cont)
            {
                // Проверяем наличие заголовка
                var titleNode = fb2Cont.GetDescendants(ElementNames.Title).FirstOrDefault();
                if (titleNode != null)
                {
                    title = titleNode.ToString().Trim();
                }

                // Добавляем текущую главу
                chapters.Add(new Chapter(title, content));

                // Обрабатываем вложенные узлы
                if (fb2Cont.GetDescendants(ElementNames.BookBodySection).FirstOrDefault() != null)
                {
                    foreach (var childNode in fb2Cont.Content)
                    {
                        ProcessNode(childNode, chapters); // Рекурсивный вызов для вложенных глав
                    }
                }
            }
            else
            {
                // Если это не контейнер, добавляем узел как главу без заголовка
                chapters.Add(new Chapter(title, content));
            }
        }

    }
}
