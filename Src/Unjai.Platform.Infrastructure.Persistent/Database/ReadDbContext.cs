using Microsoft.EntityFrameworkCore;

namespace Unjai.Platform.Infrastructure.Persistent.Database;

internal sealed class ReadDbContext : AppDbContext
{
    public ReadDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
