using Microsoft.EntityFrameworkCore;

namespace Unjai.Platform.Infrastructure.Persistent.Database;

internal sealed class WriteDbContext(DbContextOptions<AppDbContext> options) : AppDbContext(options)
{
}
