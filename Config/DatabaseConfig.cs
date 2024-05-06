using AvaFront.Infrastructure.AppSettings;
using AvaFront.Infrastructure.CosmosDbData.Repository;
using AvaFront.Infrastructure.Extensions;

namespace AvaFront.API.Config
{
    public static class DatabaseConfig
    {
        /// <summary>
        ///     Setup Cosmos DB
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void SetupCosmosDb(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind database-related bindings
            CosmosDbSettings cosmosDbConfig = configuration.GetSection("AvaFrontDatabase").Get<CosmosDbSettings>();

            // register CosmosDB client and data repositories
            services.AddCosmosDb(cosmosDbConfig.EndpointUrl,
                                 cosmosDbConfig.PrimaryKey,
                                 cosmosDbConfig.DatabaseName,
                                 cosmosDbConfig.Containers);

            services.AddSingleton<ConversationHistoryRepository, ConversationHistoryRepository>();
        }
    }
}
