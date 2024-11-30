using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VersOne.Epub;
using MauiApp1.interfaces;

namespace MauiApp1.classes
{
    public class EpubBook : IBook
    {
        public string FilePath { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public List<Chapter> Chapters { get; private set; }
        public byte[] CoverImage { get; private set; }

        public EpubBook(string filePath)
        {
            FilePath = filePath;
            VersOne.Epub.EpubBook epubBook = EpubReader.ReadBook(filePath);
            Title = epubBook.Title;
            Author = epubBook.Author;
            Description = epubBook.Description;
            CoverImage = epubBook.CoverImage;
            Chapters = new List<Chapter>();


            var bookRef = EpubReader.OpenBook(FilePath);

            var titles = new Dictionary<string, string>();
            var navigationItems = GetNavigationItems(bookRef.GetNavigation(), titles);

            // Сопоставление глав с контентом из ReadingOrder
            int chapterIndex = 0;
            foreach (var navigationItem in navigationItems)
            {
                string chapterContent;
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

        // получения глав с сохранением их ключей в словарь
        private Dictionary<string, string> GetNavigationItems(List<EpubNavigationItemRef> navigationItemRefs, Dictionary<string, string> titles)
        {
            foreach (var navigationItemRef in navigationItemRefs)
            {
                titles[navigationItemRef.HtmlContentFileRef.Key] = navigationItemRef.Title;  

                GetNavigationItems(navigationItemRef.NestedItems, titles);
            }
            return titles;
        }

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


    
}


