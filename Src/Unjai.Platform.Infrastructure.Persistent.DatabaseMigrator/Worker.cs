using Unjai.Platform.Application.Repositories.TenantAdmins;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Domain.Entities.TenantsAdmin;
using Unjai.Platform.Domain.Entities.TenantsAdminRole;
using Unjai.Platform.Infrastructure.Persistent.Database.Extensions;
using Unjai.Platform.Infrastructure.Security.Cryptography.Hashing;

namespace Unjai.Platform.Infrastructure.Persistent.DatabaseMigrator;

public sealed class Worker(
    ILogger<Worker> logger,
    IServiceProvider serviceProvider,
    IConfiguration config,
    IHostApplicationLifetime lifetime)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Database migration phase started.");

            serviceProvider.ApplyMigrations();

            logger.LogInformation("Database migration phase completed.");

            logger.LogInformation("Bootstrap admin seeding phase started.");

            using var scope = serviceProvider.CreateScope();

            var tenantAdminRepository =
                scope.ServiceProvider.GetRequiredService<ITenantAdminRepository>();

            var unitOfWork =
                scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var hasDefaultAdmin =
                await tenantAdminRepository.HasDefaultAdminAsync(stoppingToken);

            if (hasDefaultAdmin)
            {
                logger.LogInformation(
                    "Bootstrap admin already exists. Seeding skipped.");
                return;
            }

            string username = config["BootstrapAdmin:Username"] ?? "admin";
            string password = config["BootstrapAdmin:Password"]
                ?? throw new InvalidOperationException(
                    "Bootstrap admin password is not configured.");

            var admin = new TenantAdmin
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                IsActive = true,
                RoleId = (int)TenantAdminRoleCode.SuperAdmin
            };

            await tenantAdminRepository.CreateAsync(admin, stoppingToken);
            await unitOfWork.SaveChangesAsync(stoppingToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                "Bootstrap admin account created successfully. Username: {Username}",
                username);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning("Database migrator execution was cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex,
                "Database migrator failed during execution. Application will terminate.");
            throw;
        }
        finally
        {
            logger.LogInformation("Shutting down database migrator host.");
            lifetime.StopApplication();
        }
    }
}
