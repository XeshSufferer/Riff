using SQLite;
using Riff.Models;
using System.IO;

namespace Riff.Repositories.Database
{
    public class SqliteContext
    {
        private readonly SQLiteAsyncConnection _database;

        public SqliteContext()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "riff_messages.db");
            _database = new SQLiteAsyncConnection(dbPath);
            
            _ = Task.Run(async () => await InitializeDatabaseAsync());
        }

        public SQLiteAsyncConnection Database => _database;

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Начинаем инициализацию базы данных...");
                
                var messageResult = await _database.CreateTableAsync<Message>();
                System.Diagnostics.Debug.WriteLine($"Таблица Message создана: {messageResult}");
                
                var chatResult = await _database.CreateTableAsync<Chat>();
                System.Diagnostics.Debug.WriteLine($"Таблица Chat создана: {chatResult}");
                
                await _database.CreateIndexAsync("IX_Message_ChatId", "Message", "ChatId");
                await _database.CreateIndexAsync("IX_Message_Created", "Message", "Created");
                await _database.CreateIndexAsync("IX_Message_SenderId", "Message", "SenderId");
                
                await _database.CreateIndexAsync("IX_Chat_IsActive", "Chat", "IsActive");
                await _database.CreateIndexAsync("IX_Chat_LastMessageTime", "Chat", "LastMessageTime");
                await _database.CreateIndexAsync("IX_Chat_MembersId", "Chat", "MembersId");
                
                System.Diagnostics.Debug.WriteLine("База данных успешно инициализирована!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка инициализации базы данных: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        public async Task ForceInitializeAsync()
        {
            await InitializeDatabaseAsync();
        }

        public async Task CloseAsync()
        {
            if (_database != null)
            {
                await _database.CloseAsync();
            }
        }
    }
}
