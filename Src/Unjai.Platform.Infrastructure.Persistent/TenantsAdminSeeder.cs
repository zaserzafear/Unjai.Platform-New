using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Domain.Entities.TenantsAdmin;
using Unjai.Platform.Infrastructure.Persistent.Database;
using Unjai.Platform.Infrastructure.Security.Helpers;

namespace Unjai.Platform.Infrastructure.Persistent;

public static class TenantsAdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(TenantsAdminSeeder));

        bool hasAdmin = await db.TenantAdmins
            .AnyAsync(x => !x.IsDeleted);

        if (hasAdmin)
            return;

        string password = Guid.NewGuid().ToString();
        string passwordHash = PasswordHasher.Hash(password);

        var admin = new TenantAdmin
        {
            Username = "admin",
            PasswordHash = passwordHash,
        };

        db.TenantAdmins.Add(admin);
        await db.SaveChangesAsync();

        logger.LogWarning(
            "Initial tenants admin created. Username: admin , Password: {Password}", password);
    }
}
