using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Riff.Models;
using Riff.Services.Interfaces;
using Riff.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.ViewModels
{
    public partial class ChatPageViewModel : ObservableObject, IDisposable
    {
        private string CurrentUserId => Preferences.Get("USERID", "");

        [ObservableProperty]
        public ObservableCollection<Message> _messages = new ObservableCollection<Message>();

        [ObservableProperty]
        private string _message = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _currentChatName = "Чат";

        private readonly IChatsService _chats;
        private readonly IMessageService _messageService;
        private string _currentChatId = string.Empty;

        public ChatPageViewModel(IChatsService chats, IMessageService messageService)
        {
            _chats = chats;
            _messageService = messageService;
        }

        public async void Sub()
        {
            var chatId = Preferences.Get("LAST_CLICKED_CHAT_ID", "");
            var chatName = Preferences.Get("LAST_CLICKED_CHAT_NAME", "Чат");
            
            UpdateChatName(chatName);
            
            if (!string.IsNullOrEmpty(chatId))
            {
                await LoadMessagesAsync(chatId);
            }
            else
            {
                Messages.Clear();
            }

            _chats.OnMessageReceived += AddMessage;
        }

        public void Dispose()
        {
            _chats.OnMessageReceived -= AddMessage;
        }

        private async void AddMessage(MessageDto message)
        {
            try
            {
                if (message.SenderId == CurrentUserId)
                {
                    var existingMessage = Messages.FirstOrDefault(m => m.CorrelationId == message.CorrelationId);
                    if (existingMessage != null)
                    {
                        existingMessage.Id = message.Id;
                        await _messageService.UpdateMessageAsync(existingMessage);
                    }
                    return;
                }

                var newMessage = new Message
                {
                    SenderId = message.SenderId,
                    ChatId = message.ChatId,
                    Text = message.Text,
                    Created = message.Created,
                    Id = message.Id,
                    IsModified = message.IsModified,
                    CorrelationId = message.CorrelationId
                };

                await _messageService.SaveMessageAsync(newMessage);
                
                if (message.ChatId == _currentChatId)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Messages.Add(newMessage);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обработки входящего сообщения: {ex.Message}");
            }
        }


        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(Message))
                return;

            try
            {
                var localMessage = new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    ChatId = _currentChatId,
                    SenderId = CurrentUserId, 
                    Text = Message,
                    Created = DateTime.UtcNow,
                    IsModified = false,
                    CorrelationId = Guid.NewGuid().ToString()
                };

                Messages.Add(localMessage);

                await _messageService.SaveMessageAsync(localMessage);

                await _chats.SendMessage(Message);

                Message = string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
                
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Messages.Add(new Message
                    {
                        Id = "error_send",
                        Text = "Ошибка отправки сообщения",
                        Created = DateTime.UtcNow,
                        ChatId = _currentChatId
                    });
                });
            }
        }

        public async Task LoadMessagesAsync(string chatId)
        {
            if (string.IsNullOrEmpty(chatId))
            {
                Messages.Clear();
                return;
            }

            _currentChatId = chatId;
            IsLoading = true;
            
            try
            {
                var messages = await _messageService.GetChatMessagesAsync(chatId);
                
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Messages.Clear();
                    foreach (var message in messages)
                    {
                        Messages.Add(message);
                    }
                });

                System.Diagnostics.Debug.WriteLine($"Загружено {messages.Count} сообщений для чата {chatId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки сообщений для чата {chatId}: {ex.Message}");
                
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Messages.Clear();
                    Messages.Add(new Message
                    {
                        Id = "error",
                        Text = "Ошибка загрузки сообщений",
                        Created = DateTime.UtcNow,
                        ChatId = chatId
                    });
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task ClearMessagesAsync()
        {
            if (!string.IsNullOrEmpty(_currentChatId))
            {
                try
                {
                    await _messageService.ClearChatMessagesAsync(_currentChatId);
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Messages.Clear();
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка очистки сообщений: {ex.Message}");
                }
            }
        }

        public void UpdateChatName(string chatName)
        {
            CurrentChatName = chatName ?? "Чат";
        }

        public async Task<int> GetMessageCountAsync()
        {
            if (string.IsNullOrEmpty(_currentChatId))
                return 0;

            try
            {
                return await _messageService.GetMessageCountAsync(_currentChatId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения количества сообщений: {ex.Message}");
                return 0;
            }
        }
    }
}
