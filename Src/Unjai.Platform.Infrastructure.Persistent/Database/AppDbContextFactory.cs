using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Unjai.Platform.Infrastructure.Persistent.Database;

internal sealed class WriteDbContextFactory
    : IDesignTimeDbContextFactory<WriteDbContext>
{
    public WriteDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("POSTGRES_CONNECTIONSTRING")
            ?? "Host=localhost;Port=5432;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<WriteDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new WriteDbContext(options);
    }
}
