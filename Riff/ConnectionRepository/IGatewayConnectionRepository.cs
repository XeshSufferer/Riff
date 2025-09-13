using Riff.Models;
using Riff.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.ConnectionRepository
{
    public interface IGatewayConnectionRepository
    {
        Task InitializeConnection();
        Task Register(UserRegisterData data);
        Task Login(UserLoginData data);
        Task LoginByJWT(string token);
        Task CreateChat(string username);
        Task SendMessage(MessageSendingDTO data);

        public event Action<string> OnRegisterSuccess;
        public event Action OnRegisterFailed;

        public event Action<string> OnLoginSuccess;
        public event Action OnLoginFailed;

        public event Action<ChatDto> OnChatCreated;
        public event Action<MessageDto> OnMessageReceived;
    }
}
