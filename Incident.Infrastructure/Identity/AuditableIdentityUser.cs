using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Incident.Infrastructure.Identity
{
    public class AuditableIdentityUser : IdentityUser
    {
        [JsonIgnore]
        public bool Active { get; set; } = true;

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = default!;

        [JsonIgnore]
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        [JsonIgnore]
        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
    }
}
