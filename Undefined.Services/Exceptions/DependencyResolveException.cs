namespace Undefined.Services.Exceptions;

public class DependencyResolveException : Exception
{
    public DependencyResolveException(string? message) : base(message)
    {
    }
}