
namespace Incident.Domain.Entities
{
    public class UserEnitity
    {
        public string Id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Roles { get; set; } = [];
    }
}
