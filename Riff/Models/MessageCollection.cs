namespace Riff.Models;

public class MessageCollection
{
    public string ChatId { get; set; } = string.Empty;
    public List<Message> Messages { get; set; } = new List<Message>();
}