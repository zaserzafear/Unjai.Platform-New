namespace Unjai.Platform.Infrastructure.Security.TrustedIpSources.Configurations;

public class TrustIpSourceOptions
{
    public string[] TrustedNetworks { get; init; } = [];
    public string[] TrustedProxies { get; init; } = [];
    public int? ForwardLimit { get; init; }
}
