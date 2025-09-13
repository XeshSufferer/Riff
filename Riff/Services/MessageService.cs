using Riff.Models;
using Riff.Repositories.Interfaces;
using Riff.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageCollectionRepository _messageRepository;

        public MessageService(IMessageCollectionRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<List<Message>> GetChatMessagesAsync(string chatId)
        {
            return await _messageRepository.GetMessagesByChatIdAsync(chatId);
        }

        public async Task<MessageCollection?> GetMessageCollectionAsync(string chatId)
        {
            return await _messageRepository.GetByChatIdAsync(chatId);
        }

        public async Task<bool> SaveMessageAsync(Message message)
        {
            return await _messageRepository.AddMessageAsync(message);
        }

        public async Task<bool> UpdateMessageAsync(Message message)
        {
            return await _messageRepository.UpdateMessageAsync(message);
        }

        public async Task<bool> DeleteMessageAsync(string messageId)
        {
            return await _messageRepository.DeleteMessageAsync(messageId);
        }

        public async Task<bool> SaveMessageCollectionAsync(MessageCollection messageCollection)
        {
            return await _messageRepository.SaveAsync(messageCollection);
        }

        public async Task<int> GetMessageCountAsync(string chatId)
        {
            return await _messageRepository.GetMessageCountAsync(chatId);
        }

        public async Task<bool> ClearChatMessagesAsync(string chatId)
        {
            return await _messageRepository.DeleteMessagesByChatIdAsync(chatId);
        }
    }
}
