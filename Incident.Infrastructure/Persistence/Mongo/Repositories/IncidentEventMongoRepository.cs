using Incident.Domain.Entities;
using Incident.Domain.Interfaces;
using Incident.Infrastructure.Persistence.Mongo.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace Incident.Infrastructure.Persistence.Mongo.Repositories
{
    public class IncidentEventMongoRepository : IIncidentEventMongoRepository
    {

        private readonly IMongoCollection<IncidentEventDocument> _collection;

        public IncidentEventMongoRepository(IOptions<MongoSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);

            var database = mongoClient.GetDatabase(settings.Value.Database);

            _collection = database.GetCollection<IncidentEventDocument>(settings.Value.Collection);
        }

        public async Task CreateEventAsync(IncidentEvent incidentEvent)
        {
            var document = new IncidentEventDocument
            {
                IncidentId = incidentEvent.IncidentId.ToString(),
                Type = incidentEvent.Type,
                OccurredAt = incidentEvent.OccurredAt,
                Payload = BsonDocument.Parse(incidentEvent.Payload.GetRawText()),
                Metadata = incidentEvent.Metadata,
            };

            await _collection.InsertOneAsync(document);
        }

        public async Task<IEnumerable<IncidentEvent>> GetByIncidentIdAsync(Guid incidentId)
        {
            var filter = Builders<IncidentEventDocument>
                .Filter.Eq(x => x.IncidentId, incidentId.ToString());

            var events = await _collection
                .Find(filter)
                .SortByDescending(x => x.OccurredAt)
                .ToListAsync();

            return events.Select(e => new IncidentEvent
            {
                Id = e.Id,
                Type = e.Type,
                IncidentId = Guid.Parse(e.IncidentId),
                OccurredAt = e.OccurredAt,
                Payload = JsonSerializer.Deserialize<JsonElement>(e.Payload.ToJson()),
                Metadata = e.Metadata,
            });
        }
    }
}
