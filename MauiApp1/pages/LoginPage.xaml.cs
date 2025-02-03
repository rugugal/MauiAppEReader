namespace MauiApp1.pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginVM loginvm)
        {
            InitializeComponent();
            BindingContext = loginvm;
        }
    }
}

