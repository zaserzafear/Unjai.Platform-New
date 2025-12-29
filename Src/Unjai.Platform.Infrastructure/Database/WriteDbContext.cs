using Microsoft.EntityFrameworkCore;

namespace Unjai.Platform.Infrastructure.Database;

internal sealed class WriteDbContext(DbContextOptions<AppDbContext> options) : AppDbContext(options)
{
}
