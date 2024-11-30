using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.classes
{
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
