using Firebase.Auth;
using Firebase.Database;
using Microsoft.Maui.Controls; 

namespace MauiApp1
{
    public partial class App : Application
    {
        public static FirebaseAuthClient _authClient;
        public static FirebaseClient _firebaseClient;

        public App(FirebaseAuthClient authClient, FirebaseClient firebaseClient)
        {
            InitializeComponent();
            _authClient = authClient;
            _firebaseClient = firebaseClient;

            // Проверяем, есть ли текущий пользователь
            if (_authClient.User != null)
            {
                // Пользователь авторизован, направляем на MainPage
                MainPage = new AppShell();
                Shell.Current.GoToAsync("//Library");
            }
            else
            {
                // Пользователь не авторизован, направляем на SignInPage
                MainPage = new AppShell();
                Shell.Current.GoToAsync("//LoginPage");
            }

        }

    }
}
