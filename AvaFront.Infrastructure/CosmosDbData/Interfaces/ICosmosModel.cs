using Newtonsoft.Json;

namespace AvaFront.Infrastructure.CosmosDbData.Interfaces
{
    public interface ICosmosModel
    {
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
    }
}
