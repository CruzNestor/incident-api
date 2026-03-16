using Incident.Application.DTOs.Auth;

namespace Incident.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest request);
    }
}
