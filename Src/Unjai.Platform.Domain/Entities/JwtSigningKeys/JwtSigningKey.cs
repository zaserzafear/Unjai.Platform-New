namespace Unjai.Platform.Domain.Entities.JwtSigningKeys;

public sealed class JwtSigningKey
{
    public string KeyId { get; set; } = default!;
    public string PublicKeyPem { get; set; } = default!;
    public string? PrivateKeyPem { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
