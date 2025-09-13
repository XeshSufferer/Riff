using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Riff.Models;
using Riff.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Riff.ViewModels
{
    public partial class ChatsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<ChatInfo> _chats = new ObservableCollection<ChatInfo>();

        [ObservableProperty]
        public string _usernameForChat = "";

        [ObservableProperty]
        public bool _isLoading = false;

        private readonly IChatsService _chatsService;

        public ChatsPageViewModel(IChatsService chats)
        {
            _chatsService = chats;
            
            _chatsService.OnChatCreated += OnChatCreated;
            _chatsService.OnMessageReceived += OnMessageReceived;
            
            _ = Task.Run(LoadChatsAsync);
        }

        [RelayCommand]
        public async Task CreateChat()
        {
            if (string.IsNullOrWhiteSpace(UsernameForChat))
                return;

            IsLoading = true;
            try
            {
                await _chatsService.CreateChat(UsernameForChat);
                UsernameForChat = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания чата: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task LoadChats()
        {
            await LoadChatsAsync();
        }



        private async Task LoadChatsAsync()
        {
            IsLoading = true;
            try
            {
                var chats = await _chatsService.GetLocalChatsAsync();
                
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Chats.Clear();
                    foreach (var chat in chats)
                    {
                        Chats.Add(ConvertToChatInfo(chat));
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки чатов: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void OnChatCreated(ChatDto chat)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Chats.Add(ConvertToChatInfo(chat));
            });
        }

        private void OnMessageReceived(MessageDto message)
        {
        }

        private ChatInfo ConvertToChatInfo(ChatDto chat)
        {
            return new ChatInfo
            {
                ChatName = chat.Name,
                ChatId = chat.Id,
                LastMessage = "",
                LastMessageTime = chat.Created,
                UnreadCount = 0,
                Description = chat.Description ?? ""
            };
        }

        public void Dispose()
        {
            _chatsService.OnChatCreated -= OnChatCreated;
            _chatsService.OnMessageReceived -= OnMessageReceived;
        }
    }
}
