using MauiApp1.pages;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage()); 
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage()); 
        }
    }

}
