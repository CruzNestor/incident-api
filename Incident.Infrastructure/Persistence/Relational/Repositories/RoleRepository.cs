using Incident.Domain.Entities;
using Incident.Domain.Interfaces;
using Incident.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Incident.Infrastructure.Persistence.Relational.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleRepository(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<RoleEntity?> FindByNameAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null) return null;

            return new RoleEntity
            {
                Id = role.Id,
                Name = role.Name ?? "",
            };
        }
    }
}
