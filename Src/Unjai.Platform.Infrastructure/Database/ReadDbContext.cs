using Microsoft.EntityFrameworkCore;

namespace Unjai.Platform.Infrastructure.Database;

internal sealed class ReadDbContext : AppDbContext
{
    public ReadDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
}
