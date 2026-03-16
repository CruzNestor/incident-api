using Incident.Domain.Entities;

namespace Incident.Domain.Interfaces
{
    public interface IIncidentEventMongoRepository
    {
        Task CreateEventAsync(IncidentEvent incidentEvent);
        Task<IEnumerable<IncidentEvent>> GetByIncidentIdAsync(Guid incidentId);
    }
}
