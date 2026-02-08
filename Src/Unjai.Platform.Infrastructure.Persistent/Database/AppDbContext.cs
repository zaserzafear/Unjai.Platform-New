using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Domain.Entities.Tenants;
using Unjai.Platform.Infrastructure.Persistent.Database.Conventions;
using Unjai.Platform.Infrastructure.Persistent.Outbox;

namespace Unjai.Platform.Infrastructure.Persistent.Database;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        SoftDeleteConvention.Apply(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
}
