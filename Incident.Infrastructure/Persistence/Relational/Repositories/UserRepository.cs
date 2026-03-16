using Incident.Application.DTOs.Auth;
using Incident.Domain.Entities;
using Incident.Domain.Interfaces;
using Incident.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Incident.Infrastructure.Persistence.Relational.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public UserRepository(UserManager<AppUser> userManager, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
        }

        public async Task<UserEnitity?> FindByIdAsync(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null) return null;

            return new UserEnitity
            {
                Id = appUser.Id,
                FirstName = appUser.FirstName ?? "",
                LastName = appUser.LastName ?? "",
                UserName = appUser.UserName!,
                Email = appUser.Email!,
                Roles = (await _userManager.GetRolesAsync(appUser)).ToList()
            };
        }

        public async Task<UserEnitity?> FindByNameAsync(string username)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return null;

            return new UserEnitity
            {
                Id = appUser.Id,
                FirstName = appUser.FirstName ?? "",
                LastName = appUser.LastName ?? "",
                UserName = appUser.UserName!,
                Email = appUser.Email!,
                Roles = (await _userManager.GetRolesAsync(appUser)).ToList()
            };
        }

        public async Task<UserEnitity?> FindByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) return null;

            return new UserEnitity
            {
                Id = appUser.Id,
                FirstName = appUser.FirstName ?? "",
                LastName = appUser.LastName ?? "",
                UserName = appUser.UserName!,
                Email = appUser.Email!,
                Roles = (await _userManager.GetRolesAsync(appUser)).ToList()
            };
        }

        public async Task<UserEnitity> CreateAsync(UserEnitity user)
        {
            var appUser = new AppUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email
            };

            await _userManager.CreateAsync(appUser);
            foreach (var role in user.Roles)
            {
                await _userManager.AddToRoleAsync(appUser, role);
            }
            
            user.Id = appUser.Id;
            return user;
        }

        public async Task<bool> CheckPasswordAsync(string id, string password)
        {
            var appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
                return false;

            return await _userManager.CheckPasswordAsync(appUser, password);
        }

        public string GenerateToken(UserEnitity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Value.Secret);

            var claims = new List<Claim>
            {
                //new(JwtRegisteredClaimNames.Sub, user.Id),
                //new(JwtRegisteredClaimNames.Name, user.UserName),
                //new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email)

            };

            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOptions.Value.Audience,
                Issuer = _jwtOptions.Value.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
