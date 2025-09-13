using Riff.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.Repositories.Interfaces
{
    public interface IMessageCollectionRepository
    {
        Task<MessageCollection?> GetByChatIdAsync(string chatId);

        Task<bool> SaveAsync(MessageCollection messageCollection);

        Task<bool> AddMessageAsync(Message message);

        Task<bool> UpdateMessageAsync(Message message);

        Task<bool> DeleteMessageAsync(string messageId);

        Task<List<Message>> GetMessagesByChatIdAsync(string chatId);

        Task<bool> DeleteMessagesByChatIdAsync(string chatId);

        Task<int> GetMessageCountAsync(string chatId);
    }
}
