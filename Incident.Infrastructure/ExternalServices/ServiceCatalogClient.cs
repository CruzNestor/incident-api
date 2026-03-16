using Incident.Application.DTOs.ServiceCatalog;
using Incident.Application.Interfaces.ServiceCatalogClient;
using System.Net.Http.Json;

namespace Incident.Infrastructure.ExternalServices
{
    public class ServiceCatalogClient : IServiceCatalogClient
    {
        private readonly HttpClient _httpClient;

        public ServiceCatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceCatalogResponse?> GetServiceAsync(string serviceId)
        {
            var response = await _httpClient.GetAsync($"/services/{serviceId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ServiceCatalogResponse>();
        }
    }
}
