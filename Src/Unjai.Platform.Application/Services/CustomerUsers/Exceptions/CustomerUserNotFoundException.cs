namespace Unjai.Platform.Application.Services.CustomerUsers.Exceptions;

public sealed class CustomerUserNotFoundException : Exception
{
    public CustomerUserNotFoundException(string message) : base(message)
    {
    }
}
