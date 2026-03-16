using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Incident.Infrastructure.Persistence.Mongo.Documents
{
    public class IncidentEventDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        public required string IncidentId { get; set; }

        public required string Type { get; set; }

        public DateTime OccurredAt { get; set; }

        public required BsonDocument Payload { get; set; }

        public required Dictionary<string, string> Metadata { get; set; }
    }
}
