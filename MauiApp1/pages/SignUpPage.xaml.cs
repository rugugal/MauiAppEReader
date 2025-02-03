namespace MauiApp1.pages
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage(SignUpVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}

