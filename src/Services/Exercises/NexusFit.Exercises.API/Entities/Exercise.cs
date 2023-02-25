using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NexusFit.Exercises.API.Entities;

public sealed class Exercise
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
