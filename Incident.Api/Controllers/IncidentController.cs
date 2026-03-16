using Incident.Application.DTOs.Incident;
using Incident.Application.Interfaces.Incident;
using Incident.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Incident.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _service;

        public IncidentController(IIncidentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateIncidentRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] IncidentSeverity? severity = null,
            [FromQuery] IncidentStatus? status = null,
            [FromQuery] string? serviceId = null,
            [FromQuery] SortEnum sort = SortEnum.DESC)
        {
            return Ok(await _service.GetAllAsync(page, pageSize, severity, status, serviceId, sort));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var incident = await _service.GetByIdAsync(id);
            return Ok(incident);
        }

        [HttpPatch("{id}/{status}")]
        public async Task<IActionResult> UpdateStatus(Guid id, IncidentStatus status)
        {
            var incident = await _service.UpdateStatusAsync(id, status);
            return Ok(incident);
        }
    }
}
