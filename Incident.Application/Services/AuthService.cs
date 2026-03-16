using Incident.Application.DTOs.Auth;
using Incident.Application.Exceptions;
using Incident.Application.Interfaces.Auth;
using Incident.Domain.Interfaces;

namespace Incident.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _userRepository
                .FindByNameAsync(request.UserName) ?? throw new UnauthorizedException("Usuario o contraseña incorrectos");
            
            var isValid = await _userRepository.CheckPasswordAsync(user.Id, request.Password);

            if (!isValid)
            {
                throw new UnauthorizedException("Usuario o contraseña incorrectos");
            }

            var token = _userRepository.GenerateToken(user);

            return new LoginResponse
            {
                Id = user.Id,
                Token = token,
                UserName = request.UserName,
                Role = (user.Roles != null && user.Roles.First() != null) ? user.Roles.First() : ""
            };
        }
    }
}
