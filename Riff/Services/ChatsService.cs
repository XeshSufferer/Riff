using Riff.ConnectionRepository;
using Riff.Models;
using Riff.Repositories.Interfaces;
using Riff.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Services
{
    public class ChatsService : IChatsService
    {
        public event Action<ChatDto>? OnChatCreated
        {
            add => _connection.OnChatCreated += value;
            remove => _connection.OnChatCreated -= value;
        }

        public event Action<MessageDto> OnMessageReceived
        {
            add => _connection.OnMessageReceived += value;
            remove => _connection.OnMessageReceived -= value;
        }

        private readonly IGatewayConnectionRepository _connection;
        private readonly IChatRepository _chatRepository;
        private readonly IMessageCollectionRepository _messageRepository;

        public ChatsService(IGatewayConnectionRepository connection, IChatRepository chatRepository, IMessageCollectionRepository messageRepository)
        {
            _connection = connection;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;

            _connection.OnChatCreated += OnChatCreatedHandler;
            _connection.OnMessageReceived += OnMessageReceivedHandler;
        }

        public async Task CreateChat(string username)
        {
            await _connection.CreateChat(username);
        }

        public async Task SendMessage(string message)
        {
            MessageSendingDTO data = new MessageSendingDTO()
            {
                Message = message,
                ChatId = Preferences.Get("LAST_CLICKED_CHAT_ID", "000000000000000000000000")
            };
            await _connection.SendMessage(data);
        }

        public async Task<List<ChatDto>> GetLocalChatsAsync()
        {
            var chats = await _chatRepository.GetAllActiveAsync();
            return chats.Select(chat => new ChatDto
            {
                Id = chat.Id,
                Name = chat.Name,
                Description = chat.Description,
                MembersId = System.Text.Json.JsonSerializer.Deserialize<List<string>>(chat.MembersId) ?? new List<string>(),
                Created = chat.Created
            }).ToList();
        }

        private async void OnChatCreatedHandler(ChatDto chatDto)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Получен чат от сервера: {chatDto.Name} (ID: {chatDto.Id})");
                
                var chat = new Chat
                {
                    Id = chatDto.Id,
                    Name = chatDto.Name,
                    Description = chatDto.Description,
                    MembersId = System.Text.Json.JsonSerializer.Serialize(chatDto.MembersId),
                    Created = chatDto.Created,
                    LastMessageTime = chatDto.Created,
                    LastMessageText = "",
                    UnreadCount = 0,
                    IsActive = true
                };
                
                await _chatRepository.SaveAsync(chat);
                System.Diagnostics.Debug.WriteLine("Чат сохранен в локальную БД");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения чата: {ex.Message}");
            }
        }

        private async void OnMessageReceivedHandler(MessageDto messageDto)
        {
            try
            {
                var message = new Message
                {
                    Id = messageDto.Id,
                    ChatId = messageDto.ChatId,
                    SenderId = messageDto.SenderId,
                    Text = messageDto.Text,
                    Created = messageDto.Created,
                    IsModified = messageDto.IsModified,
                    CorrelationId = messageDto.CorrelationId
                };

                await _messageRepository.AddMessageAsync(message);
                System.Diagnostics.Debug.WriteLine($"Сообщение сохранено: {message.Text}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения сообщения: {ex.Message}");
            }
        }

    }
}
