
namespace Incident.Infrastructure.Identity
{
    public class AppRole : AuditableIdentityRole
    {
        public ICollection<AppUserRole> UserRoles { get; } = [];
    }
}
