using Microsoft.EntityFrameworkCore;

namespace Unjai.Platform.Infrastructure.Persistent.Database;

internal sealed class ReadDbContext
    : AppDbContext<ReadDbContext>
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options)
        : base(options)
    {
    }
}
