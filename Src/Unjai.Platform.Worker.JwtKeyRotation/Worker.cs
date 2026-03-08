using Unjai.Platform.Application.Services.JwtKeyStores;

namespace Unjai.Platform.Worker.JwtKeyRotation;

public class Worker(
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
            logger.LogInformation("JWT key rotation job started.");

            using var scope = serviceProvider.CreateScope();

            var keyStoreService =
                scope.ServiceProvider.GetRequiredService<JwtKeyStoreService>();

            var rotateBeforeExpiry =
                config.GetValue<TimeSpan>("JwtKeyRotation:RotateBeforeExpiry");

            var keyLifetime =
                config.GetValue<TimeSpan>("JwtKeyRotation:KeyLifetime");

            var rotated = await keyStoreService.RotateKeyIfNeededAsync(
                rotateBeforeExpiry,
                keyLifetime,
                stoppingToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "JWT key rotation job completed. Rotated: {Rotated}",
                    rotated);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning("JWT key rotation job was cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "JWT key rotation job failed.");
            throw;
        }
        finally
        {
            logger.LogInformation("JWT key rotation job shutting down.");
            lifetime.StopApplication();
        }
    }
}
