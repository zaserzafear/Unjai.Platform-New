namespace Unjai.Platform.Infrastructure.Security.Networking.TrustedIp;

public class TrustIpSourceOptions
{
    public string[] TrustedNetworks { get; init; } = [];
    public string[] TrustedProxies { get; init; } = [];
    public int? ForwardLimit { get; init; }
}
