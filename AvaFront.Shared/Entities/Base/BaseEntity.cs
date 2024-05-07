using System.Text.Json.Serialization;
namespace AvaFront.Shared.Entities.Base
{
    public abstract class BaseEntity
    {
        public virtual string id { get; set; } = Guid.NewGuid().ToString();
        public virtual int SchemaVersion { get; set; }
    }
}
