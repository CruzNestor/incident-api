
namespace Incident.Application.DTOs.ServiceCatalog
{
    public class ServiceCatalogResponse
    {
        public required string OwnerEmail { get; set; }

        public required string Tier { get; set; }
    }
}
