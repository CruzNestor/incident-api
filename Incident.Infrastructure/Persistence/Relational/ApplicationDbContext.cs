using Incident.Application.Interfaces.UserContext;
using Incident.Domain.Entities;
using Incident.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Incident.Infrastructure.Persistence.Relational
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>, AppUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        private readonly IUserContext _userContext;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IUserContext userContext
            ) : base(options)
        {
            _userContext = userContext;
        }

        public DbSet<IncidentEntity> Incidents { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is AuditFields ||
                    e.Entity is AuditableIdentityRole ||
                    e.Entity is AuditableIdentityUser
                );

            foreach (var entry in entries)
            {
                var entity = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    dynamic auditable = entity;
                    auditable.CreatedAt = DateTime.UtcNow;
                    auditable.CreatedBy = _userContext.UserId ?? "SYSTEM";
                }

                if (entry.State == EntityState.Modified)
                {
                    dynamic auditable = entity;
                    auditable.UpdatedAt = DateTime.UtcNow;
                    auditable.UpdatedBy = _userContext.UserId ?? "SYSTEM";
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(AuditFields).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType)
                        .Property(nameof(AuditFields.CreatedAt))
                        .ValueGeneratedOnAdd()
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                    builder.Entity(entityType.ClrType)
                        .Property(nameof(AuditFields.CreatedBy))
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                }
            }

            builder.Entity<AppUser>(entity =>
            {
                entity.HasMany(e => e.UserRoles)
                    .WithOne(ur => ur.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                entity.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();
            });

            // Configuración para AppUserRole
            builder.Entity<AppUserRole>(entity =>
            {
                // Clave compuesta usando UserId y RoleId
                entity.HasKey(e => new { e.UserId, e.RoleId });

                // Relación con AppUser
                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .IsRequired();

                // Relación con AppRoles
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .IsRequired();
            });

            builder.Entity<IncidentEntity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.Severity)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.ServiceId)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.Entity<IncidentEntity>()
                .HasIndex(x => new { x.Severity, x.Status,  x.ServiceId, x.CreatedAt })
                .HasDatabaseName("IX_Incidents_Filter_Order");
        }
    }
}
