using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth;
using Microsoft.Extensions.Logging;
using MauiApp1.pages;
using Firebase.Database;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = "AIzaSyAiLgZcD7Sy06IkwDVD3cJ30TG4HHhVBYA",
                AuthDomain = "book-firebase-app.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
               {
                    new EmailProvider()
               },
                UserRepository = new FileUserRepository("LibraryApp")
            }));

            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginVM>();
            builder.Services.AddSingleton<SignUpPage>();
            builder.Services.AddSingleton<SignUpVM>();

            builder.Services.AddSingleton(new FirebaseClient("https://book-firebase-app-default-rtdb.europe-west1.firebasedatabase.app/"));


            return builder.Build();
        }
    }
}
