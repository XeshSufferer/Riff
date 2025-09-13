using Riff.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<Message>> GetChatMessagesAsync(string chatId);

        Task<MessageCollection?> GetMessageCollectionAsync(string chatId);

        Task<bool> SaveMessageAsync(Message message);

        Task<bool> UpdateMessageAsync(Message message);

        Task<bool> DeleteMessageAsync(string messageId);

        Task<bool> SaveMessageCollectionAsync(MessageCollection messageCollection);

        Task<int> GetMessageCountAsync(string chatId);

        Task<bool> ClearChatMessagesAsync(string chatId);
    }
}
