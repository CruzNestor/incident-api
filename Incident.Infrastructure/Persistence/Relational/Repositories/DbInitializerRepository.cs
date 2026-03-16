using Incident.Domain.Entities;
using Incident.Domain.Enums;
using Incident.Domain.Interfaces;
using Incident.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Text.Json;

namespace Incident.Infrastructure.Persistence.Relational.Repositories
{
    public static class DbInitializerRepository
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var incidentEventRepository = scope.ServiceProvider
                .GetRequiredService<IIncidentEventMongoRepository>();

            // 1. Crear roles
            List<string> roles = ["USER", "ADMIN"];
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole {
                        Name = roleName
                    });
                }
            }

            // 2. Crear usuarios por defecto
            var defaultUsers = new List<(string UserName, string Password, string Role)>
            {
                ("admin@email.com", "Admin123!", "ADMIN"),
                ("user@email.com", "User123!", "USER"),
                ("juanito@email.com", "Juanito123!", "USER"),
            };

            foreach (var (userName, password, role) in defaultUsers)
            {
                if (await userManager.FindByNameAsync(userName) is null)
                {
                    var user = new AppUser
                    {
                        UserName = userName,
                        Email = userName,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                    else
                    {
                        throw new Exception($"Error creating user {userName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // 3. Crear incidencias por defecto
            if (!context.Incidents.Any())
            {
                var incidents = new List<IncidentEntity>
                {
                    new() {
                        Id = Guid.NewGuid(),
                        Title = "Pago falla al confirmar",
                        Description = "Se recibe como respuesta un Error 500",
                        Severity = IncidentSeverity.HIGH,
                        Status = IncidentStatus.OPEN,
                        ServiceId = "payments-service",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "SYSTEM"
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        Title = "Falla conexión a base de datos",
                        Description = "La conexíon a la base de datos tarda mucho tiempo y no responde",
                        Severity = IncidentSeverity.MEDIUM,
                        Status = IncidentStatus.OPEN,
                        ServiceId = "database-service",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "SYSTEM"
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        Title = "Notificaciones por correo tiene retraso",
                        Description = "Los correos electronicos se acumulan por demasiado tiempo",
                        Severity = IncidentSeverity.LOW,
                        Status = IncidentStatus.OPEN,
                        ServiceId = "notification-service",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "SYSTEM"
                    }
                };

                await context.Incidents.AddRangeAsync(incidents);
                await context.SaveChangesAsync();

                foreach (var item in incidents)
                {
                    var severity = item.Severity.ToString();
                    var status = item.Status.ToString();

                    var newEvent = new IncidentEvent
                    {
                        IncidentId = item.Id,
                        Type = "incident_created",
                        OccurredAt = DateTime.UtcNow,
                        Payload = JsonSerializer.SerializeToElement(new Dictionary<string, string>
                        {
                            { "Title", item.Title },
                            { "Severity", severity },
                            { "Status", status }
                        }),
                        Metadata = new Dictionary<string, string>
                        {
                            { "CorrelationId", Guid.NewGuid().ToString() }
                        }
                    };

                    await incidentEventRepository.CreateEventAsync(newEvent);
                }

            }
        
        }
    }
}
