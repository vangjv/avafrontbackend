using AutoGen.Core;
using AvaFront.AutoGen.Builders;
using AvaFront.AutoGen.Models;
using AvaFront.AutoGen.SharedPrompts;
using AvaFront.AutoGen.Agents.Restaurant.RAGData;
using System.Text.Json;
using AvaFront.AutoGen.Restaurant.PromptRepo;

namespace AvaFront.AutoGen.Agents.Restaurant
{
    public class RestaurantAgent
    {
        public AgentBuilder agentBuilder { get; set; }
        public RestaurantAgent()
        {
            agentBuilder = new AgentBuilder();
        }

        public async Task<ConversationHistoryContext> ExecuteConversationAsync(string message, ConversationHistory conversationHistory)
        {
            //load conversation history (for local testing)
            //var conversationHistory = await LoadConversationHistory(Directory.GetCurrentDirectory() + "\\conversationHistory.json");
            if ( conversationHistory == null)
            {
                conversationHistory = new ConversationHistory();
            }
            //categorize the user input
            var categorizeAgent = agentBuilder.CategorizerAgent().Build();
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
                case "AGENT INFO":
                    var informationAgent = agentBuilder.InformationAgent(RestaurantPromptRepo.RestaurantFrontDeskPersona, SpiceRoad.Data).Build();
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
                case "AGENT ORDERS":
                    var ordersAgent = agentBuilder.RestaurantOrdersAgent("SpiceRoad").Build();
                    //inject message history
                    var ordersAgentUserMessageHistory = conversationHistory.GetAgentUserMessageHistory(ordersAgent.Name);
                    //add most recent user message
                    ordersAgentUserMessageHistory.Add(userMessage);
                    var ordersAgentChat = await ordersAgent.GenerateReplyAsync(ordersAgentUserMessageHistory);
                    lastResponse = ordersAgentChat.GetContent();
                    //save conversation history
                    conversationHistory.AddToConversationHistory(ordersAgent, userMessage, new ExtendedMessage(Role.Assistant, ordersAgentChat.GetContent(), ordersAgent.Name));
                    break;
                case "AGENT RSVP":
                    var rsvpAgent = agentBuilder.RestaurantRSVPAgent("SpiceRoad").Build();
                    //inject message history
                    var rsvpAgentUserMessageHistory = conversationHistory.GetAgentUserMessageHistory(rsvpAgent.Name);
                    //add most recent user message
                    rsvpAgentUserMessageHistory.Add(userMessage);
                    //continue conversation
                    var rsvpAgentChat = await rsvpAgent.GenerateReplyAsync(rsvpAgentUserMessageHistory);
                    lastResponse = rsvpAgentChat.GetContent();
                    //save conversation history
                    conversationHistory.AddToConversationHistory(rsvpAgent, userMessage, new ExtendedMessage(Role.Assistant, rsvpAgentChat.GetContent(), rsvpAgent.Name));
                    break;
                default:
                    var catchAllAgent = agentBuilder.CatchAllAgent().Build();
                    //inject message history
                    var catchAllAgentUserMessageHistory = conversationHistory.GetAgentUserMessageHistory(catchAllAgent.Name);
                    //add most recent user message
                    catchAllAgentUserMessageHistory.Add(userMessage);
                    //continue conversation
                    var catchAllAgentChat = await catchAllAgent.GenerateReplyAsync(catchAllAgentUserMessageHistory);
                    lastResponse = catchAllAgentChat.GetContent();
                    conversationHistory.AddToConversationHistory(catchAllAgent, userMessage, new ExtendedMessage(Role.Assistant, catchAllAgentChat.GetContent(), catchAllAgent.Name));
                    //save conversation history
                    break;
            }
            //save conversation history (for local testing)
            //await SaveConversationHistoryToJson(conversationHistory, Directory.GetCurrentDirectory() + "\\conversationHistory.json");    
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
