using Incident.Application.Interfaces.ServiceCatalogClient;
using Incident.Application.Services;
using Incident.Domain.Entities;
using Incident.Domain.Enums;
using Incident.Domain.Interfaces;
using Moq;

namespace Incident.Api.Tests.Services
{
    public class IncidentServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnIncidentResponse_WhenIncidentExists()
        {
            // Arrange
            var incidentId = Guid.NewGuid();

            var incidentEntity = new IncidentEntity
            {
                Id = incidentId,
                Title = "Test incident",
                Description = "Test description",
                Severity = IncidentSeverity.HIGH,
                Status = IncidentStatus.OPEN,
                ServiceId = "service-1"
            };

            var repositoryMock = new Mock<IIncidentRepository>();
            var eventRepositoryMock = new Mock<IIncidentEventMongoRepository>();
            var serviceCatalogClientMock = new Mock<IServiceCatalogClient>();

            repositoryMock
                .Setup(x => x.GetByIdAsync(incidentId))
                .ReturnsAsync(incidentEntity);

            eventRepositoryMock
                .Setup(x => x.GetByIncidentIdAsync(incidentId))
                .ReturnsAsync([]);

            var service = new IncidentService(
                repositoryMock.Object,
                eventRepositoryMock.Object,
                serviceCatalogClientMock.Object
            );

            // Act
            var result = await service.GetByIdAsync(incidentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(incidentId, result!.Id);
            Assert.Equal("Test incident", result.Title);
            Assert.Equal("HIGH", result.Severity);
            Assert.Equal("OPEN", result.Status);
        }

    }
}
