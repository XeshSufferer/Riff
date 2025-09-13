using Riff.Models;
using Riff.Repositories.Database;
using Riff.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Riff.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly SqliteContext _context;

        public ChatRepository(SqliteContext context)
        {
            _context = context;
        }

        public async Task<Chat?> GetByIdAsync(string chatId)
        {
            try
            {
                if (string.IsNullOrEmpty(chatId))
                {
                    return null;
                }

                await _context.Database.CreateTableAsync<Chat>();

                var chat = await _context.Database.Table<Chat>()
                    .FirstOrDefaultAsync(c => c.Id == chatId);

                return chat;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения чата {chatId}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Chat>> GetAllActiveAsync()
        {
            try
            {
                await _context.Database.CreateTableAsync<Chat>();

                var chats = await _context.Database.Table<Chat>()
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.LastMessageTime)
                    .ToListAsync();

                return chats ?? new List<Chat>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения активных чатов: {ex.Message}");
                return new List<Chat>();
            }
        }

        public async Task<List<Chat>> GetByUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new List<Chat>();
                }

                var chats = await _context.Database.Table<Chat>()
                    .Where(c => c.IsActive && c.MembersId.Contains(userId))
                    .OrderByDescending(c => c.LastMessageTime)
                    .ToListAsync();

                return chats ?? new List<Chat>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения чатов пользователя {userId}: {ex.Message}");
                return new List<Chat>();
            }
        }

        public async Task<bool> SaveAsync(Chat chat)
        {
            try
            {
                if (chat == null || string.IsNullOrEmpty(chat.Id))
                {
                    return false;
                }

                var existingChat = await GetByIdAsync(chat.Id);
                
                if (existingChat != null)
                {
                    var rowsAffected = await _context.Database.UpdateAsync(chat);
                    return rowsAffected > 0;
                }
                else
                {
                    await _context.Database.InsertAsync(chat);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения чата {chat?.Id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Chat chat)
        {
            try
            {
                if (chat == null || string.IsNullOrEmpty(chat.Id))
                {
                    return false;
                }

                var rowsAffected = await _context.Database.UpdateAsync(chat);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления чата {chat?.Id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string chatId)
        {
            try
            {
                if (string.IsNullOrEmpty(chatId))
                {
                    return false;
                }

                var rowsAffected = await _context.Database.DeleteAsync<Chat>(chatId);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления чата {chatId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeactivateAsync(string chatId)
        {
            try
            {
                if (string.IsNullOrEmpty(chatId))
                {
                    return false;
                }

                var chat = await GetByIdAsync(chatId);
                if (chat == null)
                {
                    return false;
                }

                chat.IsActive = false;
                return await UpdateAsync(chat);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка деактивации чата {chatId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateLastMessageAsync(string chatId, string messageText, DateTime messageTime)
        {
            try
            {
                if (string.IsNullOrEmpty(chatId))
                {
                    return false;
                }

                var chat = await GetByIdAsync(chatId);
                if (chat == null)
                {
                    return false;
                }

                chat.LastMessageText = messageText ?? "";
                chat.LastMessageTime = messageTime;

                return await UpdateAsync(chat);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления последнего сообщения для чата {chatId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUnreadCountAsync(string chatId, int unreadCount)
        {
            try
            {
                if (string.IsNullOrEmpty(chatId))
                {
                    return false;
                }

                var chat = await GetByIdAsync(chatId);
                if (chat == null)
                {
                    return false;
                }

                chat.UnreadCount = Math.Max(0, unreadCount);
                return await UpdateAsync(chat);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления количества непрочитанных для чата {chatId}: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetCountAsync()
        {
            try
            {
                var count = await _context.Database.Table<Chat>()
                    .Where(c => c.IsActive)
                    .CountAsync();

                return count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка подсчета чатов: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<Chat>> SearchByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return await GetAllActiveAsync();
                }

                var chats = await _context.Database.Table<Chat>()
                    .Where(c => c.IsActive && c.Name.Contains(searchTerm))
                    .OrderByDescending(c => c.LastMessageTime)
                    .ToListAsync();

                return chats ?? new List<Chat>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка поиска чатов по запросу '{searchTerm}': {ex.Message}");
                return new List<Chat>();
            }
        }

        public static string SerializeMembersId(List<string> membersId)
        {
            try
            {
                return JsonSerializer.Serialize(membersId ?? new List<string>());
            }
            catch
            {
                return "[]";
            }
        }

        public static List<string> DeserializeMembersId(string membersIdJson)
        {
            try
            {
                if (string.IsNullOrEmpty(membersIdJson))
                    return new List<string>();
                
                return JsonSerializer.Deserialize<List<string>>(membersIdJson) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
