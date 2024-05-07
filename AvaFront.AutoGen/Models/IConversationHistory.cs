using AutoGen;
using AutoGen.Core;
using AvaFront.Shared.Entities.Base;

namespace AvaFront.AutoGen.Models
{
    public interface IConversationHistory
    {
        public List<AgentWithConversation> AgentsWithConversations { get; set; }
        public DateTime DateCreated { get; set; }        
        public string LearnedUserSpecificData { get; set; } //Used for injection at the top of each agent's conversation

        public void AddToConversationHistory(ConversableAgent agent, ExtendedMessage userMessage, ExtendedMessage agentResponse);
        public List<IMessage> GetAgentUserMessageHistory(string agentName);
    }
}
