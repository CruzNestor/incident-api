using Incident.Infrastructure.Persistence.Relational;
using Microsoft.EntityFrameworkCore;

namespace Incident.Api.Configurations
{
    public static class AutomaticMigrationConfig
    {
        public static async Task ValidAutomaticMigration(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            bool ejecutarMigracion = configuration.GetValue<bool>("RunMigrationsOnStartup");
            if (!ejecutarMigracion) return;

            try
            {
                logger.LogInformation("Applying migrations...");

                var context = services.GetRequiredService<ApplicationDbContext>();

                await context.Database.MigrateAsync();

                logger.LogInformation("Migrations applied successfully.");

                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error applying migrations.");
                throw;
            }

            //var retries = 3;

            //while (retries > 0)
            //{
            //    try
            //    {
            //        logger.LogInformation("Applying migrations...");

            //        var context = services.GetRequiredService<ApplicationDbContext>();

            //        await context.Database.MigrateAsync();

            //        logger.LogInformation("Migrations applied successfully.");

            //        return;
            //    }
            //    catch (Exception ex)
            //    {
            //        retries--;

            //        logger.LogWarning(ex, "Database not ready. Retrying in 2 seconds...");

            //        await Task.Delay(2000);
            //    }
            //}

            //throw new Exception("Could not apply migrations after several retries.");
        }
    }
}
