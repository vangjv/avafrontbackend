using AvaFront.Infrastructure.CosmosDbData.Interfaces;
using Microsoft.Azure.Cosmos;

namespace AvaFront.Infrastructure.CosmosDbData
{
    public class CosmosDbContainer : ICosmosDbContainer
    {
        public Container _container { get; }

        public CosmosDbContainer(CosmosClient cosmosClient,
                                 string databaseName,
                                 string containerName)
        {
            this._container = cosmosClient.GetContainer(databaseName, containerName);
        }
    }
}
