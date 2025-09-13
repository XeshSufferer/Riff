using Riff.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.Repositories.Interfaces
{
    public interface IChatRepository
    {
        Task<Chat?> GetByIdAsync(string chatId);

        Task<List<Chat>> GetAllActiveAsync();

        Task<List<Chat>> GetByUserIdAsync(string userId);

        Task<bool> SaveAsync(Chat chat);

        Task<bool> UpdateAsync(Chat chat);

        Task<bool> DeleteAsync(string chatId);

        Task<bool> DeactivateAsync(string chatId);

        Task<bool> UpdateLastMessageAsync(string chatId, string messageText, DateTime messageTime);

        Task<bool> UpdateUnreadCountAsync(string chatId, int unreadCount);

        Task<int> GetCountAsync();

        Task<List<Chat>> SearchByNameAsync(string searchTerm);
    }
}
