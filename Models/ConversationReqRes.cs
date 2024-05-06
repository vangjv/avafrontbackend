namespace AvaFront.API.Models
{
    public class ConversationRequest
    {
        public string Message { get; set; }
    }

    public class ConversationResponse
    {
        public ConversationResponse(string message, string conversationId, string action = null)
        {
            Message = message;
            ConversationId = conversationId;
            Action = action;
        }
        public string Message { get; set; }
        public string Action { get; set; }
        public string ConversationId { get; set; }
    }
}
