using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Models
{
    public partial class ChatInfo : ObservableObject
    {
        [ObservableProperty]
        private string _lastMessage = "";

        [ObservableProperty]
        private string _chatName = "";

        [ObservableProperty]
        private string _chatId = "";

        [ObservableProperty]
        private string _description = "";

        [ObservableProperty]
        private DateTime _lastMessageTime = DateTime.UtcNow;

        [ObservableProperty]
        private int _unreadCount = 0;

        [ObservableProperty]
        private bool _isActive = true;

        [RelayCommand]
        public async Task ShowChatScreen()
        {
            Preferences.Set("LAST_CLICKED_CHAT_ID", ChatId);
            Preferences.Set("LAST_CLICKED_CHAT_NAME", ChatName);
            await Shell.Current.GoToAsync(nameof(ChatPage));
        }

        [RelayCommand]
        public async Task DeleteChat()
        {
            await Task.CompletedTask;
        }
    }
}
