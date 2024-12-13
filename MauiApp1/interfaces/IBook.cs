using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database.Query;
using MauiApp1.classes;

namespace MauiApp1.interfaces
{

    public class Book
    {
        public string id { get; set; }
        public string UserId { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string AddedDate { get; set; }
        public int currentPage { get; set; }
        public List<Chapter> Chapters { get; set; }
        public byte[] CoverImage { get; set; }
        public int ReadingProgress => Chapters != null && Chapters.Count > 0
         ? (currentPage) * 100 / (Chapters.Count == 0 ? Chapters.Count : Chapters.Count + 1)
         : 0;
        public async Task UpdateCurrentPageAsync()
        {
            if (!string.IsNullOrEmpty(id))
            {
                var firebaseClient = App._firebaseClient;
                await firebaseClient
                    .Child("Books")
                    .Child(id)
                    .Child("currentPage")
                    .PutAsync(currentPage);
            }
        }
    }
}
    
