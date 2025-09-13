using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;
using SQLite;

namespace Riff.Models;

public partial class Message : ObservableObject
{
    [PrimaryKey]
    public string Id { get; set; } = "";
    
    [ObservableProperty] private string _chatId = "";
    [ObservableProperty] private string _senderId = "";
    [ObservableProperty] private string _text = "";
    [ObservableProperty] private DateTime _created = DateTime.UtcNow;
    [ObservableProperty] private bool _isModified = false;
    [ObservableProperty] private string _correlationId = "";
}



public class MessageDto
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("chat_id")] public string ChatId { get; set; }
    [JsonPropertyName("sender_id")] public string SenderId { get; set; }
    [JsonPropertyName("text")] public string Text { get; set; }
    [JsonPropertyName("created")] public DateTime Created { get; set; }
    [JsonPropertyName("is_modified")] public bool IsModified { get; set; }
    [JsonPropertyName("correlation_id")] public string CorrelationId { get; set; }
}