using Microsoft.Maui.Controls; 

namespace MauiApp1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Оборачиваем AppShell в NavigationPage
            MainPage = new NavigationPage(new AppShell());
        }
    }
}
