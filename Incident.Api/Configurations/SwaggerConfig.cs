using Microsoft.OpenApi.Models;

namespace Incident.Api.Configurations
{
    public static class SwaggerConfig
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Reybanpac API",
                    Version = "v1",
                    Description = "API .NET 9",
                });

                // Añadir definición de seguridad para JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    Description = "Introduce el token JWT con el prefijo 'Bearer'. Ejemplo: 'Bearer <tu-token>'"
                });

                // Añadir requerimiento de seguridad para todas las operaciones
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                options.ResolveConflictingActions(apiDescription => apiDescription.First());
            });
        }
    }
}
