using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VersOne.Epub;
using MauiApp1.interfaces;
using Firebase.Database.Query;
using Firebase.Auth;

namespace MauiApp1.classes
{
    public class EpubBook : Book
    {
        public string id { get;  set; }
        public string UserId { get;  set; }
        public string FilePath { get;  set; }
        public string Title { get;  set; }
        public string Author { get;  set; }
        public string Description { get;  set; }
        public List<Chapter> Chapters { get;  set; }
        public int currentPage { get; set; }
        public string AddedDate { get; set; }
        public byte[] CoverImage { get;  set; }

        public VersOne.Epub.EpubBook epubBook;

        public EpubBook(string filePath)
        {
            FilePath = filePath;
            currentPage = 0;
            epubBook = EpubReader.ReadBook(filePath);
            Title = epubBook.Title ?? "";
            Author = epubBook.Author ?? "";
            Description = epubBook.Description ?? "";
            CoverImage = epubBook.CoverImage ?? File.ReadAllBytes("placeholder.png");
            Chapters = new List<Chapter>();
            UserId = App._authClient.User.Info.Uid;
            AddedDate = DateTime.Now.ToString();
            var bookRef = EpubReader.OpenBook(FilePath);
            var navigation = bookRef.GetNavigation();
            var titles = new Dictionary<string, string>();
            var navigationItems = GetNavigationItems(navigation, titles);
            if (navigationItems.Count == 0)
            {
                AddChaptersFromReadingOrderWithoutNavigation();
            }
            else
            {
                int chapterIndex = 0;
                foreach (var navigationItem in navigationItems)
                {
                    string chapterContent;
                    while (epubBook.ReadingOrder[chapterIndex].Key != navigationItem.Key)
                    {
                        chapterContent = ExtractPlainText(epubBook.ReadingOrder[chapterIndex].Content);
                        Chapters.Add(new Chapter("", chapterContent));
                        chapterIndex++;
                    }
                    chapterContent = ExtractPlainText(epubBook.ReadingOrder[chapterIndex].Content);
                    Chapters.Add(new Chapter(navigationItem.Value, chapterContent));
                    chapterIndex++;
                }
                for (int i = chapterIndex; i < epubBook.ReadingOrder.Count; i++)
                {
                    var chapterContent = ExtractPlainText(epubBook.ReadingOrder[i].Content);
                    Chapters.Add(new Chapter("", chapterContent));
                }
            }
            Chapters.Add(new Chapter("Конец", "Конец"));
        }
        private void AddChaptersFromReadingOrderWithoutNavigation()
        {
            foreach (var item in epubBook.ReadingOrder)
            {
                var chapterContent = ExtractPlainText(item.Content);
                Chapters.Add(new Chapter("", chapterContent));
            }
        }
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


