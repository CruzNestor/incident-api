using Incident.Application.DTOs.ServiceCatalog;

namespace Incident.Application.Interfaces.ServiceCatalogClient
{
    public interface IServiceCatalogClient
    {
        Task<ServiceCatalogResponse?> GetServiceAsync(string serviceId);
    }
}
