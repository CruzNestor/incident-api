using Incident.Application.Interfaces.UserContext;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Incident.Infrastructure.Secutiry
{
    public class HttpUserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId =>
            _httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
