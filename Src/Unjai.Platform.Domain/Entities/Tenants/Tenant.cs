using Unjai.Platform.Domain.Abstractions;

namespace Unjai.Platform.Domain.Entities.Tenants;

public sealed class Tenant : EntityBase
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private Tenant() { }

    public Tenant(string code, string name)
    {
        SetCode(code);
        SetName(name);
        IsActive = true;
    }

    public void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Tenant code is required.");

        if (code.Length > 50)
            throw new DomainException("Tenant code must not exceed 50 characters.");

        Code = code.Trim();
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tenant name is required.");

        if (name.Length > 200)
            throw new DomainException("Tenant name must not exceed 200 characters.");

        Name = name.Trim();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
