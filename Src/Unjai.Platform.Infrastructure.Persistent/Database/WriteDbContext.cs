using Microsoft.EntityFrameworkCore;

namespace Unjai.Platform.Infrastructure.Persistent.Database;

internal sealed class WriteDbContext
    : AppDbContext<WriteDbContext>
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options)
        : base(options)
    {
    }
}
