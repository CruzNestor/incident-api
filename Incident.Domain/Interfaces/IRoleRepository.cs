using Incident.Domain.Entities;

namespace Incident.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<RoleEntity?> FindByNameAsync(string roleName);
    }
}
