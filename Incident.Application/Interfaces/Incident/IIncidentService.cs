using Incident.Application.DTOs.Incident;
using Incident.Application.DTOs.PagedResponse;
using Incident.Domain.Enums;

namespace Incident.Application.Interfaces.Incident
{
    public interface IIncidentService
    {
        Task<IncidentResponse> CreateAsync(CreateIncidentRequest request);
        Task<PagedResponse<IncidentResponse>> GetAllAsync(
            int page, 
            int pageSize,
            IncidentSeverity? severity,
            IncidentStatus? status,
            string? serviceId,
            SortEnum sort
        );
        Task<IncidentResponse?> GetByIdAsync(Guid id);
        Task<IncidentResponse> UpdateStatusAsync(Guid id, IncidentStatus status);
    }
}
