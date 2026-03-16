using Incident.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Incident.Domain.Entities
{
    [Table("Incidents", Schema = "INC")]
    public class IncidentEntity : AuditFields
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(100)]
        public required string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public required IncidentSeverity Severity { get; set; }

        public required IncidentStatus Status { get; set; } = IncidentStatus.OPEN;

        [StringLength(100)]
        public required string ServiceId { get; set; } = string.Empty;
    }
}
