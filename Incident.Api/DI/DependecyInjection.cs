using Incident.Application.Interfaces.Auth;
using Incident.Application.Interfaces.Incident;
using Incident.Application.Interfaces.UserContext;
using Incident.Application.Services;
using Incident.Domain.Interfaces;
using Incident.Infrastructure.Persistence.Mongo.Repositories;
using Incident.Infrastructure.Persistence.Relational.Repositories;
using Incident.Infrastructure.Secutiry;

namespace Incident.Api.DI
{
    public static class DependecyInjection
    {
        public static void InjectionContainer(this IServiceCollection services)
        {
            // User
            services.AddScoped<IUserRepository, UserRepository>();

            // Http User Context
            services.AddScoped<IUserContext, HttpUserContext>();

            // Role
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Auth
            services.AddScoped<IAuthService, AuthService>();

            // Mongo Incident Event
            services.AddScoped<IIncidentEventMongoRepository, IncidentEventMongoRepository>();

            // Incident
            services.AddScoped<IIncidentRepository, IncidentRepository>();
            services.AddScoped<IIncidentService, IncidentService>();

        }
    }
}
