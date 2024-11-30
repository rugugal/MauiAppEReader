using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.classes;

namespace MauiApp1.interfaces
{
    public class IBook
    {
        public string FilePath { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public List<Chapter> Chapters { get; private set; }
        public byte[] CoverImage { get; private set; }
    }
}
