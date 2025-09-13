using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Riff.ConnectionRepository;
using Riff.Models;
using Riff.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.ViewModels
{
    public partial class RegisterPageViewModel : ObservableObject, IDisposable
    {

        private readonly IGatewayConnectionRepository _gateway;
        private readonly IAccountService _accounts;

        [ObservableProperty]
        private string _login;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _nickname;

        public RegisterPageViewModel(IGatewayConnectionRepository gateway, IAccountService accounts)
        {
            _gateway = gateway;
            _accounts = accounts;
        }


        public void Sub()
        {
            _accounts.OnRegisterSuccess += GotoMainPage;
            _accounts.OnLoginSuccess += GotoMainPage;
        }

        public void Dispose()
        {
            _accounts.OnRegisterSuccess -= GotoMainPage;
            _accounts.OnLoginSuccess -= GotoMainPage;
        }



        private async void GotoMainPage(string token)
        {
            await Shell.Current.GoToAsync(nameof(ChatsPage));
        }

        [RelayCommand]
        public async Task GotoLoginPage()
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }


        [RelayCommand]
        public async Task RegisterButton()
        {
            await _accounts.Register(Nickname, Login, Password);
        }
    }
}
