namespace Undefined.Services.Exceptions;

public class ServiceRuntimeResolverException : Exception
{
    public ServiceRuntimeResolverException(string? message) : base(message)
    {
    }
}