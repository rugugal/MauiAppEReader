using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VersOne.Epub;

namespace MauiApp1.classes
{
    public class Book
    {
        public int BookId { get; private set; }
        public string FilePath { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public List<Chapter> Chapters { get; private set; }
        public byte[] CoverImage { get; private set; }

        public Book(string filePath)
        {
            FilePath = filePath;
            EpubBook epubBook = EpubReader.ReadBook(filePath);
            Title = epubBook.Title;
            Author = epubBook.Author;
            Description = epubBook.Description;
            CoverImage = epubBook.CoverImage;
            Chapters = new List<Chapter>();

            // Открытие книги для извлечения навигации и контента
            var bookRef = EpubReader.OpenBook(FilePath);

            // Перебор элементов навигации (глав)
            var navigationItems = GetNavigationItems(bookRef.GetNavigation());

            // Сопоставление глав с контентом из ReadingOrder
            int chapterIndex = 0;
            foreach (var navigationItem in navigationItems)
            {
                // Извлекаем соответствующий текст из ReadingOrder

                string chapterContent;
                // Добавляем главу в список
                while (epubBook.ReadingOrder[chapterIndex].Key != navigationItem.Key) {
                    chapterContent = ExtractPlainText(epubBook.ReadingOrder[chapterIndex].Content);
                    Chapters.Add(new Chapter("", chapterContent));
                    chapterIndex++;
                }
                chapterContent = ExtractPlainText(epubBook.ReadingOrder[chapterIndex].Content);
                Chapters.Add(new Chapter(navigationItem.Value, chapterContent));
                chapterIndex++;

            }
        }

        // Метод для получения элементов навигации (глав) с сохранением их ключей в словарь
        private Dictionary<string, string> GetNavigationItems(List<EpubNavigationItemRef> navigationItemRefs)
        {
            var titles = new Dictionary<string, string>();

            foreach (var navigationItemRef in navigationItemRefs)
            {
                titles[navigationItemRef.HtmlContentFileRef.Key] = navigationItemRef.Title;  // Инициализируем пустыми строками

                // Если есть вложенные главы, рекурсивно их извлекаем
                foreach (var nestedNavigationItemRef in navigationItemRef.NestedItems)
                {
                    titles[nestedNavigationItemRef.HtmlContentFileRef.Key] = navigationItemRef.Title; // Инициализируем пустыми строками
                }
            }
            return titles;
        }
        //private void ExtractChapters(EpubBook epubBook)
        //{


        //    foreach (var navigationItemRef in bookRef.GetNavigation())
        //    {
        //        AddChapterFromNavigationItem(navigationItemRef, 0);
        //    }
        //}

        //private void AddChapterFromNavigationItem(EpubNavigationItemRef navigationItemRef, int indentLevel)
        //{
        //    string title ;
        //    string content;


        //    foreach (var nestedItemRef in navigationItemRef.NestedItems)
        //    {
        //        AddChapterFromNavigationItem(nestedItemRef, indentLevel + 1);
        //    }
        //    foreach (EpubLocalTextContentFile textContentFile in epubBook.ReadingOrder)
        //    {
        //        PrintTextContentFile(textContentFile);
        //    }

        //}


        private string ExtractPlainText(string htmlContent)
        {
            HtmlNode.ElementsFlags["title"] = HtmlElementFlag.Closed;
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(htmlContent);
            StringBuilder sb = new();
            foreach (HtmlNode node in htmlDocument.DocumentNode.SelectNodes("//text()"))
            {
                string text = node.InnerText.Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    sb.AppendLine(text);
                }
            }
            return sb.ToString();
        }
    }


    public class Chapter
    {
        public string Title { get; private set; }
        public string Content { get; private set; }

        public Chapter(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}


