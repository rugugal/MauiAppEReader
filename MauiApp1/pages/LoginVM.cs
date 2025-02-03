using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.pages
{
    public partial class LoginVM : ObservableObject
    {
        public readonly FirebaseAuthClient _authClient;
        [ObservableProperty]
        private string _email;
        [ObservableProperty]
        private string _password;
        private string _username;
        public string Username
        {
            get => _username;
            private set => SetProperty(ref _username, value);
        }

        [ObservableProperty]
        private string _secretMessage;
        public LoginVM(FirebaseAuthClient authClient)
        {
            if (_authClient != authClient) _authClient = authClient;
        }

        [RelayCommand]
        private async Task SignIn()
        {
            if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите корректный email.", "ОК");
                return;
            }
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Пароль должен содержать минимум 6 символов.", "ОК");
                return;
            }
            try
            {
                var user = await _authClient.SignInWithEmailAndPasswordAsync(Email, Password);
                Username = user.User?.Info?.DisplayName;
                App._authClient = _authClient;
                var newLibraryPage = new LibraryPage();
                await Application.Current.MainPage.Navigation.PushModalAsync(newLibraryPage);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка входа", "Пользователь с такими данными не зарегистрирован", "ОК");
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        [RelayCommand]
        private async Task SignOut()
        {
            _authClient.SignOut();
            await Shell.Current.GoToAsync("//SignUp");
        }
        [RelayCommand]
        private async Task NavigateSignUp()
        {
            await Shell.Current.GoToAsync("//SignUp");
        }

    }

}
