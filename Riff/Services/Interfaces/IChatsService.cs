using Riff.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.Services.Interfaces
{
    public interface IChatsService
    {
        event Action<ChatDto>? OnChatCreated;
        event Action<MessageDto> OnMessageReceived;
        
        Task CreateChat(string username);
        Task SendMessage(string message);
        
        Task<List<ChatDto>> GetLocalChatsAsync();
    }
}
