using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Infrastructure.Security.TrustedIpSources.Configurations;

namespace Unjai.Platform.Infrastructure.Security.Forwarding.Extensions;

public static class TrustedIpSourceExtensions
{
    public static void AddTrustedIpSources(this IServiceCollection services,
        TrustIpSourceOptions trustIpSourceOptions)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;

            foreach (var cidr in trustIpSourceOptions.TrustedNetworks)
            {
                if (System.Net.IPNetwork.TryParse(cidr, out var network))
                    options.KnownIPNetworks.Add(network);
                else
                    throw new InvalidOperationException($"Invalid CIDR: {cidr}");
            }

            foreach (var ip in trustIpSourceOptions.TrustedProxies)
            {
                if (System.Net.IPAddress.TryParse(ip, out var address))
                    options.KnownProxies.Add(address);
                else
                    throw new InvalidOperationException($"Invalid IP: {ip}");
            }

            if (trustIpSourceOptions.ForwardLimit.HasValue)
                options.ForwardLimit = trustIpSourceOptions.ForwardLimit.Value;

            options.RequireHeaderSymmetry = true;
        });
    }

    public static WebApplication UseTrustedIpSources(this WebApplication app)
    {
        app.UseForwardedHeaders();

        return app;
    }
}
