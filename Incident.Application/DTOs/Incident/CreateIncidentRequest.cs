
namespace Incident.Application.DTOs.Incident
{
    public class CreateIncidentRequest
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Severity { get; set; }
        public string ServiceId { get; set; } = String.Empty;
    }
}
