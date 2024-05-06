using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AvaFront.API.Models
{
    public class ChatConversation
    {
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ChatMessage> Messages { get; set; }

        public ChatConversation(string id)
        {
            Id = id;
            CreatedDate = DateTime.Now;
            Messages = new List<ChatMessage>();
        }

        public static ChatConversation? FromJsonString(string jsonString)
        {
            try
            {
                return JsonSerializer.Deserialize<ChatConversation>(jsonString);                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error parsing JSON: {e.Message}");
                return null;
            }
        }

        public void AddSystemMessage(string content)
        {
            Messages.Add(new ChatMessage(AIChatRole.System, content));
        }
        public void AddUserMessage(string content)
        {
            Messages.Add(new ChatMessage(AIChatRole.User, content));
        }

        public void AddAssistantMessage(string content)
        {
            Messages.Add(new ChatMessage(AIChatRole.Assistant, content));
        }
    }

    public class ChatMessage
    {
        [JsonPropertyName("r")]
        public AIChatRole Role { get; set; }
        [JsonPropertyName("c")]
        public string Content { get; set; }
        [JsonPropertyName("d")]
        public DateTime CreatedDate { get; set; }

        public ChatMessage(AIChatRole role, string content)
        {
            Role = role;
            Content = content;
            CreatedDate = DateTime.Now;
        }
    }

    public enum AIChatRole
    {
        System,
        User,        
        Assistant
    }
}
