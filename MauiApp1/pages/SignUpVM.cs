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
    public partial class SignUpVM : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;
        [ObservableProperty]
        private string _email;
        [ObservableProperty]
        private string _username;
        [ObservableProperty]
        private string _password;
        public SignUpVM(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }
        [RelayCommand]
        private async Task SignUp()
        {
            if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите корректный email.", "ОК");
                return;
            }
            if (string.IsNullOrWhiteSpace(Username))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Имя пользователя не может быть пустым.", "ОК");
                return;
            }
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Пароль должен содержать минимум 6 символов.", "ОК");
                return;
            }
            try
            {
                await _authClient.CreateUserWithEmailAndPasswordAsync(Email, Password, Username);
                await Shell.Current.GoToAsync("//SignIn");
            }
            catch (FirebaseAuthException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка регистрации", ex.Message, "ОК");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка регистрации", "Произошла ошибка. Попробуйте снова.", "ОК");
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
        private async Task NavigateSignIn()
        {
            await Shell.Current.GoToAsync("//SignIn");
        }
    }
}
