namespace MauiApp1.pages
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }
        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LibraryPage());
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

