
using Incident.Domain.Entities;

namespace Incident.Application.DTOs.Incident
{
    public class IncidentResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Severity { get; set; }
        public required string Status { get; set; }
        public string ServiceId { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }
        public IEnumerable<IncidentEvent> Events { get; set; } = [];
    }
}
