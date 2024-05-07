
using AutoGen.OpenAI;
using AutoGen;
using AvaFront.AutoGen.Builders;
using AvaFront.AutoGen.Restaurant.PromptRepo;
using AvaFront.AutoGen.SharedPrompts;

namespace AvaFront.AutoGen.Agents.Restaurant
{
    public static class RestaurantAgentBuilderExtensions
    {
        // Extension method to configure an CategorizerAgent
        public static AgentBuilder CategorizerAgent(this AgentBuilder builder)
        {
            var gpt4config = new AzureOpenAIConfig(
                endpoint: Environment.GetEnvironmentVariable("gpt4:AzureOpenAIEndpoint") ?? throw new Exception("Please set OPENAI_API_KEY environment variable."),
                deploymentName: Environment.GetEnvironmentVariable("gpt4:DeploymentName") ?? throw new Exception("Please set DeploymentName environment variable."),
                apiKey: Environment.GetEnvironmentVariable("gpt4:ApiKey") ?? throw new Exception("Please set ApiKey environment variable.")
            );
            builder.Agent = new AssistantAgent(
                name: "CategorizerAgent",
                systemMessage: new PromptBuilder().Add(RestaurantPromptRepo.CategorizerAgentPrompt).Add(SharedPromptRepo.SafetySystemMessage).Build(),
                llmConfig: new ConversableAgentConfig
                {
                    Temperature = 0,
                    ConfigList = [gpt4config],
                });
            return builder;
        }

        // Extension method to configure an CatchAllAgent
        public static AgentBuilder CatchAllAgent(this AgentBuilder builder)
        {
            builder.Agent = new AssistantAgent(
                name: "CatchAllAgent",
                systemMessage: new PromptBuilder().Add(RestaurantPromptRepo.CatchAllAgentPrompt).Add(SharedPromptRepo.SafetySystemMessage).Build(),
                llmConfig: new ConversableAgentConfig
                {
                    Temperature = 0,
                    ConfigList = [builder.agentLLMConfig],
                });
            return builder;
        }

        // Extension method to configure a RestaurantRSVPAgent
        public static AgentBuilder RestaurantRSVPAgent(this AgentBuilder builder, string restaurantName)
        {
            builder.Agent = new AssistantAgent(
                name: "RestaurantRSVPAgent",
                systemMessage: new PromptBuilder().Add(RestaurantPromptRepo.RestaurantRSVPAgentPrompt(restaurantName)).Add(SharedPromptRepo.SafetySystemMessage).Build(),
                llmConfig: new ConversableAgentConfig
                {
                    Temperature = 0,
                    ConfigList = [builder.agentLLMConfig],
                });
            return builder;
        }

        // Extension method to configure a OrdersAgent
        public static AgentBuilder RestaurantOrdersAgent(this AgentBuilder builder, string restaurantName)
        {
            builder.Agent = new AssistantAgent(
                name: "OrdersAgent",
                systemMessage: new PromptBuilder().Add(RestaurantPromptRepo.RestaurantOrdersAgentPrompt(restaurantName)).Add(SharedPromptRepo.SafetySystemMessage).Build(),
                llmConfig: new ConversableAgentConfig
                {
                    Temperature = 0,
                    ConfigList = [builder.agentLLMConfig],
                });
            return builder;
        }
    }
}
