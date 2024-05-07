using AutoGen.Core;
using AvaFront.AutoGen.Builders;
using AvaFront.AutoGen.Models;
using System.Text.Json;
using AvaFront.AutoGen.Restaurant.PromptRepo;
using AvaFront.AutoGen.Agents.AvaFront.PromptRepo;

namespace AvaFront.AutoGen.Agents.AvaFront
{
    public class AvaFrontAgent
    {
        public AgentBuilder agentBuilder { get; set; }
        public AvaFrontAgent()
        {
            agentBuilder = new AgentBuilder();
        }

        public async Task<ConversationHistoryContext> ExecuteConversationAsync(string message, ConversationHistory conversationHistory)
        {
            if ( conversationHistory == null)
            {
                conversationHistory = new ConversationHistory();
            }
            //categorize the user input
            var categorizeAgent = agentBuilder.AvaFrontCategorizerAgent().Build();
            TextMessage chatMessage = new TextMessage(Role.User, message, "User");
            List<TextMessage> userMessageHistory = conversationHistory.GetHistoricalUserMessages();
            userMessageHistory.Add(chatMessage);
            ExtendedMessage userMessage = new ExtendedMessage(Role.User, message, "User");
            var categoryResponse = await categorizeAgent.GenerateReplyAsync(userMessageHistory);
            conversationHistory.AddToConversationHistory(categorizeAgent, userMessage, new ExtendedMessage(Role.Assistant, categoryResponse.GetContent(), categorizeAgent.Name));

            string lastResponse = "";
            //route the user input to the appropriate agent
            switch (categoryResponse.GetContent())
            {
                case "SALES":
                    var salesAgent = agentBuilder.SystemAgent().Build();
                    ExtendedMessage salesMessage = new ExtendedMessage(Role.System, "NAVIGATETOSALES", "System");
                    conversationHistory.AddToConversationHistory(salesAgent, userMessage, salesMessage);
                    lastResponse = "NAVIGATETOSALES";
                    break;
                case "RESTAURANT":
                    var restaurantAgent = agentBuilder.SystemAgent().Build();
                    ExtendedMessage restaurantMessage = new ExtendedMessage(Role.System, "NAVIGATETORESTAURANT", "System");
                    conversationHistory.AddToConversationHistory(restaurantAgent, userMessage, restaurantMessage);
                    lastResponse = "NAVIGATETORESTAURANT";
                    break;
                case "AUTHENTICATION":
                    var authenticationAgent = agentBuilder.SystemAgent().Build();
                    ExtendedMessage authenticationMessage = new ExtendedMessage(Role.System, "NAVIGATETOAUTHENTICATION", "System");
                    conversationHistory.AddToConversationHistory(authenticationAgent, userMessage, authenticationMessage);
                    lastResponse = "NAVIGATETOAUTHENTICATION";
                    break;
                default:
                    var informationAgent = agentBuilder.InformationAgent(AvaFrontPromptRepo.AvaFrontDeskPersona, RAGData.AvaFront.Data).Build();
                    //inject message history
                    var informationAgentUserMessageHistory = conversationHistory.GetAgentUserMessageHistory(informationAgent.Name);
                    //add most recent user message
                    informationAgentUserMessageHistory.Add(userMessage);
                    //continue conversation
                    var informationAgentChat = await informationAgent.GenerateReplyAsync(informationAgentUserMessageHistory);
                    lastResponse = informationAgentChat.GetContent();
                    //save conversation history
                    conversationHistory.AddToConversationHistory(informationAgent, userMessage, new ExtendedMessage(Role.Assistant, informationAgentChat.GetContent(), informationAgent.Name));
                    break;
            }
            ConversationHistoryContext conversationHistoryContext = new ConversationHistoryContext(conversationHistory, lastResponse);
            return conversationHistoryContext;
        }
        
        //for local testing
        public async Task<ConversationHistory> LoadConversationHistory(string jsonFilePath)
        {
            try
            {
                string jsonString = await File.ReadAllTextAsync(jsonFilePath);
                var conversationHistory = JsonSerializer.Deserialize<ConversationHistory>(jsonString);
                //build application model
                var applicationConversationHistory = new ConversationHistory();
                //loop through loaded data and build application model
                conversationHistory.AgentsWithConversations.ForEach(agent =>
                {
                    var agentWithConversation = new AgentWithConversation();
                    agentWithConversation.AgentName = agent.AgentName;
                    var messages = new List<ExtendedMessage>();
                    agent.Messages.ForEach(message =>
                    {
                        if (message.RoleValue == "user")
                        {
                            messages.Add(new ExtendedMessage(Role.User, message.Content, message.From));
                        }
                        else if (message.RoleValue == "assistant")
                        {
                            messages.Add(new ExtendedMessage(Role.Assistant, message.Content, message.From));
                        }
                        else if (message.RoleValue == "system")
                        {
                            messages.Add(new ExtendedMessage(Role.System, message.Content, message.From));
                        }
                        else if (message.RoleValue == "function")
                        {
                            messages.Add(new ExtendedMessage(Role.Function, message.Content, message.From));
                        }                        
                    });
                    agentWithConversation.Messages = messages;
                    applicationConversationHistory.AgentsWithConversations.Add(agentWithConversation);                
                });
                return applicationConversationHistory;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        //for local testing
        public async Task SaveConversationHistoryToJson(ConversationHistory conversationHistory, string filePath)
        {
            try
            {
                //add role values to each message (struct doesn't serialize fields)
                conversationHistory.AgentsWithConversations.ForEach(agent =>
                {
                    agent.Messages.ForEach(msg =>
                    {
                        msg.RoleValue = msg.Role.ToString().ToLower();
                    });
                });
                JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
                {
                    IncludeFields = true
                };
                var jsonString = JsonSerializer.Serialize(conversationHistory, jsonSerializerOptions);               

                await File.WriteAllTextAsync(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }          
    }
}
