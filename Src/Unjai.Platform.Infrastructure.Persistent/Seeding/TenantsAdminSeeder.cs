using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.TenantAdmins;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Domain.Entities.TenantsAdmin;
using Unjai.Platform.Domain.Entities.TenantsAdminRole;
using Unjai.Platform.Infrastructure.Security.Cryptography.Hashing;

namespace Unjai.Platform.Infrastructure.Persistent.Seeding;

public static class TenantsAdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct)
    {
        using var scope = services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(TenantsAdminSeeder));
        var repository = scope.ServiceProvider.GetRequiredService<ITenantAdminRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        bool hasAdmin = await repository.HasDefaultAdminAsync(ct);

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

        await repository.CreateAsync(admin, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogWarning(
            "Initial tenants admin created. Username: admin , Password: {Password}", password);
    }
}
