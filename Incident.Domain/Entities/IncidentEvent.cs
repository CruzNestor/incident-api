using System.Text.Json;

namespace Incident.Domain.Entities
{
    public class IncidentEvent
    {
        public string Id { get; set; } = default!;

        public Guid IncidentId { get; set; }

        public string Type { get; set; } = default!;

        public DateTime OccurredAt { get; set; }

        public JsonElement Payload { get; set; }

        public required Dictionary<string, string> Metadata { get; set; }
    }
}
