using Riff.Models;
using Riff.Repositories.Database;
using Riff.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Riff.Repositories
{
    public class MessageCollectionRepository : IMessageCollectionRepository
    {
        private readonly SqliteContext _context;

        public MessageCollectionRepository(SqliteContext context)
        {
            _context = context;
        }

        public async Task<MessageCollection?> GetByChatIdAsync(string chatId)
        {
            try
            {
                var messages = await _context.Database.Table<Message>()
                    .Where(m => m.ChatId == chatId)
                    .OrderBy(m => m.Created)
                    .ToListAsync();

                if (messages == null || !messages.Any())
                {
                    return null;
                }

                return new MessageCollection
                {
                    ChatId = chatId,
                    Messages = messages
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения коллекции сообщений для чата {chatId}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveAsync(MessageCollection messageCollection)
        {
            try
            {
                if (messageCollection?.Messages == null || !messageCollection.Messages.Any())
                {
                    return false;
                }

                await DeleteMessagesByChatIdAsync(messageCollection.ChatId);

                foreach (var message in messageCollection.Messages)
                {
                    await _context.Database.InsertAsync(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения коллекции сообщений для чата {messageCollection?.ChatId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddMessageAsync(Message message)
        {
            try
            {
                if (message == null || string.IsNullOrEmpty(message.Id))
                {
                    return false;
                }

                await _context.Database.InsertAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка добавления сообщения {message?.Id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateMessageAsync(Message message)
        {
            try
            {
                if (message == null || string.IsNullOrEmpty(message.Id))
                {
                    return false;
                }

                var rowsAffected = await _context.Database.UpdateAsync(message);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления сообщения {message?.Id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteMessageAsync(string messageId)
        {
            try
            {
                if (string.IsNullOrEmpty(messageId))
                {
                    return false;
                }

                var rowsAffected = await _context.Database.DeleteAsync<Message>(messageId);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления сообщения {messageId}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Message>> GetMessagesByChatIdAsync(string chatId)
        {
            try
            {
                if (string.IsNullOrEmpty(chatId))
                {
                    return new List<Message>();
                }

                var messages = await _context.Database.Table<Message>()
                    .Where(m => m.ChatId == chatId)
                    .OrderBy(m => m.Created)
                    .ToListAsync();

                return messages ?? new List<Message>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения сообщений для чата {chatId}: {ex.Message}");
                return new List<Message>();
            }
        }

        public async Task<bool> DeleteMessagesByChatIdAsync(string chatId)
        {
            try
            {
                if (string.IsNullOrEmpty(chatId))
                {
                    return false;
                }

                var rowsAffected = await _context.Database.Table<Message>()
                    .Where(m => m.ChatId == chatId)
                    .DeleteAsync();

                return rowsAffected >= 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления сообщений для чата {chatId}: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetMessageCountAsync(string chatId)
        {
            try
            {
                if (string.IsNullOrEmpty(chatId))
                {
                    return 0;
                }

                var count = await _context.Database.Table<Message>()
                    .Where(m => m.ChatId == chatId)
                    .CountAsync();

                return count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка подсчета сообщений для чата {chatId}: {ex.Message}");
                return 0;
            }
        }
    }
}
