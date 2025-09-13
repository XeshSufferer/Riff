using Riff.ConnectionRepository;
using Riff.Managers;
using Riff.Services.Interfaces;
using System.Xml.Serialization;

namespace Riff.Services
{
    public class AccountService : IAccountService
    {
        private readonly IGatewayConnectionRepository _connection;
        private readonly IJwtManager _jwtManager;

        public event Action<string> OnLoginSuccess
        {
            add => _connection.OnLoginSuccess += value;
            remove => _connection.OnLoginSuccess -= value;
        }

        public event Action OnLoginFailed
        {
            add => _connection.OnLoginFailed += value;
            remove => _connection.OnLoginFailed -= value;
        }

        public event Action<string> OnRegisterSuccess
        {
            add => _connection.OnRegisterSuccess += value; 
            remove => _connection.OnRegisterSuccess -= value;
        }

        public event Action OnRegisterFailed
        {
            add => _connection.OnRegisterFailed += value;
            remove => _connection.OnRegisterFailed -= value;
        }

        public AccountService(IGatewayConnectionRepository connection, IJwtManager jwtManager)
        {
            _jwtManager = jwtManager;
            _connection = connection;

            SubscribeEvent();
            TryAutologinBySavedJWT();
        }

        private void SubscribeEvent()
        {
            _connection.OnLoginSuccess += SetGlobalJWT;
            _connection.OnRegisterSuccess += SetGlobalJWT;
        }

        private void SetGlobalJWT(string token) => _jwtManager.SetGlobalJwt(token);


        private async void TryAutologinBySavedJWT()
        {
            string token = await SecureStorage.GetAsync("TOKEN");
            if(token != null)
            {
                await _jwtManager.SetGlobalJwt(token);
            }
            await _connection.LoginByJWT(token);
        }

        public async Task Login(string login, string password)
        {
            await _connection.Login(new Models.UserLoginData
            {
                Login = login,
                Password = password
            });
        }

        public async Task Register(string name, string login, string password)
        {
            await _connection.Register(new Models.UserRegisterData
            {
                Password = password,
                Login = login,
                Nickname = name
            });
        }
    }
}
