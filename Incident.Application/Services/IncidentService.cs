using Incident.Application.DTOs.Incident;
using Incident.Application.DTOs.PagedResponse;
using Incident.Application.Exceptions;
using Incident.Application.Interfaces.Incident;
using Incident.Application.Interfaces.ServiceCatalogClient;
using Incident.Domain.Entities;
using Incident.Domain.Enums;
using Incident.Domain.Interfaces;
using System.Text.Json;

namespace Incident.Application.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly IIncidentRepository _repository;
        private readonly IIncidentEventMongoRepository _eventRepository;
        private readonly IServiceCatalogClient _catalogClient;

        public IncidentService(
            IIncidentRepository repository, 
            IIncidentEventMongoRepository eventRepository,
            IServiceCatalogClient catalogClient)
        {
            _repository = repository;
            _eventRepository = eventRepository;
            _catalogClient = catalogClient;
        }

        public async Task<IncidentResponse> CreateAsync(CreateIncidentRequest request)
        {
            var incident = new IncidentEntity()
            {
                Title = request.Title,
                Description = request.Description,
                Severity = Enum.Parse<IncidentSeverity>(request.Severity),
                Status = IncidentStatus.OPEN,
                ServiceId = request.ServiceId
            };

            var created = await _repository.CreateAsync(incident);
            var correlationId = Guid.NewGuid().ToString();

            await _eventRepository.CreateEventAsync(new IncidentEvent
            {
                IncidentId = created.Id,
                Type = "incident_created",
                OccurredAt = DateTime.UtcNow,
                Payload = JsonSerializer.SerializeToElement(new { 
                    created.Title,
                    Severity = request.Severity.ToString(),
                    Status = IncidentStatus.OPEN.ToString(),
                }),
                Metadata = new Dictionary<string, string>
                {
                    { "CorrelationId", correlationId }
                }
            });

            try
            {
                var service = await _catalogClient.GetServiceAsync(created.ServiceId);

                await _eventRepository.CreateEventAsync(new IncidentEvent
                {
                    IncidentId = created.Id,
                    Type = "service_catalog_snapshot",
                    OccurredAt = DateTime.UtcNow,
                    Payload = JsonSerializer.SerializeToElement(service),
                    Metadata = new Dictionary<string, string>
                    {
                        { "CorrelationId", correlationId },
                        { "Status", service == null ? "not_found" : "success" }
                    }
                });
            }
            catch
            {
                await _eventRepository.CreateEventAsync(new IncidentEvent
                {
                    IncidentId = created.Id,
                    Type = "service_catalog_snapshot",
                    OccurredAt = DateTime.UtcNow,
                    Payload = JsonSerializer.SerializeToElement(
                        new { Error = "catalog_unavailable" }
                    ),
                    Metadata = new Dictionary<string, string>
                    {
                        { "CorrelationId", correlationId },
                        { "Status", "error" }
                    }
                });
            }

            return new IncidentResponse
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                Severity = created.Severity.ToString(),
                Status = created.Status.ToString(),
                ServiceId = created.ServiceId
            };
        }

        public async Task<PagedResponse<IncidentResponse>> GetAllAsync(
            int page,
            int pageSize,
            IncidentSeverity? severity,
            IncidentStatus? status,
            string? serviceId,
            SortEnum sort)
        {
            var pagedEntities = await _repository.GetAllAsync(page, pageSize, severity, status, serviceId, sort);

            return new PagedResponse<IncidentResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = pagedEntities.TotalItems,
                TotalPages = pagedEntities.TotalPages,
                Items = [.. pagedEntities.Items.Select(x => new IncidentResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Severity = x.Severity.ToString(),
                    Status = x.Status.ToString(),
                    ServiceId = x.ServiceId,
                    CreatedAt = x.CreatedAt
                })]
            };
        }

        public async Task<IncidentResponse?> GetByIdAsync(Guid id)
        {
            var incident = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Not found");
            var events = await _eventRepository.GetByIncidentIdAsync(id);

            return new IncidentResponse
            {
                Id = incident.Id,
                Title = incident.Title,
                Description = incident.Description,
                Severity = incident.Severity.ToString(),
                Status = incident.Status.ToString(),
                ServiceId = incident.ServiceId,
                CreatedAt= incident.CreatedAt,
                Events = events
            };
        }
    
        public async Task<IncidentResponse> UpdateStatusAsync(Guid id, IncidentStatus status)
        {
            var incident = await _repository
                .GetByIdAsync(id) ?? throw new NotFoundException("Not found");

            var updated = await _repository.UpdateStatusAsync(incident, status);

            var correlationId = Guid.NewGuid().ToString();

            await _eventRepository.CreateEventAsync(new IncidentEvent
            {
                IncidentId = updated.Id,
                Type = "incident_status_changed",
                OccurredAt = DateTime.UtcNow,
                Payload = JsonSerializer.SerializeToElement(new
                {
                    updated.Title,
                    Severity = updated.Severity.ToString(),
                    Status = status.ToString()
                }),
                Metadata = new Dictionary<string, string>
                {
                    { "CorrelationId", correlationId }
                }
            });

            return new IncidentResponse
            {
                Id = updated.Id,
                Title = updated.Title,
                Description = updated.Description,
                Severity = updated.Severity.ToString(),
                Status = updated.Status.ToString(),
                ServiceId = updated.ServiceId
            }; ;
        }
    }
}
