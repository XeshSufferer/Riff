using Microsoft.AspNetCore.SignalR.Client;
using Riff.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.ConnectionRepository
{
    public class GatewayConnectionRepository : IGatewayConnectionRepository, IJwtDependentService
    {

        private readonly string _hubUrl;
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public HubConnection Connection { get; set; }



        public event Action<string> OnRegisterSuccess;
        public event Action OnRegisterFailed;

        public event Action<string> OnLoginSuccess;
        public event Action OnLoginFailed;

        public event Action<ChatDto> OnChatCreated;
        public event Action<MessageDto> OnMessageReceived;

        public GatewayConnectionRepository(string serverUrl, string prefix)
        {
            _hubUrl = serverUrl + prefix;
        }

        public async Task InitializeConnection()
        {
            if (Connection != null)
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
            }

            Connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect()
                .Build();

            SubscribeAllServerEvents();

            await Connection.StartAsync();
        }

        public async Task SetJwtToken(string token)
        {
            var _jwtToken = token;
            if (Connection != null)
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
            }

            Connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(_jwtToken);
                })
                .WithAutomaticReconnect()
                .Build();

            SubscribeAllServerEvents();

            await Connection.StartAsync();
        }

        private void SubscribeAllServerEvents()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription?.Dispose();
            }
            _subscriptions.Clear();

            _subscriptions.Add(Connection.On<string, string>("LoginSuccess", (token, userid) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Preferences.Set("USERID", userid);
                    OnLoginSuccess?.Invoke(token);
                });
            }));

            _subscriptions.Add(Connection.On("LoginFailed", () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnLoginFailed?.Invoke();
                });
            }));




            _subscriptions.Add(Connection.On<string, string>("RegisterSuccess", (token, userid) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Preferences.Set("USERID", userid);
                    OnLoginSuccess?.Invoke(token);
                });
            }));

            _subscriptions.Add(Connection.On("RegisterFailed", () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnLoginFailed?.Invoke();
                });
            }));

            _subscriptions.Add(Connection.On<ChatDto>("OnChatCreated", chatname =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnChatCreated?.Invoke(chatname);
                });
            }));

            _subscriptions.Add(Connection.On<MessageDto>("OnMessageReceived", message =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnMessageReceived?.Invoke(message);
                });
            }));

        }





        public async Task Login(UserLoginData data)
        {
            await Connection.SendAsync("Login", data);
        }

        public async Task Register(UserRegisterData data)
        {
            await Connection.SendAsync("Register", data);
        }

        public async Task LoginByJWT(string token)
        {
            await Connection.SendAsync("Autologin", token);
        }


        public async Task CreateChat(string username)
        {
            System.Diagnostics.Debug.WriteLine($"Отправляем запрос на создание чата с пользователем: {username}");
            await Connection.SendAsync("CreateChatWith", username);
            System.Diagnostics.Debug.WriteLine("Запрос на создание чата отправлен");
        }

        public async Task SendMessage(MessageSendingDTO data)
        {
            await Connection.SendAsync("SendMessage", data);
        }
    }
}
