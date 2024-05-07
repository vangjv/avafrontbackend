namespace AvaFront.AutoGen.Models
{
    public class ConversationHistoryContext
    {
        public ConversationHistory ConversationHistory { get; set; }
        public string LastResponse { get; set; }
        public ConversationHistoryContext (ConversationHistory conversationHistory, string lastResponse)
        {
            ConversationHistory = conversationHistory;
            LastResponse = lastResponse;
        }


    }
}
