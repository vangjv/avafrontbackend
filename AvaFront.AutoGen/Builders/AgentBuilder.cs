using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;
using AvaFront.AutoGen.SharedPrompts;

namespace AvaFront.AutoGen.Builders
{
    public class AgentBuilder
    {
        public ConversableAgent Agent;
        public AzureOpenAIConfig agentLLMConfig;
        public AgentBuilder() {      
            agentLLMConfig = new AzureOpenAIConfig(
                endpoint: Environment.GetEnvironmentVariable("gpt35turbo:AzureOpenAIEndpoint") ?? throw new Exception("Please set OPENAI_API_KEY environment variable."),
                deploymentName: Environment.GetEnvironmentVariable("gpt35turbo:DeploymentName") ?? throw new Exception("Please set DeploymentName environment variable."),
                apiKey: Environment.GetEnvironmentVariable("gpt35turbo:ApiKey") ?? throw new Exception("Please set ApiKey environment variable.")
            );
        }

        // Base method to finalize the building process
        public ConversableAgent Build()
        {
            return this.Agent;
        }
    }

    public static class AgentBuilderExtensions
    {
        // Extension method to configure a UserAgent
        public static AgentBuilder UserAgent(this AgentBuilder builder)
        {
            builder.Agent = new AssistantAgent("User");
            return builder;
        }
        // Extension method to configure an AssistantAgent
        public static AgentBuilder AssistantAgent(this AgentBuilder builder)
        {
            builder.Agent = new AssistantAgent("Assistant");
            return builder;
        }

        // Extension method to configure a UserProxyAgent
        public static AgentBuilder UserProxyAgent(this AgentBuilder builder)
        {
            builder.Agent = new UserProxyAgent("UserProxyAgent");
            return builder;
        }
        // Extension method to configure a UserAgent
        public static AgentBuilder SystemAgent(this AgentBuilder builder)
        {
            builder.Agent = new AssistantAgent("System");
            return builder;
        }

        // Extension method to configure a InformationalAgent
        public static AgentBuilder InformationAgent(this AgentBuilder builder, string agentPersona, string ragData)
        {
            var gpt4config = new AzureOpenAIConfig(
                endpoint: Environment.GetEnvironmentVariable("gpt4:AzureOpenAIEndpoint") ?? throw new Exception("Please set OPENAI_API_KEY environment variable."),
                deploymentName: Environment.GetEnvironmentVariable("gpt4:DeploymentName") ?? throw new Exception("Please set DeploymentName environment variable."),
                apiKey: Environment.GetEnvironmentVariable("gpt4:ApiKey") ?? throw new Exception("Please set ApiKey environment variable.")
            );
            builder.Agent = new AssistantAgent(
                name: "InformationAgent",
                systemMessage: new PromptBuilder().Add(SharedPromptRepo.InformationAgentPrompt(agentPersona, ragData)).Add(SharedPromptRepo.SafetySystemMessage).Build(),
                llmConfig: new ConversableAgentConfig
                {
                    Temperature = 0,
                    ConfigList = [gpt4config],
                });
            return builder;
        }
    }

    }
