using SQLite;
using System.Text.Json.Serialization;

namespace Riff.Models;

[Table("Chats")]
public class Chat
{
    [PrimaryKey]
    public string Id { get; set; } = "";
    
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string MembersId { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastMessageTime { get; set; } = DateTime.UtcNow;
    public string LastMessageText { get; set; } = "";
    public int UnreadCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class ChatDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("members_id")] public List<string> MembersId { get; set; } = new();
    [JsonPropertyName("created")] public DateTime Created { get; set; } = DateTime.UtcNow;
}