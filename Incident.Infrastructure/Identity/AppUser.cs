using Microsoft.AspNetCore.Identity;

namespace Incident.Infrastructure.Identity
{
    public class AppUser : AuditableIdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public virtual ICollection<AppUserRole> UserRoles { get; set; } = [];

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = [];
    }
}
