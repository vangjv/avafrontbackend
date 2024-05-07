
using AutoGen.OpenAI;
using AutoGen;
using AvaFront.AutoGen.Builders;
using AvaFront.AutoGen.SharedPrompts;
using AvaFront.AutoGen.Agents.AvaFront.PromptRepo;

namespace AvaFront.AutoGen.Agents.AvaFront
{
    public static class AvaFrontAgentBuilderExtensions
    {

        // Extension method to configure an CategorizerAgent
        public static AgentBuilder AvaFrontCategorizerAgent(this AgentBuilder builder)
        {
            var gpt4config = new AzureOpenAIConfig(
                endpoint: Environment.GetEnvironmentVariable("gpt4:AzureOpenAIEndpoint") ?? throw new Exception("Please set OPENAI_API_KEY environment variable."),
                deploymentName: Environment.GetEnvironmentVariable("gpt4:DeploymentName") ?? throw new Exception("Please set DeploymentName environment variable."),
                apiKey: Environment.GetEnvironmentVariable("gpt4:ApiKey") ?? throw new Exception("Please set ApiKey environment variable.")
            );
            builder.Agent = new AssistantAgent(
                name: "AvaFrontCategorizerAgent",
                systemMessage: new PromptBuilder().Add(AvaFrontPromptRepo.AvaFrontCategorizerAgentPrompt).Add(SharedPromptRepo.SafetySystemMessage).Build(),
                llmConfig: new ConversableAgentConfig
                {
                    Temperature = 0,
                    ConfigList = [gpt4config],
                });
            return builder;
        }
        
    }
}
