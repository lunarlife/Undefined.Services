namespace Undefined.Services.Exceptions;

public class ServiceResolveException : Exception
{
    public ServiceResolveException(string? message) : base(message)
    {
    }
}