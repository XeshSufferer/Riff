using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Riff.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject, IDisposable
    {

        private readonly IAccountService _accountService;


        [ObservableProperty]
        private string _login;

        [ObservableProperty]
        private string _password;

        public LoginPageViewModel(IAccountService accountService) 
        {
            _accountService = accountService;
        }

        public void Sub()
        {
            _accountService.OnLoginSuccess += GotoMainPage;
        }

        private async void GotoMainPage(string token)
        {
            await Shell.Current.GoToAsync(nameof(ChatsPage));
        }

        public void Dispose()
        {
            _accountService.OnLoginSuccess -= GotoMainPage;
        }

        [RelayCommand]
        private async Task LoginButton()
        {
            await _accountService.Login(Login, Password);
        }
    }
}
