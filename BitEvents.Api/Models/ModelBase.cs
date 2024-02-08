using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BitEvents.Api.Models;

public class ModelBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAtUtc { get; set; } = null;

    public bool IsDeleted { get; set; } = false;
}