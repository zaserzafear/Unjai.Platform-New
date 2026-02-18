using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Domain.Entities.TenantsAdmin;
using Unjai.Platform.Domain.Entities.TenantsAdminRole;
using Unjai.Platform.Infrastructure.Persistent.Database;
using Unjai.Platform.Infrastructure.Security.Cryptography.Hashing;

namespace Unjai.Platform.Infrastructure.Persistent.Seeding;

public static class TenantsAdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(TenantsAdminSeeder));

        bool hasAdmin = await db.TenantAdmins
            .AsNoTracking()
            .IgnoreQueryFilters()
            .AnyAsync();

        if (hasAdmin)
            return;

        string password = Guid.NewGuid().ToString();
        string passwordHash = PasswordHasher.Hash(password);

        var admin = new TenantAdmin
        {
            Username = "admin",
            PasswordHash = passwordHash,
            IsActive = true,
            RoleId = (int)TenantAdminRoleCode.SuperAdmin
        };

        db.TenantAdmins.Add(admin);
        await db.SaveChangesAsync();

        logger.LogWarning(
            "Initial tenants admin created. Username: admin , Password: {Password}", password);
    }
}
