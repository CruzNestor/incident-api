using Incident.Domain.Entities;
using Incident.Domain.Enums;

namespace Incident.Domain.Interfaces
{
    public interface IIncidentRepository
    {
        Task<IncidentEntity> CreateAsync(IncidentEntity incident);

        Task<PagedResult<IncidentEntity>> GetAllAsync(
            int page,
            int pageSize,
            IncidentSeverity? severity,
            IncidentStatus? status,
            string? serviceId,
            SortEnum sort
        );

        Task<IncidentEntity?> GetByIdAsync(Guid id);

        Task<IncidentEntity> UpdateStatusAsync(IncidentEntity incident, IncidentStatus status);
    }
}
