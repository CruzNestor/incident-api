using Incident.Domain.Entities;
using Incident.Domain.Enums;
using Incident.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Incident.Infrastructure.Persistence.Relational.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly ApplicationDbContext _context;

        public IncidentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IncidentEntity> CreateAsync(IncidentEntity incident)
        {
            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            return incident;
        }

        public async Task<PagedResult<IncidentEntity>> GetAllAsync(
            int page,
            int pageSize,
            IncidentSeverity? severity,
            IncidentStatus? status,
            string? serviceId,
            SortEnum sort)
        {
            var query = _context.Incidents.AsQueryable();

            // Filtros opcionales
            if (severity != null)
                query = query.Where(x => x.Severity == severity);

            if (status != null)
                query = query.Where(x => x.Status == status);

            if (!string.IsNullOrEmpty(serviceId))
                query = query.Where(x => x.ServiceId == serviceId);

            // Total antes de paginar
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Orden dinámico
            query = sort == SortEnum.ASC
                ? query.OrderBy(x => x.CreatedAt)
                : query.OrderByDescending(x => x.CreatedAt);

            // Paginación
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<IncidentEntity>
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<IncidentEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Incidents
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IncidentEntity> UpdateStatusAsync(IncidentEntity incident, IncidentStatus status)
        {
            incident.Status = status;
            await _context.SaveChangesAsync();

            return new IncidentEntity
            {
                Id = incident.Id,
                Title = incident.Title,
                Description = incident.Description,
                Severity = incident.Severity,
                Status = incident.Status,
                ServiceId = incident.ServiceId,
            };
        }
    }
}
