using AvaFront.Infrastructure.CosmosDbData.Interfaces;
using AvaFront.AutoGen.Models;

namespace AvaFront.Infrastructure.CosmosDbData.Repository
{
    public class ConversationHistoryRepository : CosmosDbRepository<ConversationHistory>
    {
        /// <summary>
        ///     CosmosDB container name
        /// </summary>
        public override string ContainerName { get; } = "ConversationHistory";

        public ConversationHistoryRepository(ICosmosDbContainerFactory factory) : base(factory)
        { 

        }
    }
}
