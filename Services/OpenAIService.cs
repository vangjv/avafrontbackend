using AvaFront.API.Models;
using Azure;
using Azure.AI.OpenAI;
using System.Text.Json;

namespace AvaFront.API.Services
{
    public class OpenAIService
    {
        private AzureKeyCredential azureOpenAIApiKey;
        private OpenAIClient _openAIClient;
        private readonly RedisService _redisService;
        private string defaultSystemMessage = "";
        public OpenAIService(Uri azureOpenAIResourceUri, AzureKeyCredential azureOpenAIApiKey, string systemMessage = "You are a helpful assistant. You will talk like a pirate.")
        {
            defaultSystemMessage = systemMessage;
            _openAIClient = new OpenAIClient(azureOpenAIResourceUri, azureOpenAIApiKey);
            _redisService = new RedisService(Environment.GetEnvironmentVariable("RedisConnectionString"));
        }

        public async Task<string> GenerateTextAsync(string? conversationId, string prompt)
        {
            Response<ChatCompletions> response;
            ChatResponseMessage responseMessage;

            //get conversation from redis
            var conversation = await _redisService.GetCache(conversationId);

            //existing conversation exist
            if (conversation != null)
            {
                //parse response from redis
                var chatConversation = ChatConversation.FromJsonString(conversation);
                if (chatConversation != null)
                {
                    chatConversation.AddUserMessage(prompt);
                    response = await _openAIClient.GetChatCompletionsAsync(ChatCompletionOptionsFromChatConversation(chatConversation));
                    responseMessage = response.Value.Choices[0].Message;
                    Console.WriteLine($"[{responseMessage.Role.ToString().ToUpperInvariant()}]: {responseMessage.Content}");

                    //add response to redis conversation
                    try
                    {
                        chatConversation.AddAssistantMessage(responseMessage.Content);
                        string jsonConversation = JsonSerializer.Serialize(chatConversation);
                        await _redisService.SetCache(conversationId, jsonConversation);
                        return responseMessage.Content;
                    } catch (Exception e)
                    {
                        Console.WriteLine($"Error adding response to redis: {e.Message}");
                        return "";
                    }
                }
            }  

            //create new conversation
            //add system message to conversation
            var newChatConversation = new ChatConversation(conversationId);
            newChatConversation.AddSystemMessage(defaultSystemMessage);
            //add user prompt
            newChatConversation.AddUserMessage(prompt);
            //get response
            response = await _openAIClient.GetChatCompletionsAsync(ChatCompletionOptionsFromChatConversation(newChatConversation));
            responseMessage = response.Value.Choices[0].Message;
            Console.WriteLine($"[{responseMessage.Role.ToString().ToUpperInvariant()}]: {responseMessage.Content}");

            //add response to redis conversation
            try
            {
                newChatConversation.AddAssistantMessage(responseMessage.Content);
                string jsonConversation = JsonSerializer.Serialize(newChatConversation);
                await _redisService.SetCache(conversationId,jsonConversation);
                return responseMessage.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error adding response to redis: {e.Message}");
                return "";
            }                   
        }

        public static ChatCompletionsOptions ChatCompletionOptionsFromChatConversation(ChatConversation chatConversation)
        {
            IList<ChatRequestMessage> messages = new List<ChatRequestMessage>();
            foreach (var message in chatConversation.Messages)
            {
                if (message.Role == AIChatRole.System)
                {
                    messages.Add(new ChatRequestSystemMessage(message.Content));
                }
                else if (message.Role == AIChatRole.User)
                {
                    messages.Add(new ChatRequestUserMessage(message.Content));
                }
                else if (message.Role == AIChatRole.Assistant)
                {
                    messages.Add(new ChatRequestAssistantMessage(message.Content));
                }
            }
            var chatCompletionsOptions = new ChatCompletionsOptions("gpt-35-turbo", messages);            
            return chatCompletionsOptions;
        }   
    }
}
