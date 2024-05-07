using AutoGen;
using AutoGen.Core;
using AvaFront.Shared.Entities.Base;

namespace AvaFront.AutoGen.Models
{
    public class ConversationHistory : BaseEntity, IConversationHistory 
    {
        public List<AgentWithConversation> AgentsWithConversations { get; set; } = new List<AgentWithConversation>();
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string LearnedUserSpecificData { get; set; } //Used for injection at the top of each agent's conversation
        public int SchemaVersion { get; set; }

        public void AddToConversationHistory(ConversableAgent agent, ExtendedMessage userMessage, ExtendedMessage agentResponse)
        {
            //look if an agent that matches the agent is already in the conversation history
            //add agent/user conversation to agent conversation history
            AgentWithConversation agentWithConversation;
            try
            {
                agentWithConversation = AgentsWithConversations.First(x => x.AgentName == agent.Name);
                //add messages
                agentWithConversation.Messages.Add(userMessage);
                agentWithConversation.Messages.Add(agentResponse);
            }
            catch (Exception ex)
            {
                agentWithConversation = new AgentWithConversation();
                agentWithConversation.AgentName = agent.Name;
                agentWithConversation.Messages = [userMessage, agentResponse];
                AgentsWithConversations.Add(agentWithConversation);
            }
            //add user/agent conversation to user conversation history
            AgentWithConversation userWithConversation;
            try
            {
                userWithConversation = AgentsWithConversations.First(x => x.AgentName == "User");
                //add messages
                userWithConversation.Messages.Add(userMessage);
                userWithConversation.Messages.Add(agentResponse);
            }
            catch (Exception ex)
            {
                userWithConversation = new AgentWithConversation();
                userWithConversation.AgentName = "User";
                userWithConversation.Messages = [userMessage, agentResponse];
                AgentsWithConversations.Add(userWithConversation);
            }
        }

        public List<IMessage> GetAgentUserMessageHistory(string agentName)
        {
            List<IMessage> agentUserMessageHistory = new List<IMessage>();
            if (AgentsWithConversations != null)
            {
                foreach (var agentWithConversation in AgentsWithConversations)
                {
                    if (agentWithConversation.AgentName == agentName)
                    {
                        //get all messages where the agent interacted with the user (the message prior to the agent message is a user message)
                        for (int i = 0; i < agentWithConversation.Messages.Count; i++)
                        {
                            if (i >= 1 && agentWithConversation.Messages[i].From == agentName)
                            {
                                agentUserMessageHistory.Add(agentWithConversation.Messages[i - 1]);
                                agentUserMessageHistory.Add(agentWithConversation.Messages[i]);
                            }
                        }
                    }
                }
            }
            return agentUserMessageHistory;
        }

        public List<TextMessage> GetHistoricalUserMessages()
        {
            //get AgentWithConversation for user
            List<TextMessage> textMessages = new List<TextMessage>();
            var userWithConversation = AgentsWithConversations.FirstOrDefault(x => x.AgentName == "User");
            if (userWithConversation != null)
            {
                var uniqueUserConversation = RemoveDuplicates(userWithConversation.Messages);
                foreach (var msg in uniqueUserConversation)
                {
                    //don't include messages from CategorizerAgent
                    if (msg.From != "CategorizerAgent")
                    {
                        textMessages.Add(msg as TextMessage);
                    }                    
                }
            }
            //remove duplicates by object comparison        
            return textMessages.Distinct().ToList(); 
        }

        public List<ExtendedMessage> RemoveDuplicates(List<ExtendedMessage> messages)
        {
            var uniqueMap = new Dictionary<string, ExtendedMessage>();

            foreach (var message in messages)
            {
                var uniqueKey = $"{message.From}_{message.DateCreated}";

                if (!uniqueMap.ContainsKey(uniqueKey))
                {
                    uniqueMap[uniqueKey] = message;
                }
            }

            return uniqueMap.Values.ToList();
        }

        //transform for database
        public void TransformForDatabase()
        {
            try
            {
                AgentsWithConversations.ForEach(agent =>
                {
                    agent.Messages.ForEach(msg =>
                    {
                        msg.RoleValue = msg.Role.ToString().ToLower();
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void TransformForApplication()
        {
            //build application model
            var applicationConversationHistory = new ConversationHistory();
            //loop through loaded data and build application model
            AgentsWithConversations.ForEach(agent =>
            {
                var agentWithConversation = new AgentWithConversation();
                agentWithConversation.AgentName = agent.AgentName;
                var messages = new List<ExtendedMessage>();
                agent.Messages.ForEach(message =>
                {
                    if (message.RoleValue == "user")
                    {
                        message.Role = Role.User;
                    }
                    else if (message.RoleValue == "assistant")
                    {
                        message.Role = Role.Assistant;
                    }
                    else if (message.RoleValue == "system")
                    {
                        message.Role = Role.System;
                    }
                    else if (message.RoleValue == "function")
                    {
                        message.Role = Role.Function;
                    }
                });
            });
        }
    }
}
