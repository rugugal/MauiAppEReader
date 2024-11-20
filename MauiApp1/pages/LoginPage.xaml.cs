namespace MauiApp1.pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void OnLoginClicked(object sender, EventArgs e)
        {

        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); 
        }
    }
}

