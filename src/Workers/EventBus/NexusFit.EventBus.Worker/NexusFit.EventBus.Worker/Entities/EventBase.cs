using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NexusFit.EventBus.Worker.Entities;

public class EventBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
    public Guid EventId { get; private set; }

    public DateTime CreationDate { get; set; }
    public bool Published { get; set; }
}
