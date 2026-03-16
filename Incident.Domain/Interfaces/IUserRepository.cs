using Incident.Domain.Entities;

namespace Incident.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEnitity?> FindByIdAsync(string id);
        Task<UserEnitity?> FindByNameAsync(string username);
        Task<UserEnitity?> FindByEmailAsync(string email);
        Task<UserEnitity> CreateAsync(UserEnitity user);
        Task<bool> CheckPasswordAsync(string id, string password);
        string GenerateToken(UserEnitity user);
    }
}
