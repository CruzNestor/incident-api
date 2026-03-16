
namespace Incident.Application.DTOs.Auth
{
    public class JwtOptions
    {
        public string Audience { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
}
