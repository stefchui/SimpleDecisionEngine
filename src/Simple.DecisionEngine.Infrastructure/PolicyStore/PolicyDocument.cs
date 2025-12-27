using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple.DecisionEngine.Infrastructure.PolicyStore
{
    public sealed class PolicyDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string TenantId { get; set; } = default!;
        public string PolicyKey { get; set; } = default!;
        public int Version { get; set; }
        public int[] Actions { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // TTL field
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? ExpireAt { get; set; }
    }
}
